using Polly.Retry;
using Polly;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCashing.Services
{
    public class Rediservice
    {

        public IDatabase redis = GetDatabase();

        #region Connection
        //Connection
        //This approach to sharing a ConnectionMultiplexer instance
        //in your application uses a static property that returns a connected instance.
        //The code provides a thread-safe way to initialize only a single connected
        //ConnectionMultiplexer instance. abortConnect is set to false, which means that the
        //call succeeds even if a connection to the Azure Cache for Redis is not established.
        //One key feature of ConnectionMultiplexer is that it automatically restores connectivity
        //to the cache once the network issue or other causes are resolved.
        private static Lazy<ConnectionMultiplexer> lazyConnection = CreateConnection();

        public static ConnectionMultiplexer Connection
        {
            get { return lazyConnection.Value; }
        }

        private static Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("localhost:6379");//Provide your connecting string
            });
        }
        #endregion

        #region Retry Policy
        // Built using Polly
        private static RetryPolicy retryPolicy = Policy
                                                    .Handle<Exception>()
                                                    .WaitAndRetry(5, p =>
                                                    {
                                                        var timeToWait = TimeSpan.FromSeconds(90);
                                                        Console.WriteLine($"Waiting for reconnection {timeToWait}");
                                                        return timeToWait;
                                                    });

        #endregion

        //Get Redis Database 
        public static IDatabase GetDatabase()
        {
            return retryPolicy.Execute(() => Connection.GetDatabase());
        }

        //Get Redis Endpoint
        public static System.Net.EndPoint[] GetEndPoints()
        {
            return retryPolicy.Execute(() => Connection.GetEndPoints());
        }

        //Get Redis Server
        public static IServer GetServer(string host, int port)
        {
            return retryPolicy.Execute(() => Connection.GetServer(host, port));
        }

    }
}
