﻿using Engaze.Core.Common;
using Engaze.Core.DataContract;
using Engaze.Core.MessageBroker;
using EventStore.ClientAPI;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Engaze.Event.Subscriber.Service
{
    public class EventMessageHandler : IEventMessageHandler
    {
        private IMessageProducer<dynamic> messageProducer;
        private KafkaConfiguration kafkaConfiguration;
        public EventMessageHandler(IMessageProducer<dynamic> messageProducer, KafkaConfiguration kafkaConfiguration)
        {
            this.messageProducer = messageProducer;
            this.kafkaConfiguration = kafkaConfiguration;
        }
        public async Task ProcessMessage(ResolvedEvent @event)
        {
            await messageProducer.WriteAsync(ComposeMessageForProducer(@event), kafkaConfiguration.Topic);
        }

        private EventStoreEvent ComposeMessageForProducer(ResolvedEvent @event)
        {
            try
            {
                return  new EventStoreEvent()
                {
                    EventId = Guid.Parse(@event.Event.EventStreamId.Substring(@event.Event.EventStreamId.IndexOf('-') + 1)),
                    Data = Encoding.UTF8.GetString(@event.Event.Data),
                    EventType = (OccuredEventType)Enum.Parse(typeof(OccuredEventType), GetEventTypeShortName(@event.Event.EventType))
                };
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

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