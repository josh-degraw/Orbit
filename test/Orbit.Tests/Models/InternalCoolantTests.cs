using System;
using System.Collections.Generic;
using System.Text;
using Orbit.Models;
using Xunit;


namespace Orbit.Tests.Models
{
    public class InternalCoolantTests
    {
        [Fact]
        public void TestDualLoop_TempWithinRange()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.ProcessData();
            Assert.True((ic.LowTempMixValvePosition == 15) && (ic.MedTempMixValvePosition == 32));
        }
        [Fact]
        public void TestDualLoop_LowTempHigh()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.TempLowLoop = 8;
            ic.ProcessData();
            Assert.True(ic.LowTempMixValvePosition == 16);
        }

        [Fact]
        public void TestDualLoop_LowTempLow()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.TempLowLoop = 0;
            ic.ProcessData();
            Assert.True(ic.LowTempMixValvePosition == 14);
        }

        [Fact]
        public void TestDualLoop_MedTempHigh()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.TempMedLoop = 15;
            ic.ProcessData();
            Assert.True(ic.MedTempMixValvePosition == 33);
        }

        [Fact]
        public void TestDualLoop_MedTempLow()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.TempMedLoop = 6;
            ic.ProcessData();
            Assert.True(ic.MedTempMixValvePosition == 31);
        }

        [Fact]
        public void TestBothPumpsOff()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.LowTempPumpOn = false;
            ic.MedTempPumpOn = false;
            ic.ProcessData();
            Assert.True(ic.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestManualMedSingleLoop_WithinRange()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.LowTempPumpOn = false;
            ic.MedTempSingleLoop = true;
            ic.ProcessData();
            Assert.True(ic.Status == SystemStatus.On);
        }

        [Fact]
        public void TestManualLowSingleLoop_LowLoopTempHigh()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.MedTempPumpOn = false;
            ic.LowTempSingleLoop = true;
            ic.TempLowLoop = 7;
            ic.CrossoverMixValvePosition = 12;
            ic.ProcessData();
            Assert.True(ic.CrossoverMixValvePosition == 13);
        }

        [Fact]
        public void TestManualLowSingleLoop_MedLoopTempLow()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.MedTempPumpOn = false;
            ic.LowTempSingleLoop = true;
            ic.TempMedLoop = 6;
            ic.CrossoverMixValvePosition = 12;
            ic.ProcessData();
            Assert.True(ic.CrossoverMixValvePosition == 11);
        }

        [Fact]
        public void TestConvertFromDualToSingleLoop()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.MedTempPumpOn = false;
            ic.ProcessData();
            Assert.True(ic.CrossoverMixValvePosition == 40);
        }

        [Fact]
        public void TestDualLoopPumpFailTrouble()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.LowTempPumpOn = false;
            ic.ProcessData();
            Assert.True(ic.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestSingleLowLoopPumpFailure()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.LowTempSingleLoop = true;
            ic.MedTempPumpOn = false;
            ic.LowTempPumpOn = false;
            ic.ProcessData();
            Assert.True(ic.MedTempPumpOn);
        }

        [Fact]
        public void TestSingleMedLoopPumpFailure()
        {
            InternalCoolantLoopData ic = new InternalCoolantLoopData();
            ic.SeedData();
            ic.MedTempSingleLoop = true;
            ic.MedTempPumpOn = false;
            ic.LowTempPumpOn = false;
            ic.ProcessData();
            Assert.True(ic.Status == SystemStatus.Trouble);
        }
    }
}
