[<AutoOpen>]
module FSwag.Types

type FSwagOptions =
    { DocumentLocation: string
      SwaggerUrl: string }

let defaultOptions =
    { DocumentLocation = "/swagger.json"
      SwaggerUrl = "/swagger" }