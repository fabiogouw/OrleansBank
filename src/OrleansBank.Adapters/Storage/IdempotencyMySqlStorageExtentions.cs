using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Hosting;
using Orleans.Runtime;
using Orleans.Storage;
using OrleansBank.Adapters.Storage;

namespace OrleansBank.Adapters
{
    public static class IdempotencyMySqlStorageExtentions
    {
        public static ISiloBuilder AddIdempotentySqlServerGrainStorage(this ISiloBuilder builder, string providerName,
            Action<IdempotencyMySqlStorageOptions> options)
        {
            return builder.ConfigureServices(services => services.AddIdempotentySqlServerGrainStorage(providerName, options));
        }

        public static IServiceCollection AddIdempotentySqlServerGrainStorage(this IServiceCollection services, string providerName,
            Action<IdempotencyMySqlStorageOptions> options)
        {
            services.AddOptions<IdempotencyMySqlStorageOptions>(providerName).Configure(options);
            return services
                .AddSingletonNamedService(providerName, Create)
                //.AddSingletonNamedService(providerName, (s, n) => (ILifecycleParticipant<ISiloLifecycle>)s.GetRequiredServiceByName<IGrainStorage>(n))
                ;
        }

        internal static IGrainStorage Create(IServiceProvider services, string name)
        {
            var options = services.GetRequiredService<IOptionsMonitor<IdempotencyMySqlStorageOptions>>();
            return ActivatorUtilities.CreateInstance<IdempotentMySqlAccountStorage>(services, name, options.Get(name));
        }
    }
}
