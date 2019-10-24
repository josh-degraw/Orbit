using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

using Orbit.Components;
using Orbit.Data;
using Orbit.Models;
using Orbit.Util;

using Xunit;

namespace Orbit.Tests.Components
{
    public class BatteryComponent_tests
    {
        [Fact]
        public void Can_return_default_value()
        {
            using var scope = OrbitServiceProvider.Instance.CreateScope();

            scope.ServiceProvider.GetRequiredService<OrbitDbContext>().InsertSeedData();

            var service = scope.ServiceProvider.GetService<IMonitoredComponent<BatteryReport>>();
            service.Invoking(async s => await s.GetCurrentValueAsync()).Should().NotThrow();
        }
    }
}