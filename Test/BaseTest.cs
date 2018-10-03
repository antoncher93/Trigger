using Microsoft.Extensions.DependencyInjection;
using System;
using Trigger.Classes;
using Trigger.Interfaces;

namespace Trigger.Test
{
    public class BaseTest
    {
        private readonly IServiceProvider _serviceProvider;

        public IObjectPool<string, Ranger> GetNewPool()
        {
            return _serviceProvider.GetService<IObjectPool<string, Ranger>>();
        }

        public BaseTest()
        {
            _serviceProvider =
                  new ServiceCollection()
                      .AddTransient<IObjectPool<string, Ranger>, RangerPool>()
                      .AddSingleton<IRangerSettings, DummyRangerSettings>()
                      .BuildServiceProvider();
        }
    }
}