#r "nuget: FsHttp"

open System
open System.Linq
open FsHttp

let cities =
    http {
        GET "https://localhost:5000/cities"
    }
    |> Request.send
    |> Response.toJsonArray
    |> Array.map (fun x -> 
        {|
            name = x?name.GetString()
            country = x?country.GetString()
        |})

let germanCities =
    cities
        .Where(fun x -> x.country = "germany")
        .ToArray()


#I "./publish/CityService.Shared"
#r "CityService.Shared.dll"
#r "nuget: Microsoft.IdentityModel.Tokens"

#load "vault.fsx"

open CityService.Shared


let token = Token.encode(Vault.secKey, Vault.issuer, "Ronald", [Roles.Premium])
let decodedToken = Token.decode(Vault.secKey, Vault.issuer, token)

http {
    GET $"https://localhost:5000/cities/{cities[0].name}/weather"
    AuthorizationBearer token
}
|> Request.send


let weather =
    [
        for city in germanCities do
            http {
                GET $"https://localhost:5000/weather/{city.name}"
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
    let dev = configureEnv $"https://localhost:5000"
    let staging = configureEnv $"https://localhost:6000"


Env.dev GET "cities" {
    GET "cities"
    config_timeout (TimeSpan.FromMinutes 5.0)
    AuthorizationBearer (Token.encode(Vault.secKey, Vault.issuer, "Ronald", [Roles.Premium]))
}

