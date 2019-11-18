using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkTest
{
    public class WaterProcessor
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string SupplyPump { get; set; }
        public string PostFilterContaminateSensor { get; set; }
        public int PreHeaterTemp { get; set; }
        public int CatalyticReactorTemp { get; set; }
        public string ReactorHealthSensor { get; set; }
        public string ReprocessDiverterValve { get; set; }
        public int ProductTankLevel { get; set; }
        public string DeliveryPump { get; set; }
       
        //public virtual ICollection<Alarm> Alarms { get; set; }

    }
}
