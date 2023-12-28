namespace CityService.Shared

module Result =
    let ofError error = Error error
    let ofOk value = Ok value
