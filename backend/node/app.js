//   - Secrets are abstracted away in the Vault.
// ----------------------------

const express = require('express');
const app = express();

app.use(express.json());

// ----------------------------
// We store data in memory,
// with natural keys,
// using with anonymous types,
// for simplicity of this demo case.
// ----------------------------

const cities = [
  {
    Name: 'Frankfurt',
    IsoTwoLetterCountryCode: 'DE',
    Weather: 'sunny',
    TempCelsius: 27.4,
  },
  {
    Name: 'Paris',
    IsoTwoLetterCountryCode: 'FR',
    Weather: 'cloudy',
    TempCelsius: 21.2,
  },
  {
    Name: 'Boston',
    IsoTwoLetterCountryCode: 'US',
    Weather: 'rainy',
    TempCelsius: 32.0,
  },
  {
    Name: 'Kyiv',
    IsoTwoLetterCountryCode: 'UA',
    Weather: 'sunny',
    TempCelsius: 30.1,
  },
  {
    Name: 'Rome',
    IsoTwoLetterCountryCode: 'IT',
    Weather: 'cloudy',
    TempCelsius: 38.4,
  },
  {
    Name: 'Offenbach',
    IsoTwoLetterCountryCode: 'DE',
    Weather: 'rainy',
    TempCelsius: 26.8,
  },
];

// ----------------------------
// Define the API endpoints.
//   - API types are defined in the shared project.
//   - Authorization is required for some endpoints.
// ----------------------------

app.get('/cities', (req, res) => {
  const cityInfo = cities.map(city => ({
    Name: city.Name,
    IsoTwoLetterCountryCode: city.IsoTwoLetterCountryCode,
  }));
  res.json(cityInfo);
});

app.get('/cities/:cityName/weather', (req, res) => {
  const cityName = req.params.cityName;
  const city = cities.find(city => city.Name === cityName);
  if (!city) {
    res.sendStatus(404);
    return;
  }
  const weatherInfo = { Weather: city.Weather, TempCelsius: city.TempCelsius };
  res.json(weatherInfo);
});

app.put('/cities/:cityName/weather', (req, res) => {
  const cityName = req.params.cityName;
  const weatherInfo = req.body;
  const city = cities.find(city => city.Name === cityName);
  if (!city) {
    res.sendStatus(404);
    return;
  }
  cities = cities.filter(city => city.Name !== cityName);
  cities.push({
    Name: cityName,
    IsoTwoLetterCountryCode: city.IsoTwoLetterCountryCode,
    Weather: weatherInfo.Weather,
    TempCelsius: weatherInfo.TempCelsius,
  });
  res.sendStatus(204);
});

const listen = port => {
  app.listen(port, () => console.log('Server is running on port 3000'));
};

listen(5000);
listen(6000);
