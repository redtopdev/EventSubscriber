using Engaze.Core.MessageBroker;
using Engaze.Event.Subscriber.Service;
using Engaze.EventSubscriber.Service;
using EventStore.ClientAPI;
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
            IServiceCollection serviceCollection = null;
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
                 services.Configure<EventSubsriptionSetting>(hostContext.Configuration.GetSection("EventSubsriptionSetting"));
                 services.AddLogging();
                 services.AddSingleton<KafkaConfiguration>();
                 services.AddSingleton(typeof(IMessageProducer<RecordedEvent>), typeof(KafkaProducer<RecordedEvent>));
                 services.AddSingleton<EventStreamListener>();
                 services.AddSingleton(typeof(IMessageHandler), typeof(KafkaWriter));
                 services.AddHostedService<SubscriberService>();
                 serviceCollection = services;
             }).Build();
            host.Run();
        }
    }
}
