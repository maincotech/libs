using Maincotech.Caching;
using Maincotech.Core.Resources;
using Maincotech.Localization;
using Maincotech.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationExtensions
    {
        public static IServiceCollection AddLocalizationCore(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("localizationDb");
            if (connectionString.IsNullOrEmpty())
            {
                throw new NotSupportedException(ResourceCore.ConnectionStringNotConfigured);
            }

            //add database context
            services.AddDbContext<LocalizationDbContext>(options => options.UseSqlServer(connectionString));

            //add repositories

            //add service
            services.AddScoped<ILocalizationService, LocalizationService>(sp =>
            {
                var dbContext = sp.GetRequiredService<LocalizationDbContext>();
                var cachedManager = sp.GetService<ICacheManager>();
                return new LocalizationService(dbContext, cachedManager);
            });

            //Setup database
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
            dbContext.EnsureCreated();

            return services;
        }

        public static IServiceCollection AddLocalizationCore([NotNull] this IServiceCollection services, [NotNull] Action<DbContextOptionsBuilder> optionsAction, ServiceLifetime dbContextLifetime = ServiceLifetime.Transient)
        {
            //add database context
            services.AddDbContext<LocalizationDbContext>(optionsAction, dbContextLifetime);

            //add repositories

            //add service
            services.AddScoped<ILocalizationService, LocalizationService>(sp =>
            {
                var dbContext = sp.GetRequiredService<LocalizationDbContext>();
                var cachedManager = sp.GetService<ICacheManager>();
                return new LocalizationService(dbContext, cachedManager);
            });

            //Setup database
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
            dbContext.EnsureCreated();

            return services;
        }
    }
}