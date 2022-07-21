using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using api.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IDynamoDBContext _dbContext;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IDynamoDBContext dbContext,
        ILogger<WeatherForecastController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("ByDate")]
    public async Task<IActionResult> GetByDate(string city, string startingDate, string endingDate)
    {
        _logger.LogInformation($"search forecast by city: {city} and date from {startingDate} to {endingDate}");
        var forecast = await _dbContext
        .QueryAsync<WeatherForecast>(
            city,
            QueryOperator.Between, 
            new object[] { startingDate, endingDate }
        )
        .GetRemainingAsync();
        return Ok(new { forecast = forecast});
    }
        //     return await _dbContext
    //         .QueryAsync<WeatherForecast>(
    //             city,
    //             Amazon.DynamoDBv2.DocumentModel.QueryOperator.Between, 
    //             new object[] { DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(5)}
    //         )
    //         .GetRemainingAsync();    

    [HttpGet("ByCity")]
    public async Task<IActionResult> GetByCity(string city)
    {
        // if (String.IsNullOrEmpty(city)) BadRequest($"city name is required");
        _logger.LogInformation($"search forecast by city: {city}");
        var forecast = await _dbContext.QueryAsync<WeatherForecast>(city.Trim())
                .GetRemainingAsync(); 
        return Ok(new { forecast = forecast });
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] WeatherForecast data)
    {
        if (!ModelState.IsValid) return BadRequest($"invalid data input {data}");
        var forecast = await GetForecastByCityAndDate(data.City, data.Date); 
        if ( forecast.Count()> 0)  
            return BadRequest($"forecast {data.City} {data.Date} already exists "); 

        _logger.LogInformation($"add forecast for {data}");
        await _dbContext.SaveAsync(data);
        return CreatedAtAction(nameof(GetByCity), new { city = data.City });
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] WeatherForecast data) {
        if (!ModelState.IsValid) return BadRequest($"invalid data input {data}");
        var forecast = await GetForecastByCityAndDate(data.City, data.Date);

        if (forecast.Count() == 0)  
            return BadRequest($"forecast {data.City} {data.Date} does not exist");

        await _dbContext.DeleteAsync<WeatherForecast>(forecast.First());
        // var forecast = await _dbContext.LoadAsync<WeatherForecast>()
        await _dbContext.SaveAsync<WeatherForecast>(data);
        return Ok(new {city = data.City}); 
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string city, string date) {
        // if (city.IsEmpty() || date.IsEmpty()) 
        //     return BadRequest($"either city or date parameters is empty");
        var forecast = await GetForecastByCityAndDate(city, date);
        if (forecast.Count() == 0)
            return BadRequest($"forecast {city} {date} does not exist");
        await _dbContext.DeleteAsync<WeatherForecast>(forecast.First());
        _logger.LogInformation($"forecast for city {city} and date {date} is deleted");
        return NoContent(); 
    }

    private async Task<List<WeatherForecast>> GetForecastByCityAndDate(string city, string date) 
        => await _dbContext
            .QueryAsync<WeatherForecast>(city, 
            QueryOperator.Equal, new object[] {date}).GetRemainingAsync();    
}
