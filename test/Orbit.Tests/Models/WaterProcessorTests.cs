using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Orbit.Models;
using Xunit;

namespace Orbit.Tests.Models
{
    public class WaterProcessorTests
    {
        [Fact]
        public void Should_properly_infer_range_limits()
        {
            var data = new WaterProcessorData();

            var alert = data.CreateRangedAlertSafe(nameof(data.ProductTankLevel));
            alert.Minimum.Should().Be(0,because:"the property has a RangeAttribute applied with the min set to 0");
            alert.Maximum.Should().Be(100, because: "the property has a RangeAttribute applied with the max set to 100");
        }

        [Fact]
        public void Should_fail_to_infer_range_limits_when_no_RangeAttribute_is_applied()
        {
            new WaterProcessorData()
                .Invoking(data => data.CreateRangedAlertSafe(nameof(data.PostReactorQualityOk)))
                .Should()
                .Throw<InvalidOperationException>(
                    because: "the system has no way of inferring a range when no attribute is found");
        }
    }
}
