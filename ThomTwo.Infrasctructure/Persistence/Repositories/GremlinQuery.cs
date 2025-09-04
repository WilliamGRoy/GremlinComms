namespace ThomTwo.Infrasctructure.Gremlin
{
    public class GremlinQuery
    {
        public string Query { get; }
        public IReadOnlyDictionary<string, object> Arguments { get; }

        public GremlinQuery(string query, IReadOnlyDictionary<string, object> arguments = null)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
            Arguments = arguments ?? new Dictionary<string, object>();
        }

        public override string ToString() => Query;
    }
}