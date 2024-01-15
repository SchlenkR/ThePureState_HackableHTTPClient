#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------

% http {
    GET "http://localhost:5000/cities"
    CacheControl "no-cache"
}































type WeatherSample =
    {
        tempMin: float
        tempMax: float
        time: DateOnly
    }

let historicalWeather (cityName: string) =
    % http {
        GET $"http://localhost:5000/cities/{cityName}/historicalWeather"
    }
    |> Response.deserializeJson<list<WeatherSample>>

let yearWithBiggestTempDiff series =
    series
    |> List.groupBy (fun x -> x.time.Year)
    |> List.map (fun (year, samples) ->
        let min = samples |> List.minBy (fun x -> x.tempMin)
        let max = samples |> List.maxBy (fun x -> x.)
        let diff = max.tempMax - min.tempMin
        {| year = year; diff = diff |}
    )
    |> List.maxBy (_.diff)

historicalWeather "frankfurt" |> yearWithBiggestTempDiff
