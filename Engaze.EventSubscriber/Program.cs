using Engaze.Core.Common;
using Engaze.Core.MessageBroker.Producer;
using Engaze.Event.Subscriber.Service;
using Engaze.EventSubscriber.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EventSubscriber
{
    class Program
    {
        public static void Main(string[] args)
        {           
            var host = new HostBuilder().ConfigureHostConfiguration(configHost =>
             {
                 configHost.AddCommandLine(args);
                 configHost.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                 configHost.AddEnvironmentVariables();
             })
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                hostContext.HostingEnvironment.EnvironmentName = System.Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            })
             .ConfigureLogging((hostContext, configLogging) =>
             {
                 if (hostContext.HostingEnvironment.IsDevelopment())
                 {
                     configLogging.AddConsole();
                     configLogging.AddDebug();
                 }
             }).ConfigureServices((hostContext, services) =>
             {
                 services.Configure<EventSubsriptionConfiguration>(hostContext.Configuration.GetSection("EventSubsriptionConfiguration"));
                 services.ConfigureProducerService(hostContext.Configuration);
                 services.AddLogging();
                 services.AddSingleton<KafkaConfiguration>();                
                 services.AddSingleton<EventStreamListener>();
                 services.AddSingleton(typeof(IEventMessageHandler), typeof(EventMessageHandler));
                 services.AddHostedService<SubscriberService>();                
             }).Build();
            host.Run();
        }
    }
}
