using Orbit.Util;
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Orbit.Components;
using Xunit;

namespace Orbit.Tests.Util
{
    public class OrbitServiceProvider_tests
    {
        [Fact]
        public void Can_instantiate_service()
        {
            var service = OrbitServiceProvider.Instance.GetService<BatteryComponent>();
            service.Should().NotBeNull();
            service.Invoking(async s => await s.GetCurrentValueAsync()).Should().NotThrow();
        }
    }
}
