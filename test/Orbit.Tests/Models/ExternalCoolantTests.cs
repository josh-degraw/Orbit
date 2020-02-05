using System;
using System.Collections.Generic;
using System.Text;
using Orbit.Models;
using Xunit;

namespace Orbit.Tests.Models
{
    public class ExternalCoolantTests
    {
        [Fact]
        public void Test_Nominal_Operation()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.ProcessData();
            Assert.True(ec.Status == SystemStatus.On);
        }

        [Fact]
        public void Test_PumpA_Failure()
        {
            ExternalCoolantLoopData ecl = new ExternalCoolantLoopData();
            ecl.SeedData();
            ecl.PumpAOn = false;
            ecl.ProcessData();
            Assert.True(ecl.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void Test_PumpB_Failure()
        {
            ExternalCoolantLoopData ecl = new ExternalCoolantLoopData();
            ecl.SeedData();
            ecl.PumpBOn = false;
            ecl.ProcessData();
            Assert.True(ecl.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void Test_Radiator_Not_Deployed()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.RadiatorDeployed = false;
            ec.ProcessData();
            Assert.True(ec.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void Test_OutflowTemp_Low()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.OutputFluidTemperature = 0;
            ec.ProcessData();
            Assert.True(ec.MixValvePosition == 24);
        }

        [Fact]
        public void Test_OutflowTemp_High()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.OutputFluidTemperature = 5;
            ec.ProcessData();
            Assert.True(ec.MixValvePosition == 26);
        }

        [Fact]
        public void Test_LineAPressure_Over_Max()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.LineAPressure = 481;
            ec.ProcessData();
            Assert.False(ec.PumpAOn);
        }

        [Fact]
        public void Test_LineBPressure_Over_Max()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.LineBPressure = 481;
            ec.ProcessData();
            Assert.False(ec.PumpBOn);
        }

        [Fact]
        public void Test_LineAPressure_High()
        {
            ExternalCoolantLoopData ec = new ExternalCoolantLoopData();
            ec.SeedData();
            ec.LineAPressure = 481;
            ec.ProcessData();
            Assert.True(ec.Status == SystemStatus.Trouble);
        }



    }
}
