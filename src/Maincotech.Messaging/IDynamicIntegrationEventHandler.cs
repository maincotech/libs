using System.Threading.Tasks;

namespace Maincotech.Messaging
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }
}