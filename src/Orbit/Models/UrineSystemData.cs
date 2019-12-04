using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class UrineSystemData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        /// <summary>
        /// indicator of overall system status (Ready, Processing, Failure...)
        /// </summary>
        public SystemStatus SystemStatus { get; set; }

        /// <summary>
        /// fullness of treated weewee holding tank as a percentage
        /// </summary>
        [Range(0, 100)]
        public int UrineTankLevel { get; set; }

        [NotMapped]
        public int urineTankUpperAlarm = 100;

        /// <summary>
        /// status of pump assembly used to pull fluid from weewee tank to the distiller assembly then from distiller
        /// assembly to the brine tank and water processor
        /// </summary>
        public bool SupplyPumpOn { get; set; }

        /// <summary>
        /// turining distiller on or off also turns on/off distiller motor and heater
        /// </summary>
        public bool DistillerOn { get; set; }

        /// <summary>
        /// Motor speed; nominal 1200 rpm
        /// </summary>
        [Range(0, 1400)]
        public int DistillerSpeed { get; set; }
        
        [NotMapped]
        public int DistillerSpeedUpperAlarm = 1300;
        [NotMapped]
        public int DistillerSpeedLowerAlarm = 1100;

        /// <summary>
        /// Temp of weewee in the distiller; nominal 45C
        /// </summary>
        [Range(0, 60)]
        public double DistillerTemp { get; set; }
        
        [NotMapped]
        public double DistillerTempUpperAlarm = 50;
        [NotMapped]
        public double DistillerTempLowerAlarm = 40;

        /// <summary>
        /// routes distillate and gasses from distiller to gas/liquid seperator cooled assembly aids condensation of
        /// water from gas
        /// </summary>
        public bool PurgePumpOn { get; set; }

        /// <summary>
        /// stores concentrated minerals and contaminates from weewee distillation process for later disposal
        /// shown as percentage
        /// </summary>
        [Range(0, 100)]
        public int BrineTankLevel { get; set; }

        [NotMapped]
        public int BrineTankLevelUpperAlarm = 100;


        #region CheckValueMethods
        private IEnumerable<Alert> CheckUrineTankLevel()
        {
            if(UrineTankLevel >= 100)  // system should start/be processing if wastetank < 95%, else shut down 
            {
                // TODO: system start
                yield return new Alert(nameof(UrineTankLevel), "Urine tank level is at capacity", AlertLevel.HighError);                
            }
            else if(UrineTankLevel >= 95) 
            {
                // TODO: system start
                yield return new Alert(nameof(UrineTankLevel), "Urine tank level is nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(UrineTankLevel));
            }
        }

        IEnumerable<Alert> CheckDistillerSpeed()
        {
            if (DistillerSpeed >= 1300)
            {
                //TODO: shut down system
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed above maximum", AlertLevel.HighError);
            }
            else if (DistillerSpeed > 1250)
            {
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is too high", AlertLevel.HighWarning);
            }
            else if (DistillerOn && (DistillerSpeed < 1150))
            {
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is too low", AlertLevel.LowWarning);
            }
            else if (DistillerOn && (DistillerSpeed <= 1100))
            {
                // TODO: shut down system
                yield return new Alert(nameof(DistillerSpeed), "Distiller speed is below minimum", AlertLevel.LowError);
            }
            else
            {
                yield return Alert.Safe(nameof(DistillerSpeed));
            }
        }

        IEnumerable<Alert> CheckDistillerTemp()
        {
            if (DistillerTemp >= 50)
            {
                //TODO: shut down system
                yield return new Alert(nameof(DistillerTemp), "Distiller temp above maximum", AlertLevel.HighError);
            }
            else if (DistillerTemp > 45.5)
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller temp is too high", AlertLevel.HighWarning);
            }
            else if (DistillerOn && (DistillerTemp < 45.5))
            {
                yield return new Alert(nameof(DistillerTemp), "Distiller temp is too low", AlertLevel.LowWarning);
            }
            else if (DistillerOn && (DistillerTemp <= 40))
            {   
                // TODO: shut down system
                yield return new Alert(nameof(DistillerTemp), "Distiller Temp is below minimum", AlertLevel.LowError);
            }
            else
            {
                yield return Alert.Safe(nameof(DistillerTemp));
            }
        }

        private IEnumerable<Alert> CheckBrineTankLevel()
        {
            if(BrineTankLevel >= 100)
            {
                // TODO: shut down system
                yield return new Alert(nameof(BrineTankLevel), "Brine tank is at capacity", AlertLevel.HighError);
            }
            else if(BrineTankLevel >= 95)
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
            // Example:
            return CheckUrineTankLevel().Concat(CheckDistillerTemp()).Concat(CheckBrineTankLevel());
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