using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Orbit.Models;

namespace Orbit.Tests.Models
{
    public class Atmosphere_Tests
    {
        [Fact]
        public void TestTempWithinAcceptableUpperDeviance()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.Temperature = 19.5;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 60);
        }

        [Fact]
        public void TestTempWithinAcceptableLowerDeviance()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.Temperature = 18.5;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 60);
        }

        [Fact]
        public void TestCrewedHumidityWithinAcceptableUpperDeviance()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.HumidityLevel = 45;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 60);
        }


        [Fact]
        public void TestHumidityWithinAcceptableLowerDeviance()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.HumidityLevel = 35;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 60);
        }

        [Fact]
        public void TestTempToHot()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.Temperature = 19.6;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 61);
        }

        [Fact]
        public void TestHumidToHigh()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.HumidityLevel = 60;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 61);
        }

        [Fact]
        public void TestMaxedTempControlBaffle()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.TempControlBafflePosition = 100;
            ad.Temperature = 19.6;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 100);
        }
        
        [Fact]
        public void TestUnCrewedToCold()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.Temperature = 2;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 59);
        }

        [Fact]
        public void TestZeroedTempControlBaffle()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.TempControlBafflePosition = 0;
            ad.Temperature = 18.4;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 0);
        }

        [Fact]
        public void TestUnCrewedToDry()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.HumidityLevel = 20;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 59);
        }

        [Fact]
        public void TestCrewedToCold()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.CabinStatus = Modes.Crewed;
            ad.Temperature = 15;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 59);
        }

        [Fact]
        public void TestCrewedToDry()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.HumidityLevel = 34;
            ad.ProcessData();
            Assert.True(ad.TempControlBafflePosition == 59);
        }

        [Fact]
        public void TestLiquidInOutflow()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.LiquidInOutflow = true;
            ad.ProcessData();
            Assert.True(ad.ReprocessBafflePosition == DiverterValvePositions.Reprocess);
        }

        [Fact]
        public void TestLiquidInOutflowCleared()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.ReprocessBafflePosition = DiverterValvePositions.Reprocess;
            ad.LiquidInOutflow = false;
            ad.ProcessData();
            Assert.True(ad.ReprocessBafflePosition == DiverterValvePositions.Accept);
        }

        [Fact]
        public void TestNewTempValueGenerated()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            AtmosphereData newad = new AtmosphereData(ad);
            Assert.False(ad.Temperature == newad.Temperature);
        }
        
        [Fact]
        public void TestSeperatorOffWhileUncrewed()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            AtmosphereData newad = new AtmosphereData(ad);
            Assert.True(newad.SeperatorSpeed == 0);
        }

        [Fact]
        public void TestSeperatorOnWhileCrewed()
        {
            AtmosphereData ad = new AtmosphereData();
            ad.SeedData();
            ad.CabinStatus = Modes.Crewed;
            AtmosphereData newad = new AtmosphereData(ad);
            Assert.True(newad.SeperatorSpeed > 1000);
        }

    }
}
