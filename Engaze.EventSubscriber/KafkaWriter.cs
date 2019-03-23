using Engaze.Core.MessageBroker;
using EventStore.ClientAPI;
using System;
using System.Threading.Tasks;

namespace Engaze.Event.Subscriber.Service
{
    public class KafkaWriter : IMessageHandler
    {
        private IMessageProducer<RecordedEvent> messageProducer;
        public KafkaWriter(IMessageProducer<RecordedEvent> messageProducer)
        {
            this.messageProducer = messageProducer;
        }
        public async Task ProcessMessage(RecordedEvent message)
        {
            await messageProducer.WriteAsync(message, "Evento");
        }
    }
}
