using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkTest
{
    public class ExternalCoolantLoop
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string PumpStatus { get; set; }
        public int RadiatorRotation { get; set; }
        public int Pressure { get; set; }
        public int OutgoingFluidTemp { get; set; }
        public int ReturnFluidTemp { get; set; }
        public int TankLevel { get; set; }
    }
}
