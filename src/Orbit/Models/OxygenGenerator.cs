using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    public class OxygenGenerator: IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "CarbonDioxideRemediation";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// state of system; standby, processing, fail, etc
        /// </summary>
        public string OxygenGeneratorStatus { get; set; }

        /// <summary>
        /// seperats water from hydrogen. vents to carbon dioxide remediator, excess vents to space
        /// </summary>
        public string RotarySeperatorStatus { get; set; }

        /// <summary>
        /// elctrolysis unit responsible for seperating water into oxygen (vent to cabin) and
        /// hydrogen (used for CO2 remediation, excess vented to space)
        /// </summary>
        public double CellStackVoltage { get; set; }

        public double RecirculationFlowPressure { get; set; }

        /// <summary>
        /// circulates water from clean water feed and rotary seperator to electrolysis cell stack
        /// </summary>
        public string RecirculationPumpStatus { get; set; }

        /// <summary>
        /// 'ACTEX' unit, balances pH of recirculating water 
        /// </summary>
        public string pHBalancerStatus { get; set; }

        /// <summary>
        /// pH of water after leaving the pH balancing ACTEX unit
        /// </summary>
        public double RecirculatingWaterPhLevel { get; set; }


        public IEnumerable<Alert> GenerateAlerts()
        {
            yield break;
        }
    }
}
