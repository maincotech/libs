using MassTransit;
using System;
using System.Text.Json;

namespace Maincotech.Messaging
{
    public class EventBusMassTransit : IEventBus, IDisposable
    {
        private IBusControl _BusControl;
        private readonly IEventBusSubscriptionsManager _subsManager;
        private const string INTEGRATION_EVENT_SUFFIX = "IntegrationEvent";

        public EventBusMassTransit(IEventBusSubscriptionsManager subscriptionsManager, IBusControl busControl)
        {
            _subsManager = subscriptionsManager;
            _BusControl = busControl;
        }

        public void Dispose()
        {
            _BusControl.Stop();
            _BusControl = null;
        }

        public void Publish<T>(T @event) where T : class
        {
            var message = new Message
            {
                EventBody = JsonSerializer.Serialize(@event, typeof(T)),
                CreationDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                EventName = typeof(T).FullName
            };
            _BusControl.Publish(message);
        }

        public void Subscribe<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name.Replace(INTEGRATION_EVENT_SUFFIX, "");
            _subsManager.AddSubscription<T, TH>();
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.AddDynamicSubscription<TH>(eventName);
        }

        public void Unsubscribe<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _subsManager.RemoveDynamicSubscription<TH>(eventName);
        }
    }
}