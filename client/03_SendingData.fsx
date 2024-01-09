#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------


type CurrentConditions =
    {
        tempCelsius: float
        humidity: float
    }

let getCurrentConditions (cityName: string) =
    % http {
        GET $"http://localhost:5000/cities/{cityName}/currentConditions"
    }
    |> Response.deserializeJson<CurrentConditions>

let setCurrentConditions (cityName: string) (currentConditions: CurrentConditions) =
    % http {
        PUT $"http://localhost:5000/cities/{cityName}/currentConditions"
        body
        jsonSerialize currentConditions
    }

let updateTemperature (cityName: string) (newTemp: float) =
    let currCond = getCurrentConditions cityName
    let newCurrCond = { currCond with tempCelsius = newTemp }
    setCurrentConditions cityName newCurrCond

// Update temp of Frankfurt
updateTemperature "frankfurt" 25.0
