namespace Maincotech.Messaging
{
    public interface IEventBus
    {
        void Publish<T>(T @event) where T : class;

        void Subscribe<T, TH>()
            where T : class
            where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : class;
    }
}