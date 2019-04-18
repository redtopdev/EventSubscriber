using Engaze.Event.Subscriber.Service;
using Engaze.EventSubscriber.Service;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading.Tasks;

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

        private readonly UserCredentials User;

        private EventSubsriptionSetting settings;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger<EventStreamListener> logger;
        public EventStreamListener(IOptions<EventSubsriptionSetting> options, IConfiguration configuration, ILogger<EventStreamListener> logger, IMessageHandler messageHandler)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.messageHandler = messageHandler;
            this.settings = options.Value;
            this.User = new UserCredentials(settings.User.Name, settings.User.Password);
        }

        public async Task OnStart()
        {
            conn = EventStoreConnection.Create(new Uri(settings.ConnString));
            await conn.ConnectAsync();
            ConnectToSubscription();
        }

        public void OnStop()
        {
            conn.Dispose();
        }

        private void ConnectToSubscription()
        {
            var sub = conn.ConnectToPersistentSubscriptionAsync(settings.Stream, settings.SubscriptionGroup, EventAppeared, SubscriptionDropped,
                User, settings.Buffersize, settings.AutoAck);
        }

        private void SubscriptionDropped(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception ex)
        {
            ConnectToSubscription();
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, ResolvedEvent resolvedEvent)
        {
            try
            {
                var data = Encoding.ASCII.GetString(resolvedEvent.Event.Data);
                messageHandler.ProcessMessage(resolvedEvent.Event);
                this.logger.LogInformation("Received: " + resolvedEvent.Event.EventStreamId + ":" + resolvedEvent.Event.EventNumber);
                this.logger.LogInformation(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "", null);
            }

            return Task.CompletedTask;
        }
    }
}
