#r "nuget: FsHttp"

open System
open FsHttp
open FsHttp.Operators

// --------------------


% http {
    GET "http://localhost:5000/cities"
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

frankfurtHistoricalWeather
|> List.groupBy (fun x -> x.time.Year)
|> List.map (fun (year, values) ->
    let min = values |> List.minBy (fun x -> x.tempMin)
    let max = values |> List.maxBy (fun x -> x.tempMax)
    let diff = max.tempMax - min.tempMin
    {| year = year; diff = diff |}
)
|> List.maxBy _.diff

let maxTemperatureDifference =
    query {
        for weather in frankfurtHistoricalWeather do
        groupBy weather.time.Year into yearGroup
        let maxDiff = query {
            for weatherSample in yearGroup do
            let diff = weatherSample.tempMax - weatherSample.tempMin
            maxBy diff
        }
        let res = {| year = yearGroup.Key; diff = maxDiff |}
        sortByDescending res.diff
        head
    }
