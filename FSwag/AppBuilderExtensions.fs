module FSwag.AppBuilderExtensions

open Middleware
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.FileProviders
open System.Reflection

type IApplicationBuilder with
    member this.UseSwaggerUI () =
        let fileProvider = EmbeddedFileProvider(typeof<SwaggerMiddleware>.GetTypeInfo().Assembly, "FSwag.Res.Swagger")
        let fileServerOptions =
            let options = FileServerOptions()
            options.FileProvider <- fileProvider
            options

        this.UseFileServer(fileServerOptions)
            .UseMiddleware<SwaggerMiddleware>()
            
    member this.UseReDoc =
        this.UseMiddleware<ReDocMiddleware>()