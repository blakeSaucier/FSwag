[<AutoOpen>]
module FSwag.AppBuilderExtensions

open Middleware
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.FileProviders
open System.Reflection
open FSharpx

type IApplicationBuilder with
    member this.UseSwaggerUI options =
        let fileProvider = EmbeddedFileProvider(typeof<SwaggerMiddleware>.GetTypeInfo().Assembly, "FSwag.Res.Swagger")
        let fileServerOptions =
            let options = FileServerOptions()
            options.FileProvider <- fileProvider
            options

        options
        |> Option.getOrElse defaultOptions
        |> this.UseFileServer(fileServerOptions)
            .UseMiddleware<SwaggerMiddleware>
            
    member this.UseReDoc options =
        options 
        |> Option.getOrElse defaultOptions
        |> this.UseMiddleware<ReDocMiddleware>

let useSwaggerUi options (app: IApplicationBuilder) = app.UseSwaggerUI options

let useReDoc options (app: IApplicationBuilder)  = app.UseReDoc options