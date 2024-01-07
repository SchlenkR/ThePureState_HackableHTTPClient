const wellKnownCountryCodes = {
  germany: 'de',
  france: 'fr',
  gb: 'gb',
};


export let allCities = [
  {
    name: 'frankfurt',
    twoLetterIsoCountryCode: wellKnownCountryCodes.germany,
    currentConditions: {
      tempCelsius: 26.4,
      humidity: 0.1,
    },
    links: {
      historicalWeather: {
        href: '/cities/frankfurt/historicalWeather',
      },
    },
  },
  {
    name: 'paris',
    twoLetterIsoCountryCode: wellKnownCountryCodes.france,
    currentConditions: {
      tempCelsius: 29.2,
      humidity: 0.2,
    },
    links: {
      historicalWeather: {
        href: '/cities/paris/historicalWeather',
      },
    },
  },
  {
    name: 'london',
    twoLetterIsoCountryCode: wellKnownCountryCodes.gb,
    currentConditions: {
      tempCelsius: 22.0,
      humidity: 0.8,
    },
    links: {
      historicalWeather: {
        href: '/cities/london/historicalWeather',
      },
    },
  },
];

export const updateCities = newCities => {
  allCities = newCities;
};
