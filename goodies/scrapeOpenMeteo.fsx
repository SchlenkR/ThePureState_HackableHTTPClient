
#r "nuget: FsHttp"
#r "nuget: Vide.Common.DSP,0.0.34 "

open System
open System.IO
open System.Text.Json

open FsHttp

open Vide
open Vide.DSP

type DspBuilder with
    member this.For(m, f) = this.Bind(m |> Vide.ofSeq, f)

type City = { name: string; lat: string; long: string }

let cities =
    [
        { name = "Frankfurt"; lat = "50.1155"; long = "8.6842" }
        { name = "Paris"; lat = "48.8534"; long = "2.3488" }
        { name = "London"; lat = "51.4893"; long = "-0.1441" }
    ]

let getWeather city =
    http {
        GET $"https://archive-api.open-meteo.com/v1/archive
                ?latitude={city.lat}&longitude={city.long}
                &start_date=1980-01-01
                &end_date=2000-01-01
                &daily=temperature_2m_max,temperature_2m_min,temperature_2m_mean
                &timezone=Europe/Berlin"
    }
    |> Request.send
    |> Response.deserializeJson<
        {| 
            daily: {| 
                time: list<DateOnly>
                temperature_2m_max: list<float>
                temperature_2m_min: list<float>
                temperature_2m_mean: list<float>
            |}
        |}
        >

let dailyWeatherInfos =
    [
        for city in cities do
            let weather = getWeather city
            
            let series =
                dsp {
                    for time in weather.daily.time do
                    for tempMax in weather.daily.temperature_2m_max do
                    for tempMin in weather.daily.temperature_2m_min do
                    for tempMean in weather.daily.temperature_2m_mean do
                    return {|
                        time = time
                        tempMax = tempMax
                        tempMin = tempMin
                        tempMean = tempMean
                    |}
                }
                |> Gen.Eval.toSeq id
                |> Seq.take weather.daily.time.Length
                |> Seq.toList
            
            {|
                city = city.name
                series = series
            |}
    ]

let json = 
    let json = JsonSerializer.Serialize(dailyWeatherInfos, JsonSerializerOptions(WriteIndented = true))
    $"export const weather = {json}"
File.WriteAllText(__SOURCE_DIRECTORY__ + "/../backend/node/weatherData.js", json)
