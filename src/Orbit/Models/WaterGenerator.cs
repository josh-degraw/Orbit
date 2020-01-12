using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    class WaterGenerator
    {
        #region Limits

        private int seperatorSpeedUpperLimit = 24000;
        private int seperatorSpeedLowerLimit = 1000;
        private int seperatorSpeedTolerance = 250;

        private int reactorTempUpperLimit = 650;
        private int reactorTempLowerLimit = 450;
        private int reactorTempTolerance = 50;

        private int storeFull = 100;
        private int storeEmpty = 0;
        private int storeTolerance = 10;

        #endregion Limits

        #region Public Properties

        [NotMapped]
        public string ComponentName => "Water Generator";

        public DateTimeOffset ReportDateTime { get; set; }

        public SystemStatus Status { get; set; }

        /// <summary>
        /// seperates water from methane gas, false when SeperatorMotorSpeed = 0
        /// </summary>
        public bool SeperatorOn { get; set; }

        /// <summary>
        /// current operating speed of the motor in RPM
        /// </summary>
        public int SeperatorMotorSpeed { get; set; }

        /// <summary>
        /// set max operating speed of the motor in RPM
        /// </summary>
        public int SeperatorMotorSetSpeed { get; set; }  

        /// <summary>
        /// used to start reaction when reactor is cold. Reaction is exothermic so heater is
        /// not required to be on throughout the reaction
        /// </summary>
        public bool HeaterOn { get; set; }

        /// <summary>
        /// highest temperature in the reactor
        /// </summary>
        public int ReactorTemp { get; set; }

        /// <summary>
        /// desired reaction temperature, reactor temperature can be reduced by cooling system
        /// </summary>
        public int ReactorSetTemp { get; set; }

        /// <summary>
        /// moves combined water and methane product to seperator
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// amount of methane in holding tank, vents to space when full
        /// </summary>
        public int MethaneStoreLevel { get; set; }

        /// <summary>
        /// simulate storage of H2 for reaction
        /// </summary>
        public int H2StoreLevel { get; set; }

        /// <summary>
        /// simulate storage of Co2 for reaction
        /// </summary>
        public int Co2StoreLevel { get; set; }

        #endregion Public Properties

        #region Methods

        public void ProcessData()
        {
            if(Status == SystemStatus.Processing)
            {
                // if reaction is just being started, heater will need to 'preheat' the reactor
                if (ReactorTemp < reactorTempLowerLimit)
                {
                    HeaterOn = true;
                }
                else
                {
                    // heater not needed once reaction is in progress
                    HeaterOn = false;
                }

                SeperatorOn = true;
                PumpOn = true;

                // check for trouble states
                if((ReactorTemp > reactorTempUpperLimit) 
                    || (ReactorTemp < reactorTempLowerLimit)
                    || (!SeperatorOn)
                    || (!PumpOn)
                    || (MethaneStoreLevel > (storeFull - storeTolerance))
                    || (HeaterOn && (ReactorTemp < reactorTempLowerLimit)) )
                {
                    Trouble();
                }
                else
                {
                    SimulateProcessing();

                    // if stores are almost empty, change to standby state
                    if ((H2StoreLevel < 5) && (Co2StoreLevel < 5))
                    {
                        Status = SystemStatus.Standby;
                    }
                }
            }
            else if(Status == SystemStatus.Standby)
            {
                if ((ReactorTemp > 100)
                    || (SeperatorOn)
                    || (PumpOn)
                    || (HeaterOn)
                    || (MethaneStoreLevel > (storeFull - storeTolerance))
                    )
                {
                    Trouble();
                }
                else
                {
                    SimulateStandby();

                    // if stores are getting full, change to processing state
                    if ((H2StoreLevel > 75) && (Co2StoreLevel > 75))
                    {
                        Status = SystemStatus.Processing;
                    }
                }
            }
        }

        private void Trouble()
        {
            Status = SystemStatus.Trouble;

            // stop system so reactants don't build up or damage components from 'dry' operation
            TurnOff();
        }

        private void SimulateProcessing()
        {
            if(MethaneStoreLevel < (storeFull - storeTolerance))
            {
                MethaneStoreLevel += 5;
            }
            else
            {
                // simulate venting methane
                MethaneStoreLevel = storeEmpty;
            }

            // simulate removal of reactant gasses to reaction  
            // TODO: fix subtraction rates to be more in line with reaction consumption ratio
            if (H2StoreLevel > storeEmpty)
            {
                H2StoreLevel -= 4;
            }
            if (Co2StoreLevel > storeEmpty)
            {
                Co2StoreLevel -= 2;
            }

        }

        private void SimulateStandby()
        {
            // turn system 'off'
            TurnOff();

            // empty methane store if > 50%
            if (MethaneStoreLevel > 50)
            {
                MethaneStoreLevel = storeEmpty;
            }

            // simulate addition of reactant gasses to stores  
            // TODO: fix addition rates to be more in line with oxygen generation production ratio
            if(H2StoreLevel < storeFull)
            {
                H2StoreLevel += 4;
            }
            if(Co2StoreLevel < storeFull)
            {
                Co2StoreLevel += 2;
            }
        }

        private void TurnOff()
        {
            SeperatorOn = false;
            PumpOn = false;
        }

        #endregion Methods


        #region Check Alerts

        #endregion Check Alerts


        #region Equality Members

        #endregion Equality Members
    }
}
