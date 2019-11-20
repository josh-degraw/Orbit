using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    public class Atmosphere: IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "Atmosphere";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;
       
        /// <summary>
        /// general status of lifesupport and environment as a whole
        /// </summary>
        public string CabinStatus { get; set; }

        public double CabinPressure { get; set; }

        /// <summary>
        /// link to Oxygen Generation System
        /// </summary>
        public double CabinOxygenLevel { get; set; }

        /// <summary>
        /// link to Carbon Dioxide Removal System
        /// </summary>
        public double CabinCarbonDioxideLevel { get; set; }

        /// <summary>
        /// not sure this will be a seperate system...
        /// </summary>
        public double CabinHumidityLevel { get; set; }

        /// <summary>
        /// link to detailed breakdown of type/quantity of particulates?
        /// </summary>
        public double CabinAirParticulatesLevel { get; set; }

        /// <summary>
        /// decibal value of cabin noise
        /// </summary>
        public double CabinAmbientNoiseLevel { get; set; }

        public IEnumerable<Alert> GenerateAlerts()
        {
            yield break;
        }
    }
}
