using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Orbit.Models
{
    public class InternalCoolantLoopData : IAlertableModel
    {
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// warmer coolant loop for experiments and avionics
        /// nominal is 10.5C
        /// </summary>
        [Range(0, 47)]
        public double TempMedCoolantLoop { get; set; }

        /// <summary>
        /// colder coolant loop for life support and, cabin air assembly, and some experiments
        /// nominal is 17C
        /// </summary>
        [Range(0, 47)]
        public double TempLowCoolantLoop { get; set; }


        [NotMapped]
        public string ComponentName = "InternalCoolantSystem";

        [NotMapped]
        private double _alertTolerance = 5;
        [NotMapped]
        private double _alarmTolerance = 10;
        [NotMapped]
        private double _lowTempLoopNominal = 10.5;
        [NotMapped]
        private double _medTempLoopNominal = 17;


        private IEnumerable<Alert> CheckLowTempLoopStatus()
        {
            if(TempLowCoolantLoop >= (TempLowCoolantLoop + _alarmTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is above maximum", AlertLevel.HighError);
                PumpOn = false;
            }
            else if (TempLowCoolantLoop >= (TempLowCoolantLoop + _alertTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is high", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else if(TempLowCoolantLoop <=(TempLowCoolantLoop - _alertTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is low", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else if(TempLowCoolantLoop <= (TempLowCoolantLoop - _alarmTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is below minimum", AlertLevel.HighError);
                PumpOn = false;
            }
            else
            {
                yield return new Alert.Safe(nameof(TempLowCoolantLoop));
                PumpOn = true;
            }
        }

        private IEnumerable<Alert> CheckMedTempLoopStatus()
        {
            if (TempMedCoolantLoop >= (TempMedCoolantLoop + _alarmTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is above maximum", AlertLevel.HighError);
                PumpOn = false;
            }
            else if (TempMedCoolantLoop >= (TempMedCoolantLoop + _alertTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is high", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else if (TempMedCoolantLoop <= (TempMedCoolantLoop - _alertTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is low", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else if (TempMedCoolantLoop <= (TempMedCoolantLoop - _alarmTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is below minimum", AlertLevel.HighError);
                PumpOn = false;
            }
            else
            {
                yield return new Alert.Safe(nameof(TempMedCoolantLoop));
                PumpOn = true;
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckLowTempLoopStatus().Concat(CheckMedTempLoopStatus());
        }
    }
}
