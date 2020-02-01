﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using Orbit.Models;

namespace Orbit.Tests.Models
{
    public class OxygenGenerator_Tests
    {
        private readonly ITestOutputHelper output;

        public OxygenGenerator_Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestSystemOutputStandby()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.OxygenLevel = 22;
            o2.ProcessData();
            output.WriteLine("SystemOutput = {0}, NumCells = {1}, O2Level = {2}", o2.SystemOutput, o2.NumActiveCells, o2.OxygenLevel);
            Assert.True(o2.SystemOutput == 0);
        }

        [Fact]
        public void TestSystemOutputProcessing()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.ProcessData();
            // numactive cells = 1 * 270 output/cell
            Assert.True(o2.SystemOutput == 270);
        }

        [Fact]
        public void TestSystemOutputTrouble1()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Trouble;
            o2.lastWorkingStatus = SystemStatus.Standby;
            o2.ProcessData();
            // numactive cells = 1 * 270 output/cell
            Assert.True(o2.SystemOutput == 0);
        }

        [Fact]
        public void TestSystemOutputTrouble2()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Trouble;
            o2.lastWorkingStatus = SystemStatus.Processing;
            o2.ProcessData();
            // numactive cells * 270 output/cell
            Assert.True(o2.SystemOutput == 270);
        }

        [Fact]
        public void TestBubbleSensorTrue()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.InflowBubblesPresent = true;
            o2.ProcessData();
            Assert.True(o2.DiverterValvePosition == DiverterValvePositions.Reprocess);
        }

        [Fact]
        public void TestDecreaseActiveCells()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.NumActiveCells = 3;
            o2.OxygenLevel = 22;
            o2.ProcessData();
            Assert.True(o2.NumActiveCells == 2);
        }

        [Fact]
        public void TestBubbleSensorFalse()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.InflowBubblesPresent = false;
            o2.ProcessData();
            Assert.True(o2.DiverterValvePosition == DiverterValvePositions.Accept);
        }

        [Fact]
        public void TestGoToStandby()
        {
            // status = standby, active cells = 0, o2level = 12, setlevel = 20
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.SeparatorOn = true;
            o2.RecirculationPumpOn = true;
            o2.NumActiveCells = 1;
            o2.OxygenLevel = 22;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Standby);
        }

        [Fact]
        public void TestH2SensorTroubleProcessing()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.HydrogenSensor = true;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestH2SensorTroubleStandby()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Standby;
            o2.HydrogenSensor = true;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestSeperatorTroubleStandby()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.SeparatorOn = true;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestSeperatorTroubleProcessing()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.SeparatorOn = false;
            o2.RecirculationPumpOn = true;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestPumpTroubleStandby()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Standby;
            o2.RecirculationPumpOn = true;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }

        [Fact]
        public void TestPumpTroubleProcessing()
        {
            OxygenGenerator o2 = new OxygenGenerator();
            o2.SeedData();
            o2.Status = SystemStatus.Processing;
            o2.SeparatorOn = true;
            o2.RecirculationPumpOn = false;
            o2.ProcessData();
            Assert.True(o2.Status == SystemStatus.Trouble);
        }
    }
}
