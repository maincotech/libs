using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Maincotech.Messaging
{
    public class MessagingStartup : IStartup
    {
        public int Order => 1;
        public IEventBusSubscriptionsManager EventBusSubscriptionsManager { get; set; }

        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var options = new MessagingOptions();
            configuration.GetSection("Messaging").Bind(options);

            if (EventBusSubscriptionsManager == null)
            {
                EventBusSubscriptionsManager = new InMemoryEventBusSubscriptionsManager();
                services.AddSingleton<IEventBusSubscriptionsManager>(EventBusSubscriptionsManager);
            }

            switch (options.ServiceBusProvider)
            {
                case ServiceBusProvider.InMemory:
                    services.AddMassTransit(x =>
                    {
                        x.UsingInMemory((context, cfg) =>
                        {
                            cfg.ReceiveEndpoint(options.Endpoint ?? "message-queue", e =>
                               {
                                   e.Consumer(() => new MessageConsumer(EventBusSubscriptionsManager));
                               });
                        });
                    });
                    break;

                default:
                    break;
            }
        }
    }
}