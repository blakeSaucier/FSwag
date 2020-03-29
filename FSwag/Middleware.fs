[<AutoOpen>]
module FSwag.Middleware

open System
open System.IO
open FSharp.Control.Tasks.V2
open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Http

let private setResponseHeaders (context: HttpContext) =
    context.Response.Headers.["Content-Type"] <- StringValues("text/html; charset=utf-8")
    context.Response.StatusCode <- 200

let private writeResponse (context: HttpContext) document =
    task {
        do setResponseHeaders context
        do! context.Response.WriteAsync(document)
    }

let private transformHtml (html: string) swaggerDocLocation =
    html.Replace("{SwaggerDocLocation}", swaggerDocLocation)

let private readDocument (resource: string) =
    task {
        let stream = typeof<FSwagOptions>.Assembly.GetManifestResourceStream(resource)
        use reader = new StreamReader(stream)
        return! reader.ReadToEndAsync()
    }

let private redocDocument options =
    task {
        let! document = readDocument "FSwag.Res.Redoc.index.html"
        return transformHtml document options.DocumentLocation
    }

let private swaggerUiDocument options =
    task {
        let! document = readDocument "FSwag.Res.Swagger.index.html"
        return transformHtml document options.DocumentLocation
    }

let private isMatch (urlPath: PathString) swaggerPath =
    urlPath.HasValue && 
        String.Equals(urlPath.Value.TrimEnd('/'), swaggerPath, StringComparison.OrdinalIgnoreCase)

type ReDocMiddleware (next: RequestDelegate, options: FSwagOptions) =
    member _.Invoke(ctx: HttpContext) =
        task {
            if isMatch ctx.Request.Path options.SwaggerUrl then
                let! redoc = redocDocument options
                do! writeResponse ctx redoc
            else
                next.Invoke ctx |> ignore
        }

type SwaggerMiddleware (next: RequestDelegate, options: FSwagOptions) =
    member this.Invoke(ctx: HttpContext) =
        task {
            if isMatch ctx.Request.Path options.SwaggerUrl then
                let! swaggerUi = swaggerUiDocument options
                do! writeResponse ctx swaggerUi
            else
                next.Invoke ctx |> ignore
        }