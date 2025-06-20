using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DuploCloudApi.Models;
using DuploCloudApi.Interface;

namespace DuploCloudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForecastsController : ControllerBase
    {
        private readonly IForecastRepository _forecastRepository;

        public ForecastsController(IForecastRepository forecastRepository)
        {
            _forecastRepository = forecastRepository;
        }

        // GET: api/Forecasts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Forecast?>>> GetForecasts()
        {
            return new OkObjectResult(await _forecastRepository.GetForecasts());
           
        }

        // GET: api/Forecasts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Forecast>> GetForecast(long id)
        {
            var forecast = await _forecastRepository.GetForecastById(id);

            if (forecast == null)
            {
                return NotFound();
            }

           return forecast;
          
        }

        // PUT: api/Forecasts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForecast(long id, Forecast forecast)
        {
            var result = await _forecastRepository.UpdateForecast(id,forecast);
            if (result == false)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // POST: api/Forecasts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Forecast>> PostForecast(ForecastDTO forecastDto)
        {
            var forecast = await _forecastRepository.AddForecast(forecastDto);
            if (forecast == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetForecast), new { id = forecast.Id }, forecast);  
        }

        // DELETE: api/Forecasts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForecast(long id)
        {
            var forecast = await _forecastRepository.DeleteForecast(id);
            if (forecast == false)
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
