namespace CityService.Shared

module Domain =
    type CityInfo = 
        {
            Name: string
            Country: string 
        }

    type WeatherInfo = 
        {
            Weather: string
            TempCelsius: float
        }

/// ISO3166-1 A-2 Country Codes
module WellKnownCountryCodes =
    let Germany = "DE"
    let France = "FR"
    let USA = "US"
    let Ukraine = "UA"
    let Italy = "IT"
