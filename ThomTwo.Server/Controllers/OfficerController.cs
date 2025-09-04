using Gremlin.Net.Driver;
using Microsoft.AspNetCore.Mvc;
using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;

namespace ThomTwo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OfficerController : ControllerBase
    {
        private readonly GremlinClient _client;
        private readonly IOfficerRepository _personRepository;

        public OfficerController(IOfficerRepository personRepository, GremlinClient gremlinClient)
        {
            _personRepository = personRepository;
               _client = gremlinClient;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        //private readonly ILogger<OfficerController> _logger;

        //public OfficerController(ILogger<OfficerController> logger, 
        //                                 GremlinClient gremlinClient)
        //{
        //    _logger = logger;
        //    _client = gremlinClient;
        //}

        [HttpGet()]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var person = await _personRepository.GetByIdAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Officer person)
        {
            await _personRepository.AddAsync(person);
            return CreatedAtAction(nameof(Get), new { id = person.Id }, person);
        }
    }
}
