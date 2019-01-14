using System;
using System.Collections.Generic;
using Trigger.Beacons;
using Trigger.Interfaces;
using Trigger.Rangers;
using System.Data.SqlClient;
using Trigger.Signal;
using Trigger.Classes;
using Trigger;
using Microsoft.Extensions.DependencyInjection;
using RangerTest.Infrastructure;

namespace RangerTest
{
    class Program
    {
        const string SqlConnectionString = @"Data Source=192.168.0.9;Initial Catalog=Shoppercoin;Persist Security Info=True;User ID=ivg;Password=ivg";

        static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddLogging()
                .AddSingleton<IRangerSettings, BaseRangerSettings>()
                .AddSingleton<ITwoLineRangerBuilder, RangerBuilder>()
                .AddScoped<IRepository<Telemetry>, SqlDataTelemetryRepository>()
                .AddSingleton<IRangerPool, RangerPool>();
        }

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var rangerPool = serviceProvider.GetRequiredService<IRangerPool>();

            rangerPool.OnEvent += (s, e) =>
            {
                Console.WriteLine(e.Type + " " + e.Timespan);

                switch (e.Type)
                {
                    case Trigger.Enums.TriggerEventType.Enter:

                        break;
                    case Trigger.Enums.TriggerEventType.Exit:
                        break;
                }
            };

            Console.WriteLine("Hello World!");

            var telemRepos = serviceProvider.GetRequiredService<IRepository<Telemetry>>();

            foreach (var telemetry in telemRepos.GetItems())
            {
                Console.WriteLine(telemetry.Protocol);

                var ranger = rangerPool["Nike"];

                ranger.OnNext(telemetry);

                Console.WriteLine();
                Console.WriteLine(ranger.Report);
            }

            telemRepos.Dispose();

            Console.ReadKey();
        }

       
    }
}
