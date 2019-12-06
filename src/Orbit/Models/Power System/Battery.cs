 using System;
 using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 using System.ComponentModel.DataAnnotations.Schema;
 using System.Linq;

namespace Orbit.Models
{
    public class Battery: IAlertableModel
    {
        /// <summary>
        /// is true if solar array output is ~160V and battery is charging, false if not
        /// </summary>
        public bool BatteryCharging { get; set; }

        /// <summary>
        /// Range is space variance should cooling system fail
        /// Nominal: 5C +- 5C
        /// </summary>
        [Range(-157, 121)]
        public int Temperature { get; set; }

        [NotMapped]
        public int temperatureUpperLimit = 15;
        [NotMapped]
        public int temperatureLowerLimit = -15;
        [NotMapped]
        public int temperatureTolerance = 5;

        /// <summary>
        /// operate to a 35% depth of discharge (dod)  which is opposite of normal charge%, 0% = fully charged, 100% would be fully discharged
        /// 35% dod would =     
        /// charge from 160v source
        /// charge algorithm based on state of charge pressure and temperature
        /// </summary>
        [Range(0, 105)]
        public double ChargeLevel { get; set; }

        [NotMapped]
        public int chargeLevelUpperLimit = 105;
        [NotMapped]
        public int chargeLevelLowerLimit = 60;
        [NotMapped]
        public int chargeLevelTolerance = 5;

        /// <summary>
        /// Nominal is 160v recieved from solar array
        /// </summary>
        [Range(0, 170)]
        public double Voltage { get; set; }

        [NotMapped]
        public int voltageUpperLimit = 165;
        [NotMapped]
        public int voltageLowerLimit = 155;
        [NotMapped]
        public int voltageTolerance = 2;

    private IEnumerable<Alert> CheckTemperature()
        {
            if(Temperature >= temperatureUpperLimit)
            {
                yield return new Alert(nameof(Temperature), "Temperatue is above maximum", AlertLevel.HighError);
            }
            else if(Temperature >= (Temperature - temperatureTolerance))
            {
                yield return new Alert(nameof(Temperature), "Temperature is elevated", AlertLevel.HighWarning);
            }
            else if(Temperature <= temperatureLowerLimit)
            {
                yield return new Alert(nameof(Temperature), "Temperature is below minimum", AlertLevel.LowError);
            }
            else if(Temperature <= (temperatureLowerLimit + temperatureTolerance))
            {
                yield return new Alert(nameof(Temperature), "Temperature is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(Temperature));
            }
        }

        private IEnumerable<Alert> CheckChargeLevel()
        {
            if (ChargeLevel >= chargeLevelUpperLimit)
            {
                yield return new Alert(nameof(ChargeLevel), "Charge level is above maximum", AlertLevel.HighError);
            }
            else if (ChargeLevel >= (ChargeLevel - chargeLevelTolerance))
            {
                yield return new Alert(nameof(ChargeLevel), "Charge level is elevated", AlertLevel.HighWarning);
            }
            else if (ChargeLevel <= chargeLevelLowerLimit)
            {
                yield return new Alert(nameof(ChargeLevel), "Charge level is below minimum", AlertLevel.LowError);
            }
            else if (ChargeLevel <= (chargeLevelLowerLimit + chargeLevelTolerance))
            {
                yield return new Alert(nameof(ChargeLevel), "Charge level is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(ChargeLevel));
            }
        }

        private IEnumerable<Alert> CheckBattery()
        {
            if(Voltage >= voltageUpperLimit)
            {
                yield return new Alert(nameof(Battery), "Battery voltage is above maximum", AlertLevel.HighError);
            }
            else if(Voltage >= (voltageUpperLimit - voltageTolerance))
            {
                yield return new Alert(nameof(Battery), "Battery voltage is elevated", AlertLevel.HighWarning);
            }
            else if(Voltage <= voltageLowerLimit)
            {
                yield return new Alert(nameof(Battery), "Battery voltage is below minimum", AlertLevel.LowError);
            }
            else if(Voltage <= (voltageLowerLimit + voltageTolerance))
            {
                yield return new Alert(nameof(Battery), "Battery voltage is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(Battery));
            }
        }

        [NotMapped]
        public string ComponentName => "Battery";
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckBattery().Concat(CheckChargeLevel()).Concat(CheckTemperature());
        }
    }
}
