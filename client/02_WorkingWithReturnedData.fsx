#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------


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

frankfurtHistoricalWeather
|> List.groupBy (fun x -> x.time.Year)
|> List.map (fun (year, samples) ->
    let min = samples |> List.minBy (fun x -> x.tempMin)
    let max = samples |> List.maxBy (fun x -> x.tempMax)
    let diff = max.tempMax - min.tempMin
    {| year = year; diff = diff |}
)
|> List.maxBy (_.diff)
