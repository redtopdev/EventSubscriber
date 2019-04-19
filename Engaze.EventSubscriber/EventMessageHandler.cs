using Engaze.Core.MessageBroker;
using System.Threading.Tasks;

namespace Engaze.Event.Subscriber.Service
{
    public class EventMessageHandler : IEventMessageHandler
    {
        private IMessageProducer<byte[]> messageProducer;
        public EventMessageHandler(IMessageProducer<byte[]> messageProducer)
        {
            this.messageProducer = messageProducer;
        }
        public async Task ProcessMessage(byte[] message)
        {
          await messageProducer.WriteAsync(message, "evento");           
        }       
    }
}