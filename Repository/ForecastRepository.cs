using DuploCloudApi.Interface;
using DuploCloudApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace DuploCloudApi.Repository
{
    public class ForecastRepository : IForecastRepository
    {
        private readonly ForecastContext _context;

        public ForecastRepository(ForecastContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<Forecast?> AddForecast(ForecastDTO forecastDto)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/");
            // api call forecast data    
            var response = client.GetAsync("v1/forecast?latitude=" + forecastDto.Latitude + "&longitude=" + forecastDto.Longitude + "&current=temperature_2m,precipitation,rain,relative_humidity_2m,apparent_temperature,showers,snowfall,wind_speed_10m,wind_direction_10m,wind_gusts_10m").Result;
            if (response.IsSuccessStatusCode)
            {
                var forecastData = response.Content.ReadAsStringAsync().Result;

                if (forecastData != null)
                {
                    if (ForecastExists(forecastDto.Latitude, forecastDto.Longitude))
                    {
                        var existingForecast = await _context.Forecasts.SingleAsync(i => i.Latitude == forecastDto.Latitude & i.Longitude == forecastDto.Longitude);
                        _context.Entry(existingForecast).State = EntityState.Modified;

                        existingForecast.ForecastJsonData = forecastData;
                        existingForecast.Date = DateTime.Now;
                        await _context.SaveChangesAsync();
                        return existingForecast;                        
                    }
                    else
                    {
                        var forecast = new Forecast
                        {
                            Latitude = forecastDto.Latitude,
                            Longitude = forecastDto.Longitude,
                            ForecastJsonData = forecastData,
                            Date = DateTime.Now

                        };
                        _context.Forecasts.Add(forecast);
                        await _context.SaveChangesAsync();
                        return forecast;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {

                return null;
            }
        }

        public async Task<bool> DeleteForecast(long id)
        {
            var forecast = await _context.Forecasts.FindAsync(id);
            if (forecast == null)
            {
                return false;
            }

            _context.Forecasts.Remove(forecast);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<Forecast?> GetForecastById(long id)
        {
            var forecast = await _context.Forecasts.FindAsync(id);

            if (forecast == null)
            {
                return null;
            }

            return forecast;
        }

        public async Task<IList<Forecast>> GetForecasts()
        {
            var forecastList = await _context.Forecasts.ToListAsync();
            
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.open-meteo.com/");

            foreach (var item in forecastList)
            {
                // api call forecast for updated data  
                var response = client.GetAsync("v1/forecast?latitude=" + item.Latitude + "&longitude=" + item.Longitude + "&current=temperature_2m,precipitation,rain,relative_humidity_2m,apparent_temperature,showers,snowfall,wind_speed_10m,wind_direction_10m,wind_gusts_10m").Result;

                if (response.IsSuccessStatusCode)
                {
                    var forecastData = response.Content.ReadAsStringAsync().Result;
                    if (forecastData != null)
                    {
                        item.ForecastJsonData = forecastData;
                        item.Date = DateTime.Now;

                        _context.Entry(item).State = EntityState.Modified;

                        try
                        {
                            await _context.SaveChangesAsync();
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            throw;
                        }

                    }

                }
            }

            return forecastList;
        }

        public async Task<bool> UpdateForecast(long id, Forecast forecast)
        {
            if (id != forecast.Id)
            {
                return false;
            }

            _context.Entry(forecast).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForecastExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }
        private bool ForecastExists(long id)
        {
            return _context.Forecasts.Any(e => e.Id == id);
        }

        private bool ForecastExists(string latitude, string longitude)
        {
            return _context.Forecasts.Any(e => e.Latitude == latitude & e.Longitude == longitude);
        }

    }
}
