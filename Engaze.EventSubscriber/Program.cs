using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using System;
using Engaze.Event.Subscriber.Service;
using Engaze.Core.MessageBroker;
using EventStore.ClientAPI;

namespace EventSubscriber
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder().ConfigureHostConfiguration(configHost =>
             {
                 configHost.AddCommandLine(args);
                 configHost.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
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
                 services.AddLogging();
                 services.AddSingleton<KafkaConfiguration>();
                 services.AddSingleton(typeof(IMessageProducer<RecordedEvent>), typeof(KafkaProducer<RecordedEvent>));
                 services.AddSingleton<EventStreamListener>();
                 services.AddSingleton(typeof(IMessageHandler), typeof(KafkaWriter));
                 services.AddHostedService<SubscriberService>();

             }).Build();
            host.Run();
        }
    }
}
