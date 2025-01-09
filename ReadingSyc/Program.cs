// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using RedisCashing.Services;
using System.Diagnostics;

Console.WriteLine("Hello, World!");


double latitude = 39.7456;
double longitude = -97.0891;

string json;
var watch = Stopwatch.StartNew();
var keyName = $"forecast:{latitude},{longitude}";


var _redis = new Rediservice().redis;

json = await _redis.StringGetAsync(keyName);


if (string.IsNullOrEmpty(json))
{
    //var forecastService = new ForecastService();
    //json = await forecastService.GetForecast(latitude, longitude);
    //var setTask = _redis.StringSetAsync(keyName, json);
    //var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
    //await Task.WhenAll(setTask, expireTask);
    Console.WriteLine("keyName not founs in Redis Cash");


}
else
{
    Console.WriteLine("get from Redis Cash");

}
var forecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(json);

watch.Stop();

Console.WriteLine(json);
Console.WriteLine(watch.ElapsedMilliseconds);