using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using OrleansBank.Core.Adapters.Actors;

namespace OrleansBank.Server
{
    class Program
    {
        private static ISiloHost _siloHost;
        static void Main(string[] args)
        {
            StartSilo(args.Length > 0 ? Convert.ToInt32(args[0]) : 0);
            Console.ReadLine();
        }

        private static void StartSilo(int portAddition)
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || devEnvironmentVariable.ToLower() == "development";
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            if (isDevelopment) 
            {
                configurationBuilder.AddUserSecrets<Program>();
            }                
            var configuration = configurationBuilder.Build();
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                //.UseLocalhostClustering()
                .UseAzureStorageClustering(op => { op.ConnectionString = configuration["StorageClustering"]; })
                .AddMemoryGrainStorage("OrleansStorage")
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .UseDashboard(options => { options.Port = 8080 + portAddition; })
                .ConfigureEndpoints(siloPort: 11111 + portAddition, gatewayPort: 30000 + portAddition)
                .Configure<EndpointOptions>(options => 
                {
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                })
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(CheckingAccountGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            _siloHost = builder.Build();
            _siloHost.StartAsync();
        }
    }
}
