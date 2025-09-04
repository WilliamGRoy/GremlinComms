using Gremlin.Net.Driver;
using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;
using Gremlin.Net.Structure;
using System;
using Gremlin.Net.Process.Traversal;

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
        var query = "g.V(id).valueMap(true)";
        var bindings = new Dictionary<string, object> { { "id", id } };
        var resultSet = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query, bindings);

        var properties = resultSet.FirstOrDefault();
        return properties == null ? null : MapValueMapToOfficer(properties);
    }

    public async Task<IEnumerable<Officer>> GetAllAsync()
    {
        var query = "g.V().hasLabel(label).valueMap(true)";
        var bindings = new Dictionary<string, object> { { "label", PersonLabel } };
        var resultSet = await _gremlinClient.SubmitAsync<Dictionary<object, object>>(query, bindings);

        return resultSet.Select(MapValueMapToOfficer).ToList();
    }

    private static Officer MapValueMapToOfficer(IDictionary<object, object> properties)
    {
        var officer = new Officer();

        if (properties.TryGetValue(T.Id, out var idValue))
        {
            officer.Id = idValue.ToString();
        }

        if (properties.TryGetValue("Name", out var nameValue) && nameValue is ICollection<object> nameList && nameList.Any())
        {
            officer.Name = nameList.First().ToString();
        }

        if (properties.TryGetValue("Age", out var ageValue) && ageValue is ICollection<object> ageList && ageList.Any())
        {
            officer.Age = Convert.ToInt32(ageList.First());
        }

        return officer;
    }

    public async Task AddAsync(Officer person)
    {
        var query = "g.addV(label)" +
                    ".property(T.id, p_id)" +
                    ".property('Name', p_name)" +
                    ".property('Age', p_age)";

        var bindings = new Dictionary<string, object>
        {
            { "label", PersonLabel },
            { "p_id", person.Id },
            { "p_name", person.Name },
            { "p_age", person.Age }
        };

        await _gremlinClient.SubmitAsync(query, bindings);
    }

    public async Task UpdateAsync(Officer person)
    {
        var query = "g.V(id).property('Name', p_name).property('Age', p_age)";
        var bindings = new Dictionary<string, object>
        {
            { "id", person.Id },
            { "p_name", person.Name },
            { "p_age", person.Age }
        };

        await _gremlinClient.SubmitAsync(query, bindings);
    }

    public async Task DeleteAsync(string id)
    {
        var query = "g.V(id).drop()";
        var bindings = new Dictionary<string, object> { { "id", id } };
        await _gremlinClient.SubmitAsync(query, bindings);
    }
}