#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------


% http {
    GET "http://localhost:5000/cities/frankfurt"
}


% http {
    PUT "http://localhost:5000/cities/frankfurt/currentConditions"
    body
    json $$"""
        {
            "tempCelsius": 8.0,
            "humidity": 0.5
        }
    """
}


// --------------------

type CurrentConditions = 
    {
        tempCelsius: float
        humidity: float
    }

let updateConditions (city: string) (updateConditions: CurrentConditions) =
    % http {
        PUT $"http://localhost:5000/cities/{city}/currentConditions"
        body
        jsonSerialize updateConditions
    }

updateConditions "frankfurt" { tempCelsius = 8.0; humidity = 0.5 }
