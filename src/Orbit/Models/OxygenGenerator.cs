using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class OxygenGenerator : IAlertableModel
    {
        #region Limits
        // these should not be constant so crew can make changes in orbit
        [NotMapped]
        int cellOutputLiters = 270;
        [NotMapped]
        int oxygenNeedPerPersonLiters = 550;
        [NotMapped]
        int oxygenLevelTolerance = 2;
        [NotMapped]
        int totalNumOfCells = 10;
        
        // makes passed in value accessible to private methods
        [NotMapped]
        double currentOxygenLevel;
        #endregion Limits

        #region Public Properties

        [NotMapped]
        public string ComponentName => "OxygenGenerator";

        public DateTimeOffset ReportDateTime { get; set; } 

        /// <summary>
        /// state of system; standby, processing, fail, etc
        /// </summary>
        public SystemStatus Status { get; set; } 

        /// <summary>
        /// denotes if this system is to run autonomously or manually
        /// </summary>
        public Modes Mode { get; set; }

        /// <summary>
        /// sensor that checks for bubbles in inflow water, water is sent back to water processor if bubbles are present
        /// </summary>
        public bool InflowBubblesPresent { get; set; }

        /// <summary>
        /// switches water from going to reaction to water processor if bubbles are present
        /// </summary>
        public DiverterValvePositions DiverterValvePosition { get; set; }
        
        /// <summary>
        /// checks if hydrogen is present in product oxygen flow, if yes then there is a problem in the system and it 
        /// shuts down
        /// </summary>
        public bool HydrogenSensor { get; set; }

        /// <summary>
        /// Separates hydrogen from water outflow, water is recirculated back into water generator
        /// hydrogen is sent to water generator or vented to space
        /// </summary>
        public bool SeparatorOn { get; set; }

        /// <summary>
        /// circulates water from clean water feed and rotary separator to electrolysis cell stack
        /// </summary>
        public bool RecirculationPumpOn { get; set; }

        /// <summary>
        /// Each person aboard would require at least 3 cells to be active to maintain baseline oxygen requirements
        /// </summary>
        public int NumActiveCells { get; set; }

        /// <summary>
        /// current oxygen output of the system
        /// </summary>
        public double SystemOutput { get; set; }

        /// <summary>
        /// The desired oxygen concentration in the station air
        /// </summary>
        public int OxygenSetLevel { get; set; }


        #endregion Public Properties

        #region Public Methods

        public void ProcessData(double oxyLevel) 
        {
            currentOxygenLevel = oxyLevel;
            if (Status.Equals(SystemStatus.Processing))
            {
                SimulateProcessing();
            }
            else if (Status.Equals(SystemStatus.Standby))
            {
                SimulateStandby();
            }
            else { }
            
            SimulateOutput();
        }

        #endregion  Public Methods

        #region Private Methods

        private void Trouble() 
        {
            Status = SystemStatus.Trouble;
            Standby();
        }

        private void Standby()
        {
            Status = SystemStatus.Standby;
            SeparatorOn = false;
            RecirculationPumpOn = false;
            NumActiveCells = 0;
        }

        private void Process() 
        {
            Status = SystemStatus.Processing;
            SeparatorOn = true;
            RecirculationPumpOn = true;
            if (InflowBubblesPresent)
            {
                DiverterValvePosition = DiverterValvePositions.Reprocess;
            }
            else
            {
                DiverterValvePosition = DiverterValvePositions.Accept;
            }
        }

        private void SimulateProcessing()
        {
            if (currentOxygenLevel < OxygenSetLevel)
            {
                if (NumActiveCells >= totalNumOfCells)
                {
                    // change to a trouble status, but keep the system working
                    Status = SystemStatus.Trouble;
                }
                else
                {
                    // increase num of active electrolysis cells to increase oxygen output
                    NumActiveCells++;
                }
            }
            else if (currentOxygenLevel > OxygenSetLevel)
            {
                // decrease active cells to decrease oxygen output if there is at least on active cell
                if (NumActiveCells > 0)
                {
                    NumActiveCells--;
                    
                    // only one acive cell, change system to standby
                    if (NumActiveCells == 0)
                    {
                        Standby();
                    }
                }
            }
            else if (InflowBubblesPresent)
            {
                // bubbles add unwanted gasses to hydrogen flow when seperated downstream
                DiverterValvePosition = DiverterValvePositions.Reprocess;
            }
            else if (!InflowBubblesPresent)
            {
                DiverterValvePosition = DiverterValvePositions.Accept;
            }
            // system cannot work if any of these components fail (simulated by an 'off' state)
            if (HydrogenSensor || !SeparatorOn || !RecirculationPumpOn)
            {
                Trouble();
            }
        }

        private void SimulateStandby()
        {
            if (currentOxygenLevel < OxygenSetLevel)
            {
                Status = SystemStatus.Processing;
            }
            // all components shoud be off while in Standby, otherwise there is a system fault
            if ( SeparatorOn || RecirculationPumpOn)
            {
                Trouble();
            }
        }

        private void SimulateOutput()
        {
            SystemOutput = NumActiveCells * cellOutputLiters;
        }

        #endregion Private Methods

        #region Check Alerts

        private IEnumerable<Alert> CheckHydrogenSensor()
        {
            if (HydrogenSensor)
            {
                Trouble();
                yield return new Alert(nameof(HydrogenSensor), "Hydrogen detected in outflow", AlertLevel.HighError);
            }
            else
            {
                yield return Alert.Safe(nameof(HydrogenSensor));
            }
        }

        private IEnumerable<Alert> CheckSeperator()
        {
            if (Status.Equals(SystemStatus.Processing))
            {
                if (!SeparatorOn)
                {
                    Trouble();
                    yield return new Alert(nameof(SeparatorOn), "Seperator off while in processing status", AlertLevel.HighError);
                }
            }
            else if (Status.Equals(SystemStatus.Standby))
            {
                if (SeparatorOn)
                {
                    Trouble();
                    yield return new Alert(nameof(SeparatorOn), "Seperator on while in Standby status", AlertLevel.HighError);
                }
            }
            else
            {
                yield return Alert.Safe(nameof(SeparatorOn));
            }
        }

        private IEnumerable<Alert> CheckPump()
        {
            if (Status.Equals(SystemStatus.Processing))
            {
                if (!RecirculationPumpOn)
                {
                    Trouble();
                    yield return new Alert(nameof(RecirculationPumpOn), "Pump is off while in Processing status", AlertLevel.HighError);
                }
            }
            else if (Status.Equals(SystemStatus.Standby))
            {
                if (RecirculationPumpOn)
                {
                    Trouble();
                    yield return new Alert(nameof(RecirculationPumpOn), "Pump in on while in Standby status", AlertLevel.HighError);
                }
            }
            else
            {
                yield return Alert.Safe(nameof(RecirculationPumpOn));
            }
        }

        private IEnumerable<Alert> CheckMaxProduction()
        {
            // at maximum output and cabin oxygen concentration is still low
            if((NumActiveCells >= totalNumOfCells) && (currentOxygenLevel < (OxygenSetLevel - oxygenLevelTolerance)))
            {
                yield return new Alert(nameof(NumActiveCells), "Maximum oxygen production not maintaining set oxygen level", AlertLevel.HighError);
            }
            else if(NumActiveCells >= totalNumOfCells && (currentOxygenLevel <= OxygenSetLevel))
            {
                yield return new Alert(nameof(NumActiveCells), "All oxygen production cells currently active", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(NumActiveCells));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckHydrogenSensor()
                .Concat(this.CheckSeperator())
                .Concat(this.CheckPump())
                .Concat(this.CheckMaxProduction());
        }

        #endregion  Check Alerts

        #region Equality Members

        public bool Equals(OxygenGenerator other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return this.ReportDateTime.Equals(other.ReportDateTime)
                && this.Status == other.Status
                && this.InflowBubblesPresent == other.InflowBubblesPresent
                && this.DiverterValvePosition == other.DiverterValvePosition
                && this.HydrogenSensor == other.HydrogenSensor
                && this.SeparatorOn == other.SeparatorOn
                && this.RecirculationPumpOn == other.RecirculationPumpOn
                && this.NumActiveCells == other.NumActiveCells
                && this.SystemOutput == other.SystemOutput
                && this.Mode == other.Mode
                && this.OxygenSetLevel == other.OxygenSetLevel;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is OxygenGenerator other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.ReportDateTime,
                this.Status,
                this.InflowBubblesPresent,
                this.DiverterValvePosition,
                this.HydrogenSensor,
                this.SeparatorOn,
                this.RecirculationPumpOn,

                // tuple for overflow arguments
                (this.NumActiveCells, this.SystemOutput, this.Mode, this.OxygenSetLevel)
                );
        }

        public static bool operator ==(OxygenGenerator left, OxygenGenerator right) => Equals(left, right);

        public static bool operator !=(OxygenGenerator left, OxygenGenerator right) => !Equals(left, right);

        #endregion Equality Members
    }
}