using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class WaterProcessorData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        /// <summary>
        /// distinguish between seperate redundant processors ('MainProcessor', 'Backup 1')
        /// </summary>
        public string ProcessorId { get; set; }

        /// <summary>
        /// indicator of overall system status (Ready, Processing, Failure...)
        /// </summary>
        public string SystemStatus { get; set; }

        /// <summary>
        /// draws water from dirty storage tank and pushes into the water processing system
        /// </summary>
        public string SupplyPump { get; set; }

        /// <summary>
        /// This sensor is located between two identical filter beds. It will trigger if contaminates are detected which
        /// indicates the first filter bed is saturated and needs to be changed by personal. The second filter is then
        /// moved to the first position and a new filter is then installed into the second filter position.
        /// </summary>
        public string PostFilterContaminateSensor { get; set; }

        /// <summary>
        /// warms incoming water before entering the reactor
        /// </summary>
        public double PreHeaterTemp { get; set; }

        /// <summary>
        /// uses heat and oxygen to oxidize any remaining organic contaminates
        /// </summary>
        public double CatalyticReactorTemp { get; set; }

        /// <summary>
        /// uses conductivity to measure the amount of organic contaminates after water has been treated by the reactor
        /// </summary>
        public string ReactorHealthSensor { get; set; }

        /// <summary>
        /// will divert water to product tank if contaminate and Reactor health sensors pass, or back into process line
        /// if either sensor indicates failure/contaminates
        /// </summary>
        public string ReprocessDiverterValve { get; set; }

        /// <summary>
        /// Stores clean water ready for consumption
        /// </summary>
        public double ProductTankLevel { get; set; }

        /// <summary>
        /// moves potable water to drinkin dispenser and various other systems?
        /// </summary>
        public string DeliveryPump { get; set; }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            //TODO: Implement
            yield break;
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "WaterProcessor";

        #endregion Implementation of IModuleComponent
    }
}