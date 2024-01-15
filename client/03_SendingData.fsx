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

let setCurrentConditions (cityName: string) (currentConditions: CurrentConditions) =
    % http {
        PUT $"http://localhost:5000/cities/{cityName}/currentConditions"
        body
        jsonSerialize currentConditions
    }

setCurrentConditions "frankfurt" { tempCelsius = 10.0; humidity = 0.8 }
