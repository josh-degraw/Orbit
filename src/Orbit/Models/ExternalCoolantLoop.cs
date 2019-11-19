using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public class ExternalCoolantLoop
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// indicator of overall system status (ie: Ready, Processing, Failure...)
        /// </summary> 
        public string SystemStatus { get; set; }

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public string PumpStatus { get; set; }

        /// <summary>
        /// position in degrees from neutral point. 
        /// </summary>
        public int RadiatorRotation { get; set; }

        /// <summary>
        /// pressure of fluid in the lines
        /// </summary>
        public int Pressure { get; set; }

        /// <summary>
        /// temperature of fluid leaving internal/external heat exchanger to radiators
        /// </summary>
        public int OutgoingFluidTemp { get; set; }

        /// <summary>
        /// temperature of fluid returning from radiators to internal/external heat exchanger
        /// </summary>
        public int ReturnFluidTemp { get; set; }

        /// <summary>
        /// contains repleneshment fluid and serves as additional resevoir for expanded fluid to go (increase of pressure),
        /// and draw from when fluid contracts (decrease pressure)
        /// </summary>
        public int TankLevel { get; set; }
    }
}
