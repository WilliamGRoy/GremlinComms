using Gremlin.Net.Driver;
using System.Threading.Tasks;

namespace ThomTwo.Infrasctructure.Gremlin
{
    public interface IGremlinQueryExecutor
    {
        Task<ResultSet<T>> ExecuteAsync<T>(GremlinQuery query);
    }
}