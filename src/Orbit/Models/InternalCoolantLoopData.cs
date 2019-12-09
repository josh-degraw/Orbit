﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class InternalCoolantLoopData : IAlertableModel
    {
        #region Limits

        private const double medCoolantLoopUpperLimit = 22;
        private const double medCoolantLoopLowerLimit = 12;
        private const double medCoolantLoopTolerance = 5;

        private const double lowTempCoolantLoopUpperLimit = 10;
        private const double lowTempCoolantLoopLowerLimit = 2;
        private const double lowTempCoolantLoopTolerance = 2;

        #endregion Limits

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// warmer coolant loop for experiments and avionics nominal is 10.5C
        /// </summary>
        [Range(0, 47)]
        public double TempMedCoolantLoop { get; set; }

        /// <summary>
        /// colder coolant loop for life support and, cabin air assembly, and some experiments nominal is 4C
        /// </summary>
        [Range(0, 47)]
        public double TempLowCoolantLoop { get; set; }

        #region IAlertableModel implementation

        [NotMapped]
        public string ComponentName => "InternalCoolantSystem";

        private IEnumerable<Alert> CheckLowTempLoopStatus()
        {
            if (TempLowCoolantLoop >= lowTempCoolantLoopUpperLimit)
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is above maximum", AlertLevel.HighError);
            }
            else if (TempLowCoolantLoop >= (lowTempCoolantLoopUpperLimit - lowTempCoolantLoopTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is high", AlertLevel.HighWarning);
            }
            else if (TempLowCoolantLoop <= lowTempCoolantLoopLowerLimit)
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is low", AlertLevel.LowError);
            }
            else if (TempLowCoolantLoop <= (lowTempCoolantLoopLowerLimit + lowTempCoolantLoopTolerance))
            {
                yield return new Alert(nameof(TempLowCoolantLoop), "Low coolant loop temperature is below minimum", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(TempLowCoolantLoop));
            }
        }

        private IEnumerable<Alert> CheckMedTempLoopStatus()
        {
            if (TempMedCoolantLoop >= medCoolantLoopUpperLimit)
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is above maximum", AlertLevel.HighError);
            }
            else if (TempMedCoolantLoop >= (medCoolantLoopUpperLimit - medCoolantLoopTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is high", AlertLevel.HighWarning);
            }
            else if (TempMedCoolantLoop <= medCoolantLoopLowerLimit)
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is low", AlertLevel.LowError);
            }
            else if (TempMedCoolantLoop <= (medCoolantLoopLowerLimit + medCoolantLoopTolerance))
            {
                yield return new Alert(nameof(TempMedCoolantLoop), "Med coolant loop temperature is below minimum", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(TempMedCoolantLoop));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckLowTempLoopStatus()
                .Concat(this.CheckMedTempLoopStatus());
        }

        #endregion IAlertableModel implementation
    }
}