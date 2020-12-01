using System;
using System.Collections.Generic;
using static Maincotech.Messaging.InMemoryEventBusSubscriptionsManager;

namespace Maincotech.Messaging
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }

        event EventHandler<string> OnEventRemoved;

        void AddDynamicSubscription<TH>(string eventName)
           where TH : IDynamicIntegrationEventHandler;

        void AddSubscription<T, TH>()
           where T : class
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : class;

        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        bool HasSubscriptionsForEvent<T>() where T : class;

        bool HasSubscriptionsForEvent(string eventName);

        Type GetEventTypeByName(string eventName);

        void Clear();

        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : class;

        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);

        string GetEventKey<T>();
    }
}