using Gremlin.Net.Driver;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThomTwo.Application.Features.Officers.Commands;
using ThomTwo.Application.Features.Officers.Queries;
using ThomTwo.Domain.Entities;

namespace ThomTwo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OfficerController : ControllerBase
    {
        private readonly GremlinClient _client;
        private readonly IMediator _mediator;

        public OfficerController(IMediator mediator, GremlinClient gremlinClient)
        {
            _mediator = mediator;
            _client = gremlinClient;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

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
                    { "prop_name", "Kiama classic surfboard5555 "},
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
            var person = await _mediator.Send(new GetOfficerByIdQuery { Id = id });
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Officer person)
        {
            var command = new CreateOfficerCommand
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age
            };
            var createdOfficer = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = createdOfficer.Id }, createdOfficer);
        }
    }
}
