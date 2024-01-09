#r "nuget: FsHttp"

open System
open FsHttp


// ------------------------------
// Alternatives
// ------------------------------

open System.Collections.Generic
open System.Linq


type WeatherSample =
    {
        tempMin: float
        tempMax: float
        time: DateOnly
    }

let getHistoricalWeather (cityName: string) =
    Http
        .Get($"http://localhost:5000/cities/{cityName}/historicalWeather")
        .Send()
        .DeserializeJson<list<WeatherSample>>()

let getYearWithBiggestTempDiff (series: IEnumerable<WeatherSample>) =
    series
        .GroupBy(fun x -> x.time.Year)
        .Select(fun g ->
            let min = g.MinBy(fun x -> x.tempMin)
            let max = g.MaxBy(fun x -> x.tempMax)
            let diff = max.tempMax - min.tempMin
            {| year = g.Key; diff = diff |}
        )
        .MaxBy (fun x -> x.diff)


let tmp1 = getHistoricalWeather("frankfurt")
let res = getYearWithBiggestTempDiff(tmp1)
