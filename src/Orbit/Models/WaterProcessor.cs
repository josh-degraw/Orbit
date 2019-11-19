using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public class WaterProcessor
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// draws water from dirty storage tank and pushes into the water processing system
        /// </summary>        
        public string SupplyPump { get; set; }

        /// <summary>
        /// This sensor is located between two identical filter beds. It will trigger if contaminates are detected 
        /// which indicates the first filter bed is saturated and needs to be changed by personal. 
        /// The second filter is then moved to the first position and a new filter is then installed into the second filter position. 
        /// </summary>
        public string PostFilterContaminateSensor { get; set; }

        /// <summary>
        /// warms incoming water before entering the reactor
        /// </summary>
        public int PreHeaterTemp { get; set; }

        /// <summary>
        /// uses heat and oxygen to oxidize any remaining organic contaminates
        /// </summary>
        public int CatalyticReactorTemp { get; set; }

        /// <summary>
        /// uses conductivity to measure the amount of organic contaminates after water has been treated by the reactor
        /// </summary>
        public string ReactorHealthSensor { get; set; }

        /// <summary>
        /// will divert water to product tank if contaminate and Reactor health sensors pass, or back into
        /// process line if either sensor indicates failure/contaminates
        /// </summary>
        public string ReprocessDiverterValve { get; set; }

        /// <summary>
        /// Stores clean water ready for consumption
        /// </summary>
        public int ProductTankLevel { get; set; }

        /// <summary>
        /// moves potable water to drinkin dispenser and various other systems?
        /// </summary>
        public string DeliveryPump { get; set; }
  
    }
}
