using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class OxygenGenerator : IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "OxygenGenerator";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// state of system; standby, processing, fail, etc
        /// </summary>
        public string OxygenGeneratorStatus { get; set; } = "StandBy";

        /// <summary>
        /// sensor that checks for bubbles in inflow water, water is sent back to water processor if bubbles are present
        /// </summary>
        public bool BubblesPresent { get; set; }

        /// <summary>
        /// switches water from going to reaction to water processor if bubbles are present
        /// </summary>
        public DiverterValvePositions DiverterValvePosition { get; set; }
        
        /// <summary>
        /// checks if hydrogen is present in product oxygen flow, if yes then there is a problem in the system and it 
        /// shuts down
        /// </summary>
        public bool HydrogenInOxygenFlow { get; set; }

        /// <summary>
        /// Separates hydrogen from water outflow, water is recirculated back into water generator
        /// hydrogen is sent to water generator or vented to space
        /// </summary>
        public bool RotarySeparatorOn { get; set; }

        /// <summary>
        /// circulates water from clean water feed and rotary separator to electrolysis cell stack
        /// </summary>
        public bool RecirculationPumpOn { get; set; }

        private IEnumerable<Alert> CheckHydrogenInOxygenFlow()
        {
            if (HydrogenInOxygenFlow)
            {
                yield return new Alert(nameof(OxygenGenerator), "Hydrogen detected in outflow", AlertLevel.HighError);
            }
            else
            {
                yield return Alert.Safe(nameof(OxygenGenerator));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckHydrogenInOxygenFlow();
        }
    }
}