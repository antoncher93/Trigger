using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Trigger.Classes;
using Trigger.Interfaces;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class BaseTest
    {
        private readonly IServiceProvider _serviceProvider;

        public IRangerPool GetNewPool()
        {
            return _serviceProvider.GetService<IRangerPool>();
        }

        public ILogger _logger;

        public BaseTest(ITestOutputHelper testOutputHelper)
        {
            _serviceProvider =
                  new ServiceCollection()
                      .AddTransient<IRangerPool, RangerPool>()
                      .AddSingleton<IRangerSettings, DummyRangerSettings>()
                      .AddLogging()
                      .BuildServiceProvider();

            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<BaseTest>();
        }
    }
}