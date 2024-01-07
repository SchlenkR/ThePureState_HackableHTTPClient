
#r "nuget: FsHttp"

open System

open FsHttp
open FsHttp.Operators


% http {
    GET "http://localhost:5000/cities"
}

% http {
    GET "http://localhost:5000/cities/frankfurt/historicalWeather"
}

% http {
    PUT "http://localhost:5000/cities/frankfurt/currentConditions"
    body
    jsonSerialize
        {|
            temperature = 20.0
            humidity = 0.5
        |}
}




// Task 1 - what was the biggest temperature difference in one year?
let frankfurtHistoricalWeather =
    http {
        GET "http://localhost:5000/cities/frankfurt/historicalWeather"
    }
    |> Request.send
    |> Response.deserializeJson<
        list<
            {|
                tempMin: float
                tempMax: float
                time: DateOnly
            |}
            >
        >

let yearWithMaxTempDiff =
    frankfurtHistoricalWeather
    |> List.groupBy (fun x -> x.time.Year)
    |> List.map (fun (year, values) ->
        let min = values |> List.minBy (fun x -> x.tempMin)
        let max = values |> List.maxBy (fun x -> x.tempMax)
        let diff = max.tempMax - min.tempMin
        {| year = year; diff = diff |}
    )
    |> List.maxBy _.diff



// ------------------
// Auth Tokens
// ------------------


#load "./shared/vault.fsx"
#load "./shared/jwt.fsx"


let token = Jwt.encode(Vault.local.secKey, Vault.local.issuer, "Ronald", [ "admin" ])

http {
    GET $"http://localhost:5000/cities/{cities[0].name}/weather"
    AuthorizationBearer token
}
|> Request.send



let configureEnv baseUrl issuer secKey =
    http {
        config_transformHeader (fun (header: Header) ->
            let address = baseUrl </> header.target.address.Value
            { header with target.address = Some address }
        )

        config_ignoreCertIssues
        config_timeout (TimeSpan.FromMinutes 5.0)
        AuthorizationBearer (Jwt.encode(secKey, issuer, "TestUser", [ "admin" ]))
    }

let httpLoc = configureEnv $"http://localhost:5000" Vault.issuer Vault.secKey
let httpProd = configureEnv $"http://localhost:6000" Vault.issuer Vault.secKey


httpLoc {
    GET "cities"
}

