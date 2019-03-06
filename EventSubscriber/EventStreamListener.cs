using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace EventSubscriber
{
    internal class EventStreamListener
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// The event store connection
        /// </summary>
        private IEventStoreConnection conn;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger<EventStreamListener> logger;
        public EventStreamListener(IConfiguration configuration, ILogger<EventStreamListener> logger)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        public void OnRun()
        {
            var stream = configuration.GetValue<string>("StreamName");
            var sub = conn.SubscribeToStreamAsync(stream, true,
                    (_, x) =>
                    {
                        var data = Encoding.ASCII.GetString(x.Event.Data);
                        this.logger.LogInformation("Received: " + x.Event.EventStreamId + ":" + x.Event.EventNumber);
                        this.logger.LogInformation(data);
                    });
        }

        public void OnStart()
        {
            var settings = ConnectionSettings.Create();
            var portNumber = configuration.GetValue<int>("EventStorePort");
            conn = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, portNumber));
            conn.ConnectAsync().Wait();
        }

        public void OnStop()
        {
            conn.Dispose();
        }

    }
}
