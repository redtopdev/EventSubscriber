using EventStore.ClientAPI;
using System.Threading.Tasks;

namespace Engaze.Event.Subscriber.Service
{
    interface IEventMessageHandler
    {
        Task ProcessMessage(byte[] message);
    }
}
