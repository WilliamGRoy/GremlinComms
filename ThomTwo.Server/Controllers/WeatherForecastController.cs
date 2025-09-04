using Gremlin.Net.Driver;
using Microsoft.AspNetCore.Mvc;

namespace ThomTwo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly GremlinClient _client;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, 
                                         GremlinClient gremlinClient)
        {
            _logger = logger;
            _client = gremlinClient;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await _client.SubmitAsync(
                requestScript: "g.V().drop()"
                );

            await _client.SubmitAsync(
                requestScript: "g.addV('product').property('id', prop_id).property('name', prop_name).property('pk', pk)",
                bindings: new Dictionary<string, object>
                {
                    { "prop_id", "68719518373" },
                    { "prop_name", "Kiama classic surfboard44444" },
                    { "pk", "Kiama44444" }
                }
               );

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = 12,//Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
