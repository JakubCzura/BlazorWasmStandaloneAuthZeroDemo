using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.WeatherApi;

namespace WeatherApi.Controllers;

public class WeatherForecastController : ApiControllerBase
{
    private static readonly string[] _summaries = [ "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" ];

    private static readonly WeatherForecast[] _weatherForecasts =
        Enumerable.Range(1, 5)
                  .Select(index => new WeatherForecast()
                  {
                      Id = index,
                      Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                      TemperatureC = Random.Shared.Next(-20, 55),
                      Summary = _summaries[Random.Shared.Next(_summaries.Length)]
                  })
                  .ToArray();

    [HttpGet("predictions")]
    public IActionResult GetAll() => Ok(_weatherForecasts);

    [HttpGet("predictions/{id}")]
    [AllowAnonymous]
    public IActionResult GetById([FromRoute] int id) => Ok(_weatherForecasts.First(x => x.Id == id));
}