using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class UrineSystemData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        /// <summary>
        /// Indicator of overall system status (Ready, Processing, Failure...)
        /// </summary>
        public SystemStatus SystemStatus { get; set; }

        /// <summary>
        /// Fullness of treated weewee holding tank as a percentage
        /// </summary>
        [Range(0, 100)]
        public int UrineTankLevel { get; set; }

        private const int urineTankUpperLimit = 100;
        private const int urineTankLevelTolerance = 5;

        /// <summary>
        /// status of pump assembly used to pull fluid from weewee tank to the distiller assembly then from distiller
        /// assembly to the brine tank and water processor
        /// </summary>
        public bool SupplyPumpOn { get; set; }

        /// <summary>
        /// Turning distiller on or off also turns on/off distiller motor and heater
        /// </summary>
        public bool DistillerOn { get; set; }

        /// <summary>
        /// Motor speed; nominal 1200 rpm
        /// </summary>
        [Range(0, 1400)]
        public int DistillerSpeed { get; set; }

        private const int distillerSpeedUpperLimit = 1300;
        private const int distillerSpeedLowerLimit = 1100;
        private const int distillerSpeedTolerance = 100;

        /// <summary>
        /// Temp of weewee in the distiller; nominal 45C
        /// </summary>
        [Range(0, 60)]
        public double DistillerTemp { get; set; }

        private const double distillerTempUpperLimit = 55;
        private const double distillerTempLowerlimit = 35;
        private const double distillerTempTolerance = 5;

        /// <summary>
        /// Routes distillate and gasses from distiller to gas/liquid separator cooled assembly aids condensation of
        /// water from gas
        /// </summary>
        public bool PurgePumpOn { get; set; }

        /// <summary>
        /// Stores concentrated minerals and contaminates from weewee distillation process for later disposal shown as percentage
        /// </summary>
        [Range(0, 100)]
        public int BrineTankLevel { get; set; }

        private const int brineTankLevelUpperLimit = 100;
        private const int brineTankLevelTolerance = 5;

        public void ProcessData(double wasteTankLevel, double temp, int speed)
        {
            if (SystemStatus == SystemStatus.Standby)
            {
                DistillerTemp = temp;
                DistillerSpeed = speed;

                if ((UrineTankLevel > 80) && (wasteTankLevel < 100) && (BrineTankLevel < 100))
                {
                    SystemStatus = SystemStatus.Processing;
                    UrineTankLevel -= 5;
                    SupplyPumpOn = true;
                    DistillerOn = true;
                    PurgePumpOn = true;
                    BrineTankLevel += 2;
                }
                else
                {
                    UrineTankLevel += 3;
                }
            }
            else if (SystemStatus == SystemStatus.Processing)
            {
                DistillerTemp = temp;
                DistillerSpeed = speed;

                if ((UrineTankLevel <= 0) || (wasteTankLevel >= 100) || (BrineTankLevel >= 100))
                {
                    SystemStatus = SystemStatus.Standby;
                    SupplyPumpOn = false;
                    DistillerOn = false;
                    PurgePumpOn = false;

                    if (UrineTankLevel <= 0)
                    {
                        UrineTankLevel = 0;
                    }
                    if (BrineTankLevel >= 100)
                    {
                        BrineTankLevel = 100;
                    }
                }
                else
                {
                    UrineTankLevel -= 5;
                    BrineTankLevel += 2;
                }
            }
            else
            {
                SystemStatus = SystemStatus.Standby;
                SupplyPumpOn = false;
                DistillerOn = false;
                DistillerTemp = temp;
                DistillerSpeed = speed;
                PurgePumpOn = false;
            }
        }

        #region CheckValueMethods

        private IEnumerable<Alert> CheckUrineTankLevel()
        {
            // system should start/be processing if wastetank < 95%, else shut down
            if (UrineTankLevel >= urineTankUpperLimit)
            {
                yield return new Alert(nameof(UrineTankLevel), "Urine tank level is at capacity", AlertLevel.HighError);
            }
            else if (UrineTankLevel >= (urineTankUpperLimit - urineTankLevelTolerance))
            {
                yield return new Alert(nameof(UrineTankLevel), "Urine tank level is nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(UrineTankLevel));
            }
        }

        private IEnumerable<Alert> CheckDistillerSpeed()
        {
            if (DistillerSpeed >= distillerSpeedUpperLimit)
            {
                //TODO: shut down system
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed above maximum", AlertLevel.HighError);
            }
            else if (DistillerSpeed > (distillerSpeedUpperLimit - distillerSpeedTolerance))
            {
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is too high", AlertLevel.HighWarning);
            }
            else if (DistillerOn && (DistillerSpeed < distillerSpeedLowerLimit))
            {
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is below minimum", AlertLevel.LowError);
            }
            else if (DistillerOn && (DistillerSpeed <= (distillerSpeedLowerLimit + distillerSpeedTolerance)))
            {
                // TODO: shut down system
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(DistillerSpeed));
            }
        }

        private IEnumerable<Alert> CheckDistillerTemp()
        {
            if (DistillerTemp >= distillerTempUpperLimit)
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller temp above maximum", AlertLevel.HighError);
            }
            else if (DistillerTemp > (distillerTempUpperLimit - distillerTempTolerance))
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller temp is too high", AlertLevel.HighWarning);
            }
            else if (DistillerOn && (DistillerTemp < distillerTempLowerlimit))
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller temp is below minimum", AlertLevel.LowError);
            }
            else if (DistillerOn && (DistillerTemp <= (distillerTempLowerlimit + distillerTempTolerance)))
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller Temp is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(DistillerTemp));
            }
        }

        private IEnumerable<Alert> CheckBrineTankLevel()
        {
            if (BrineTankLevel >= brineTankLevelUpperLimit)
            {
                // TODO: shut down system
                yield return new Alert(nameof(BrineTankLevel), "Brine tank is at capacity", AlertLevel.HighError);
            }
            else if (BrineTankLevel >= (brineTankLevelUpperLimit - brineTankLevelTolerance))
            {
                yield return new Alert(nameof(BrineTankLevel), "Brine tank is nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(BrineTankLevel));
            }
        }

        #endregion CheckValueMethods

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckUrineTankLevel()
                .Concat(this.CheckDistillerTemp())
                .Concat(this.CheckDistillerSpeed())
                .Concat(this.CheckBrineTankLevel());
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "UrineSystem";

        #endregion Implementation of IModuleComponent
    }
}