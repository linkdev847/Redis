using Newtonsoft.Json;
using RedisCashing.Services;
using System;
using System.Diagnostics;

namespace Redis.ConsoleApp
{
    class Program
    {

        static async Task Main(string[] args)
        {
            //TestRedis();

            double latitude = 39.7456;
            double longitude = -97.0891;

            string json;
            var watch = Stopwatch.StartNew();
            var keyName = $"forecast:{latitude},{longitude}";


            var _redis = new Rediservice().redis;

            json = await _redis.StringGetAsync(keyName);


            if (string.IsNullOrEmpty(json))
            {
                var forecastService = new ForecastService();
                json = await forecastService.GetForecast(latitude, longitude);
                var setTask = _redis.StringSetAsync(keyName, json);
                var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));
                await Task.WhenAll(setTask, expireTask);
            }
            else
            {
                Console.WriteLine("get from Redis Cash");

            }
            var forecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecast>>(json);

            watch.Stop();

            Console.WriteLine(json);
            Console.WriteLine(watch.ElapsedMilliseconds);

        }

        private static void TestRedis()
        {
            var _redis = new Rediservice().redis;

            //Simple Ping command to check if Database is Up, if database is up response will be pong
            var commandresponse = _redis.Execute("PING");
            Console.WriteLine(commandresponse.ToString());

            //Set a Value in Cache
            _redis.StringSet("TestConsole", "Hello from Console App, how are you doing");

            //Read recently setted Value From Cache
            var cachedresponse = _redis.StringGet("TestConsole");
            Console.WriteLine(cachedresponse.ToString());

            //Add an Object to Redis
            Employee e1 = new Employee() { Id = 1, Name = "Console Employee" };
            var jsonString = JsonConvert.SerializeObject(e1);
            _redis.StringSet("e1", jsonString);

            //Read An Object From redis
            cachedresponse = _redis.StringGet("e1");
            var employee = JsonConvert.DeserializeObject<Employee>(cachedresponse.ToString());
            Console.WriteLine($"Employee Id from Cache: {employee.Id}");
            Console.WriteLine($"Employee Name from Cache: {employee.Name}");

            Console.WriteLine("Press Any Key to exit....");
            Console.ReadLine();
        }

        public class Employee
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
}