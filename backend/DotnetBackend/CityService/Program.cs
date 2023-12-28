using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using CityService.Shared;
using Microsoft.AspNetCore.Mvc;

// ----------------------------
// Setup the technical infrastructure.
//   - Authentication via issuer-validation of JWT tokens
//   - Authorization via claims (ClaimTypes.Role) in JWT tokens
//   - Token utilities are located in the shared project.
//   - Secrets are abstracted away in the Vault.
// ----------------------------

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        JwtBearerDefaults.AuthenticationScheme,
        options =>
            options.TokenValidationParameters =
                Token.getTokenValidationParams(Vault.SecKey, Vault.Issuer));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication().UseAuthorization();


// ----------------------------
// We store data in memory, 
// with natural keys,
// using with anonymous types,
// for simplicity of this demo case.
// ----------------------------

var cities = new[]
{
    new { Name = "Frankfurt", IsoTwoLetterCountryCode = WellKnownCountryCodes.Germany, Weather = "sunny", TempCelsius = 27.4 },
    new { Name = "Paris", IsoTwoLetterCountryCode = WellKnownCountryCodes.France, Weather = "cloudy", TempCelsius = 21.2 },
    new { Name = "Boston", IsoTwoLetterCountryCode = WellKnownCountryCodes.USA, Weather = "rainy", TempCelsius = 32.0 },
    new { Name = "Kyiv", IsoTwoLetterCountryCode = WellKnownCountryCodes.Ukraine, Weather = "sunny", TempCelsius = 30.1 },
    new { Name = "Rome", IsoTwoLetterCountryCode = WellKnownCountryCodes.Italy, Weather = "cloudy", TempCelsius = 38.4 },
    new { Name = "Offenbach", IsoTwoLetterCountryCode = WellKnownCountryCodes.Germany, Weather = "rainy", TempCelsius = 26.8 },
};


// ----------------------------
// Define the API endpoints.
//   - API types are defined in the shared project.
//   - Authorization is required for some endpoints.
// ----------------------------

app
    .MapGet(
        "/cities",
        () => cities
            .Select(it => new Domain.CityInfo(it.Name, it.IsoTwoLetterCountryCode))
            .ToArray());

app
    .MapGet(
        "/cities/{cityName}/weather",
        [Authorize(Roles = Roles.Premium)] (
            string cityName) =>
        {
            // TODO: Early returns (i.e. statements) prevent useful refactorings, here remove redundant code.
            var city = cities.SingleOrDefault(it => it.Name == cityName);
            if (city is null)
                return Results.NotFound();

            return Results.Ok(
                new Domain.WeatherInfo(city.Weather, city.TempCelsius));
        });


app
    .MapPut(
        "/cities/{cityName}/weather",
        [Authorize(Roles = Roles.Premium)] (
            string cityName,
            [FromBody] Domain.WeatherInfo weatherInfo) =>
        {
            // TODO: Early returns (i.e. statements) prevent useful refactorings, here remove redundant code.
            var city = cities.SingleOrDefault(it => it.Name == cityName);
            if (city is null)
                return Results.NotFound();

            cities = cities
                .Where(it => it.Name != cityName)
                .Append(new
                {
                    Name = cityName,
                    IsoTwoLetterCountryCode = city.IsoTwoLetterCountryCode,
                    Weather = weatherInfo.Weather,
                    TempCelsius = weatherInfo.TempCelsius
                })
                .ToArray();

            return Results.NoContent();
        });

app.Run();
