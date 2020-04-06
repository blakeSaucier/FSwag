# FSwag

Render Swagger Docs in F#

Getting started with Giraffe:

```f#
open FSwag

let useRedoc (app: IApplicationBuilder) =
    app.UseReDoc None // Default settings

let configureApp (app : IApplicationBuilder) =
    app.UseStaticFiles(staticFileOptions) // Needed for serving swagger docs
    |> useRedoc // Integrate into the IApplicationBuilder Config
    |> useGiraffe
    |> ignore

```

This setup assumes your `swagger.json` (or `swagger.yaml`) file is being served at the path `/swagger.json` on your web app.

Navigating to the root of your web application will send you to `/swagger-ui` - displaying the rendered swagger UI.

Note: you must have an existing swagger spec! This will not generate anything!
