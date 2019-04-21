using Engaze.Core.MessageBroker;
using EventStore.ClientAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
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
        public async Task ProcessMessage(ResolvedEvent @event)
        {
            await  messageProducer.WriteAsync(ComposeMessageForProducer(@event), "evento");
        }

        private byte[] ComposeMessageForProducer(ResolvedEvent @event)
        {
            dynamic eventoJObject = new JObject();
            eventoJObject.Data = Encoding.UTF8.GetString(@event.Event.Data);
            eventoJObject.EventType = GetEventTypeShortName(@event.Event.EventType);
            return Encoding.UTF8.GetBytes(eventoJObject.ToString());

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventTypeAssemblyName"></param>
        /// <returns></returns>
        private string GetEventTypeShortName(string eventTypeAssemblyName)
        {
            //Event type is combination of class full name and class assembly name 
            //Engaze.Evento.Domain.Event.EventoCreated, Engaze.Evento.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            var eventTypeFullName = eventTypeAssemblyName.Split(',')[0];
            return eventTypeFullName.Substring(eventTypeFullName.LastIndexOf('.') + 1);
        }
    }
}