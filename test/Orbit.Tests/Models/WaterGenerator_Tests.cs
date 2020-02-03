using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Orbit.Models;

namespace Orbit.Tests.Models
{
    public class WaterGenerator_Tests
    {
        private readonly ITestOutputHelper output;

        public WaterGenerator_Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestProcessingIncreasesMethane()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 400;
            wg.SeperatorMotorSpeed = 2000;
            wg.ProcessData();
            Assert.True(wg.MethaneStoreLevel == 35);
        }

        [Fact]
        public void TestProcessingDecreasesH2()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 400;
            wg.SeperatorMotorSpeed = 2000;
            wg.ProcessData();
            Assert.True(wg.H2StoreLevel == 18);
        }

        [Fact]
        public void TestProcessingDecreasesCo2()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 400;
            wg.SeperatorMotorSpeed = 2000;
            wg.ProcessData();
            Assert.True(wg.Co2StoreLevel == 22);
        }

        [Fact]
        public void TestProcessingMethaneFull()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 475;
            wg.SeperatorOn = true;
            wg.SeperatorMotorSpeed = 2000;
            wg.MethaneStoreLevel = 95;
            wg.ProcessData();
            Assert.True(wg.MethaneStoreLevel == 0);
        }

        [Fact]
        public void TestProcessingH2Empty()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 475;
            wg.SeperatorOn = true;
            wg.SeperatorMotorSpeed = 2000;
            wg.H2StoreLevel = 2;
            wg.ProcessData();
            Assert.True(wg.H2StoreLevel == 0);
        }

        [Fact]
        public void TestProcessingCo2Empty()
        {
            WaterGeneratorData wg = new WaterGeneratorData();
            wg.SeedData();
            wg.Status = SystemStatus.Processing;
            wg.PumpOn = true;
            wg.ReactorTemp = 475;
            wg.SeperatorOn = true;
            wg.SeperatorMotorSpeed = 2000;
            wg.Co2StoreLevel = 4;
            wg.ProcessData();
            Assert.True(wg.Co2StoreLevel == 0);
        }


    }
}
