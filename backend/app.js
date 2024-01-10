import express, { json } from 'express';
import { expressjwt as jwt } from 'express-jwt';
import { vault } from './vault.js';
import * as Data from './data.js';
import * as WeatherData from './weatherData.js';

const app = express();

// ----------------------------
// Setup
//   - Authentication via issuer-validation of JWT tokens
//   - Authorization via claims (ClaimTypes.Role) in JWT tokens
//   - Token utilities are located in the shared project.
//   - Secrets are abstracted away in the Vault.
// ----------------------------

const requireRoles = requiredRoles => (req, res, next) => {
  const rawRoles = req.auth?.role ?? [];
  const givenRoles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
  const missingRoles = requiredRoles.filter(role => !givenRoles.includes(role));
  if (missingRoles.length > 0) return res.sendStatus(401);
  next();
};

app.use(
  json(),
  jwt({
    secret: vault.getSecKey(),
    issuer: vault.getIssuer(),
    algorithms: ['HS256'],
    credentialsRequired: false,
  })
);

const wellKnownRoles = {
  admin: 'admin',
};

// ----------------------------
// API
//
// Data: For simplicity of this demo case, we store
//   - data in memory,
//   - with natural keys.
//
// Auth:
//   - Roles might be required for some endpoints.
// ----------------------------

app.get('/cities', 
  (_req, res) =>
    {
      return res.json(Data.allCities);
    }
);

app.get('/cities/:cityName',
  (req, res) => {
    const city = Data.allCities.find(x => x.name === req.params.cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    res.json(city);
  }
);

app.get('/cities/:cityName/historicalWeather', 
  (req, res) => {
    const city = WeatherData.weather.find(x => x.city === req.params.cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    res.json(city.series);
});

app.get('/cities/:cityName/currentConditions',
  (req, res) => {
    const city = Data.allCities.find(x => x.name === req.params.cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    res.json(city.currentConditions);
  }
);

app.put('/cities/:cityName/currentConditions',
  (req, res) => {
    const city = Data.allCities.find(x => x.name === req.params.cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    const newConditions = req.body;
    city.currentConditions = newConditions;

    res.json(city);
  }
);

app.delete('/cities/:cityName',
  requireRoles([wellKnownRoles.admin]),
  (req, res) => {
    const city = Data.allCities.find(x => x.name === req.params.cityName);
    if (!city) {
      res.sendStatus(404);
      return;
    }

    Data.updateCities(Data.allCities.filter(x => x.name !== req.params.cityName));
    
    res.sendStatus(204);
  }
);

// ----------------------------
// Start the server ...
// ----------------------------

[5000, 6000].forEach(port => {
  app.listen(port, () => console.log(`Server is running on port ${port}`));
});
