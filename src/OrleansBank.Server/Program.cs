using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Orleans.Hosting;

namespace OrleansBank.Server
{
    class Program
    {
        private static ISiloHost _silo;
        static void Main(string[] args)
        {
            _silo = StartSilo().Result;
            Console.ReadLine();
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .Configure<EndpointOptions>(options => 
                {
                    options.AdvertisedIPAddress = IPAddress.Loopback;
                })
                //.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(CheckingAccountGrain).Assembly).WithReferences())
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
