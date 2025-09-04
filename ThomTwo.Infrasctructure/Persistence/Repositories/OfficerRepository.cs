using Gremlin.Net.Driver;
using Newtonsoft.Json.Linq;
using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;

namespace ThomTwo.Infrasctructure.Persistence.Repositories;

public class OfficerRepository : IOfficerRepository
{
    private readonly GremlinClient _gremlinClient;
    private const string PersonLabel = "person"; // Define the vertex label

    public OfficerRepository(GremlinClient gremlinClient)
    {
        _gremlinClient = gremlinClient ?? throw new ArgumentNullException(nameof(gremlinClient));
    }

    public async Task<Officer> GetByIdAsync(string id)
    {
        var query = $"g.V('{id}')";
        var resultSet = await _gremlinClient.SubmitAsync<dynamic>(query);

        foreach (var result in resultSet)
        {
            JObject jsonObject = JObject.FromObject(result);
            Officer entity = jsonObject.ToObject<Officer>();
            return entity;
        }

        return null;
    }


    public async Task<IEnumerable<Officer>> GetAllAsync()
    {
        var query = $"g.V().hasLabel('{PersonLabel}')";
        var results = await _gremlinClient.SubmitAsync<dynamic>(query);
        List<Officer> entities = new List<Officer>();

        foreach (var result in results)
        {
            JObject jsonObject = JObject.FromObject(result);
            Officer entity = jsonObject.ToObject<Officer>();
            entities.Add(entity);
        }

        return entities;
    }

    public async Task AddAsync(Officer person)
    {
        var properties = person.GetType().GetProperties();
        string query = $"g.addV('{PersonLabel}')";

        foreach (var property in properties)
        {
            if (property.Name == "Id") continue; // Skip Id as it might be generated
            var value = property.GetValue(person);
            if (value != null)
            {
                query += $".property('{property.Name}', '{value}')";
            }
        }
        query += $".property('id', '{person.Id}')"; // Add id property

        await _gremlinClient.SubmitAsync(query);
    }

    public async Task UpdateAsync(Officer person)
    {
        var properties = person.GetType().GetProperties();
        string id = person.Id;

        string query = $"g.V('{id}')";

        foreach (var property in properties)
        {
            if (property.Name == "Id") continue;
            var value = property.GetValue(person);
            if (value != null)
            {
                query += $".property('{property.Name}', '{value}')";
            }
        }

        await _gremlinClient.SubmitAsync(query);
    }

    public async Task DeleteAsync(string id)
    {
        var query = $"g.V('{id}').drop()";
        await _gremlinClient.SubmitAsync(query);
    }
}