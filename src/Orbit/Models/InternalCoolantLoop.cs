using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public class InternalCoolantLoop
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public string PumpStatus { get; set; }

        /// <summary>
        /// pressure of fluid in the lines
        /// </summary>
        public int Pressure { get; set; }

        /// <summary>
        /// temperature of fluid leaving cabin to internal/external heat exchanger
        /// </summary>
        public int OutgoingFluidTemp { get; set; }

        /// <summary>
        /// temperature of fluid returning to cabin from internal/external heat exchanger
        /// </summary>
        public int ReturnFluidTemp { get; set; }

        /// <summary>
        /// contains repleneshment fluid and serves as additional resevoir for expanded fluid to go (increase of pressure),
        /// and draw from when fluid contracts (decrease pressure)
        /// </summary>
        public int ReserveTankLevel { get; set; }
    }
}
