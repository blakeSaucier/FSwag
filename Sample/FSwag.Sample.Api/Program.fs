module FopenApi.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open FSwag

let webApp =
    choose [
        route "/ping"    >=> text "pong"
        route "/"        >=> htmlFile "/pages/index.html" ]

let staticFileOptions =
    let staticFileOptions = StaticFileOptions()
    staticFileOptions.ServeUnknownFileTypes <- true
    staticFileOptions

let useGiraffe (app: IApplicationBuilder) =
    app.UseGiraffe webApp
    app

let useRedoc (app: IApplicationBuilder) =
    app.UseReDoc None

let useSwagger (app: IApplicationBuilder) =
    app.UseSwaggerUI None

let configureApp (app : IApplicationBuilder) =
    app.UseStaticFiles(staticFileOptions)
    |> useRedoc
    |> useGiraffe
    |> ignore

let configureServices (services : IServiceCollection) =
    services.AddGiraffe() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot = Path.Combine(contentRoot, "wwwroot")

    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .Build()
        .Run()
    0
