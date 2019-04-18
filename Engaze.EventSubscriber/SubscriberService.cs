using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSubscriber
{
    class SubscriberService : IHostedService
    {

        /// <summary>
        /// The name of this service.  Exposed in the Windows Service UI.
        /// </summary>
        private string serviceName = "Engaze {0} Subscription Worker";

        /// <summary>
        /// The configuration
        /// </summary>
        private IConfiguration configuration;

        /// <summary>
        /// The logger
        /// </summary>
        private ILogger<SubscriberService> logger;

        /// <summary>
        /// The hosting environment
        /// </summary>
        private IHostingEnvironment hostingEnvironment;

        private EventStreamListener eventStreamListener;

        public SubscriberService(IConfiguration configuration, ILogger<SubscriberService> logger, IHostingEnvironment hostingEnvironment, EventStreamListener eventStreamListener)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
            this.eventStreamListener = eventStreamListener;
            this.hostingEnvironment.ApplicationName = string.Format(serviceName, configuration.GetValue<string>("TestName"));
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{this.hostingEnvironment.ApplicationName} is starting");
            try
            {
                await eventStreamListener.OnStart();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while running the Service");
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is stopping.");
            try
            {
                eventStreamListener.OnStop();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while stopping the Service");
            }

            return Task.CompletedTask;
        }
    }
}
