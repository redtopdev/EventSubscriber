using EventStore.ClientAPI;
using System.Threading.Tasks;

namespace Engaze.Event.Subscriber.Service
{
    interface IMessageHandler
    {
        Task ProcessMessage(RecordedEvent message);
    }
}
