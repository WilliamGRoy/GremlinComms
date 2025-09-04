using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ThomTwo.Infrasctructure.Gremlin
{
    public class GremlinQueryExecutor : IGremlinQueryExecutor
    {
        private readonly GremlinClient _gremlinClient;
        private readonly ILogger<GremlinQueryExecutor> _logger;

        public GremlinQueryExecutor(GremlinClient gremlinClient, ILogger<GremlinQueryExecutor> logger)
        {
            _gremlinClient = gremlinClient ?? throw new ArgumentNullException(nameof(gremlinClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ResultSet<T>> ExecuteAsync<T>(GremlinQuery query)
        {
            try
            {
                // Create a defensive copy of the arguments to prevent any potential modification by the client.
                var resultSet = await _gremlinClient.SubmitAsync<T>(query.Query, new Dictionary<string, object>(query.Arguments));
                LogStatusAttributes(resultSet.StatusAttributes, query);
                return resultSet;
            }
            catch (ResponseException e)
            {
                _logger.LogError(e, "Gremlin query failed. Query: {Query}", query.Query);
                LogStatusAttributes(e.StatusAttributes, query, isError: true);
                throw; // Re-throw the exception to be handled by higher layers
            }
        }

        private void LogStatusAttributes(IReadOnlyDictionary<string, object> attributes, GremlinQuery query, bool isError = false)
        {
            if (attributes == null) return;

            var statusCode = GetValueAsString(attributes, "x-ms-status-code");
            var totalTime = GetValueAsString(attributes, "x-ms-total-server-time-ms");
            var requestCharge = GetValueAsString(attributes, "x-ms-total-request-charge");
            
            if (isError)
            {
                var activityId = GetValueAsString(attributes, "x-ms-activity-id");
                var retryAfter = GetValueAsString(attributes, "x-ms-retry-after-ms");
                _logger.LogError(
                    "Gremlin Error Status - Query: {Query} | StatusCode: {StatusCode} | TotalTime: {TotalTime}ms | RequestCharge: {RequestCharge}RU | ActivityId: {ActivityId} | RetryAfter: {RetryAfter}ms",
                    query.Query, statusCode, totalTime, requestCharge, activityId, retryAfter);
            }
            else
            {
                _logger.LogInformation(
                    "Gremlin Query Status - Query: {Query} | StatusCode: {StatusCode} | TotalTime: {TotalTime}ms | RequestCharge: {RequestCharge}RU",
                    query.Query, statusCode, totalTime, requestCharge);
            }
        }

        private static string GetValueAsString(IReadOnlyDictionary<string, object> dictionary, string key) => JsonSerializer.Serialize(GetValueOrDefault(dictionary, key));

        private static object GetValueOrDefault(IReadOnlyDictionary<string, object> dictionary, string key) => dictionary.TryGetValue(key, out var value) ? value : null;
    }
}