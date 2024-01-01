
#r "nuget: FsHttp"
#r "nuget: Vide.Common.DSP,0.0.34 "

open System
open System.Text.Json
open FsHttp

open Vide
open Vide.DSP


let weather =
    http {
        GET "https://archive-api.open-meteo.com/v1/archive
                ?latitude=50.1155&longitude=8.6842
                &start_date=1950-01-01
                &end_date=1950-01-05
                &daily=temperature_2m_max,temperature_2m_min,temperature_2m_mean
                &timezone=Europe%2FBerlin"
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


type DspBuilder with
    member this.For(m, f) = this.Bind(m |> Vide.ofSeq, f)

let dailyWeatherInfos =
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













