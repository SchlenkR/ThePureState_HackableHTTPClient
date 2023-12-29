import express, { json } from 'express';
import { expressjwt } from 'express-jwt';
import { vault } from './vault.js';

const app = express();

// ----------------------------
// Setup the technical infrastructure.
//   - Authentication via issuer-validation of JWT tokens
//   - Authorization via claims (ClaimTypes.Role) in JWT tokens
//   - Token utilities are located in the shared project.
//   - Secrets are abstracted away in the Vault.
// ----------------------------

const requireRoles = requiredRoles => (req, res, next) => {
  const rawRoles = req.auth.role ?? [];
  const givenRoles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
  const missingRoles = requiredRoles.filter(role => !givenRoles.includes(role));
  if (missingRoles.length > 0) return res.sendStatus(401);
  next();
};

app.use(json());

app.use(
  expressjwt({
    secret: vault.getSecKey(),
    issuer: vault.getIssuer(),
    algorithms: ['HS256'],
    credentialsRequired: false,
  })
);

// ----------------------------
// We store data in memory,
// with natural keys,
// using with anonymous types,
// for simplicity of this demo case.
// ----------------------------

const wellKnownCountryCodes = {
  germany: 'DE',
  france: 'FR',
  usa: 'US',
  ukraine: 'UA',
  italy: 'IT',
};

const weatherConditions = {
  sunny: 'Sunny',
  cloudy: 'Cloudy',
  rainy: 'Rainy',
};

const cities = [
  {
    name: 'Frankfurt',
    twoLetterIsoCountryCode: wellKnownCountryCodes.germany,
    weather: weatherConditions.sunny,
    tempCelsius: 27.4,
  },
  {
    name: 'Paris',
    twoLetterIsoCountryCode: wellKnownCountryCodes.france,
    weather: weatherConditions.cloudy,
    tempCelsius: 21.2,
  },
  {
    name: 'Boston',
    twoLetterIsoCountryCode: wellKnownCountryCodes.usa,
    weather: weatherConditions.rainy,
    tempCelsius: 32.0,
  },
  {
    name: 'Kyiv',
    twoLetterIsoCountryCode: wellKnownCountryCodes.ukraine,
    weather: weatherConditions.sunny,
    tempCelsius: 30.1,
  },
  {
    name: 'Rome',
    twoLetterIsoCountryCode: wellKnownCountryCodes.italy,
    weather: weatherConditions.cloudy,
    tempCelsius: 38.4,
  },
  {
    name: 'Offenbach',
    twoLetterIsoCountryCode: wellKnownCountryCodes.germany,
    weather: weatherConditions.rainy,
    tempCelsius: 26.8,
  },
];

const wellKnownRoles = {
  admin: 'admin',
};

// ----------------------------
// Define the API endpoints.
//   - API types are defined in the shared project.
//   - Authorization is required for some endpoints.
// ----------------------------

app.get('/cities', (req, res) => {
  const cityInfo = cities.map(city => ({
    name: city.name,
    twoLetterIsoCountryCode: city.twoLetterIsoCountryCode,
  }));
  res.json(cityInfo);
});

app.get(
  '/cities/:cityName/weather',
  requireRoles([wellKnownRoles.admin]),
  (req, res) => {
    const cityName = req.params.cityName;
    const city = cities.find(city => city.name === cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }
    const weatherInfo = {
      weather: city.weather,
      tempCelsius: city.tempCelsius,
    };
    res.json(weatherInfo);
  }
);

app.put(
  '/cities/:cityName/weather',
  requireRoles([wellKnownRoles.admin]),
  (req, res) => {
    const cityName = req.params.cityName;
    const weatherInfo = req.body;
    const city = cities.find(city => city.name === cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    cities = cities.filter(city => city.name !== cityName);
    cities.push({
      name: cityName,
      twoLetterIsoCountryCode: city.twoLetterIsoCountryCode,
      weather: weatherInfo.weather,
      tempCelsius: weatherInfo.tempCelsius,
    });
    res.sendStatus(204);
  }
);

const listen = port => {
  app.listen(port, () => console.log(`Server is running on port ${port}`));
};

listen(5000);
listen(6000);
