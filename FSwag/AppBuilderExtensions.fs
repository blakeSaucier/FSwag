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

        let settings = Option.getOrElse defaultOptions options
        this.UseFileServer(fileServerOptions)
            .UseMiddleware<RedirectToSwaggerMiddleware>(settings)
            .UseMiddleware<SwaggerMiddleware>(settings)
            
    member this.UseReDoc options =
        let settings = Option.getOrElse defaultOptions options
        this.UseMiddleware<RedirectToSwaggerMiddleware>(settings)
            .UseMiddleware<ReDocMiddleware>(settings)

let useSwaggerUi options (app: IApplicationBuilder) = app.UseSwaggerUI options

let useReDoc options (app: IApplicationBuilder)  = app.UseReDoc options