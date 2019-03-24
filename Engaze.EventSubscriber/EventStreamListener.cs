﻿using Engaze.Event.Subscriber.Service;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
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

        private IMessageHandler messageHandler;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger<EventStreamListener> logger;
        public EventStreamListener(IConfiguration configuration, ILogger<EventStreamListener> logger, IMessageHandler messageHandler)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.messageHandler = messageHandler;
        }

        public void OnRun()
        {
            //var stream = configuration.GetValue<string>("StreamName");
            var stream = "$ce-Evento";
            var sub = conn.SubscribeToStreamAsync(stream, true,
                    (_, x) =>
                    {
                        try
                        {
                            var data = Encoding.ASCII.GetString(x.Event.Data);
                            messageHandler.ProcessMessage(x.Event);
                            Console.WriteLine("Received: " + x.Event.EventStreamId + ":" + x.Event.EventNumber);
                            Console.WriteLine(data);
                           this.logger.LogInformation("Received: " + x.Event.EventStreamId + ":" + x.Event.EventNumber);
                            this.logger.LogInformation(data);
                        }
                        catch(Exception ex)
                        {
                            logger.LogError(ex, "", null);
                        }
                    });
        }

        public void OnStart()
        {
            var settings = ConnectionSettings.Create();
            //var portNumber = configuration.GetValue<int>("EventStorePort");
            //conn = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Loopback, portNumber));
            conn = EventStoreConnection.Create(new Uri("tcp:event-store:1113"));
            conn.ConnectAsync().Wait();
        }

        public void OnStop()
        {
            conn.Dispose();
        }

    }
}