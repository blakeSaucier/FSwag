[<AutoOpen>]
module FSwag.Middleware

open System
open System.IO;
open FSharp.Control.Tasks.V2
open Microsoft.Extensions.Primitives
open Microsoft.AspNetCore.Http

let private setResponseHeaders (context: HttpContext) =
    context.Response.Headers.["Content-Type"] <- StringValues("text/html; charset=utf-8")
    context.Response.StatusCode <- 200

let private writeResponse (context: HttpContext) (documentStream: Stream) =
    task {
        do setResponseHeaders context
        use reader = new StreamReader(documentStream)
        let! document = reader.ReadToEndAsync()
        do! context.Response.WriteAsync(document)
    }

let private isMatch (urlPath: PathString) document =
    urlPath.HasValue && 
        String.Equals(urlPath.Value.TrimEnd('/'), document, StringComparison.OrdinalIgnoreCase)

type ReDocMiddleware (next: RequestDelegate) =
    member this.Invoke(ctx: HttpContext) =
        task {
            if isMatch ctx.Request.Path "/redoc" then
                let redocStream = this.GetType().Assembly.GetManifestResourceStream("FSwag.Res.Redoc.index.html");
                do! writeResponse ctx redocStream
            else
                next.Invoke ctx |> ignore
        }

type SwaggerMiddleware (next: RequestDelegate) =
    member this.Invoke(ctx: HttpContext) =
        task {
            if isMatch ctx.Request.Path "/swagger" then
                let swaggerStream = this.GetType().Assembly.GetManifestResourceStream("FSwag.Res.Swagger.index.html");
                do! writeResponse ctx swaggerStream
            else
                next.Invoke ctx |> ignore
        }