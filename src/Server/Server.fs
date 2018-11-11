module Server
    open System
    open System.IO
    open System.Threading.Tasks

    open Microsoft.AspNetCore
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.DependencyInjection

    open FSharp.Control.Tasks.V2.ContextInsensitive

    open Giraffe
    open Giraffe.Serialization

    open Reaction.Giraffe.Middleware
    open Reaction
    open Reaction.AsyncObservable

    open Shared

    let publicPath = Path.GetFullPath "../Client/public"
    let port = 8085us

    let getInitCounter() : Task<Counter> = task { return 42 }

    let webApp = router {
        get "/api/init" (fun next ctx ->
            task {
                let! counter = getInitCounter()
                return! Successful.OK counter next ctx
            })
    }

    let configureSerialization (services:IServiceCollection) =
        let fableJsonSettings = Newtonsoft.Json.JsonSerializerSettings()
        fableJsonSettings.Converters.Add(Fable.JsonConverter())
        services.AddSingleton<IJsonSerializer>(NewtonsoftJsonSerializer fableJsonSettings)

    let app = application {
        url ("http://0.0.0.0:" + port.ToString() + "/")
        use_router webApp
        memory_cache
        use_static publicPath
        service_config configureSerialization
        use_gzip
    }

    run app
