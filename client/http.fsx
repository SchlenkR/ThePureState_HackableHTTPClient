
#r "nuget: FsHttp"

open System
open FsHttp

let cities =
    http {
        GET "http://localhost:5000/cities"
    }
    |> Request.send
    |> Response.deserializeJson<list<{| name: string; twoLetterIsoCountryCode: string |}>>

let germanCities =
    cities
    |> List.map (fun x -> x.twoLetterIsoCountryCode = "DE")


#load "vault.fsx"
#load "jwt.fsx"


let token = Jwt.encode(Vault.secKey, Vault.issuer, "Ronald", [ "admin" ])
let decodedToken = Jwt.decode(Vault.secKey, Vault.issuer, token)

http {
    GET $"http://localhost:5000/cities/{cities[0].name}/weather"
    AuthorizationBearer token
}
|> Request.send


let weather =
    [
        for city in germanCities do
            http {
                GET $"http://localhost:5000/weather/{city.name}"
            }
    ]


open FsHttp.Operators


let configureEnv urlPrefix httpMethod route =
    let url = urlPrefix </> route
    http {
        Method httpMethod url
        config_ignoreCertIssues
        config_timeout (TimeSpan.FromMinutes 5.0)
        AuthorizationBearer (Token.encode(Vault.secKey, Vault.issuer, "Ronald", [Roles.Premium]))
    }

module Env =
    let dev = configureEnv $"http://localhost:5000"
    let staging = configureEnv $"http://localhost:6000"


Env.dev GET "cities" {
    GET "cities"
    config_timeout (TimeSpan.FromMinutes 5.0)
    AuthorizationBearer (Token.encode(Vault.secKey, Vault.issuer, "Ronald", [Roles.Premium]))
}

