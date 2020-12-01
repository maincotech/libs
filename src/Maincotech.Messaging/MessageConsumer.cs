using Maincotech.Logging;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Maincotech.Messaging
{
    public class MessageConsumer : IConsumer<Message>
    {
        private IEventBusSubscriptionsManager _subsManager;
        private static ILogger _Logger = AppRuntimeContext.Current.GetLogger<MessageConsumer>();

        public MessageConsumer(IEventBusSubscriptionsManager manager)
        {
            _subsManager = manager;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            _Logger.Debug($"Event:{context.Message.EventName} created on {context.Message.CreationDate} is processed on {DateTime.UtcNow}.");
            await ProcessEvent(context.Message.EventName, context.Message.EventBody);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            _Logger.Info($"Processing event: {eventName}");

            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptions = _subsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    if (subscription.IsDynamic)
                    {
                        var handler = AppRuntimeContext.Current.ServiceProvider.GetRequiredService(subscription.HandlerType) as IDynamicIntegrationEventHandler;
                        if (handler == null) continue;
                        dynamic eventData = JObject.Parse(message);

                        await Task.Yield();
                        await handler.Handle(eventData);
                    }
                    else
                    {
                        var handler = AppRuntimeContext.Current.ServiceProvider.GetRequiredService(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = _subsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                        await Task.Yield();
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
            }
            else
            {
                _Logger.Warning($"No subscription for  event: {eventName}");
            }
        }
    }
}