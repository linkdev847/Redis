using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace RedisCashing.Services
{
    public class ForecastService
    {
        public HttpClient _client { get; set; } = new HttpClient();


        public async Task<string> GetForecast(double latitude, double longitude)
        {
            var pointsRequestQuery = $"https://api.weather.gov/points/{latitude},{longitude}"; //get the URI
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("weatherCachingApp", "1.0"));
            var result = await _client.GetFromJsonAsync<JsonObject>(pointsRequestQuery);
            var gridX = result["properties"]["gridX"].ToString();
            var gridY = result["properties"]["gridY"].ToString();
            var gridId = result["Properties"]["gridId"].ToString();
            var forecastRequestQuery = $"https://api.weather.gov/gridpoints/{gridId}/{gridX},{gridY}/forecast";
            var forecastResult = await _client.GetFromJsonAsync<JsonObject>(forecastRequestQuery);
            var periodsJson = forecastResult["properties"]["periods"].ToJsonString();
            return periodsJson;
        }
    }
}
