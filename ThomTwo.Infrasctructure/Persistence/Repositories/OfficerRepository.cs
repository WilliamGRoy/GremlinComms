using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;
using Gremlin.Net.Structure;
using System;
using ThomTwo.Infrasctructure.Gremlin;
using Gremlin.Net.Process.Traversal;

namespace ThomTwo.Infrasctructure.Persistence.Repositories;

public class OfficerRepository : IOfficerRepository
{
    private readonly IGremlinQueryExecutor _gremlinExecutor;
    private const string PersonLabel = "person"; // Define the vertex label

    public OfficerRepository(IGremlinQueryExecutor gremlinExecutor)
    {
        _gremlinExecutor = gremlinExecutor ?? throw new ArgumentNullException(nameof(gremlinExecutor));
    }

    public async Task<Officer> GetByIdAsync(string id)
    {
        var gremlinQuery = new GremlinQuery("g.V(id).valueMap(true)",
            new Dictionary<string, object> { { "id", id } });
        var resultSet = await _gremlinExecutor.ExecuteAsync<Dictionary<object, object>>(gremlinQuery);

        var properties = resultSet.FirstOrDefault();
        return properties == null ? null : MapValueMapToOfficer(properties);
    }

    public async Task<IEnumerable<Officer>> GetAllAsync()
    {
        var gremlinQuery = new GremlinQuery("g.V().hasLabel(label).valueMap(true)",
            new Dictionary<string, object> { { "label", PersonLabel } });
        var resultSet = await _gremlinExecutor.ExecuteAsync<Dictionary<object, object>>(gremlinQuery);

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
        var queryText = "g.addV(label)" +
                    ".property(T.id, p_id)" +
                    ".property('pk', p_pk)" + // Add the partition key property
                    ".property('Name', p_name)" +
                    ".property('Age', p_age)";

        var parameters = new Dictionary<string, object>
        {
            { "label", PersonLabel },
            { "p_pk", PersonLabel }, // Use the label as the partition key value
            { "p_id", person.Id },
            { "p_name", person.Name },
            { "p_age", person.Age }
        };
        var gremlinQuery = new GremlinQuery(queryText, parameters);
        await _gremlinExecutor.ExecuteAsync<dynamic>(gremlinQuery);
    }

    public async Task UpdateAsync(Officer person)
    {
        var queryText = "g.V(id).property('Name', p_name).property('Age', p_age)";
        var parameters = new Dictionary<string, object>
        {
            { "id", person.Id },
            { "p_name", person.Name },
            { "p_age", person.Age }
        };
        var gremlinQuery = new GremlinQuery(queryText, parameters);
        await _gremlinExecutor.ExecuteAsync<dynamic>(gremlinQuery);
    }

    public async Task DeleteAsync(string id)
    {
        var gremlinQuery = new GremlinQuery("g.V(id).drop()",
            new Dictionary<string, object> { { "id", id } });
        await _gremlinExecutor.ExecuteAsync<dynamic>(gremlinQuery);
    }
}