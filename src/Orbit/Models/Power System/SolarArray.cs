using System;
using System.Collections.Generic;

namespace Orbit.Models
{
    public class SolarArray : IAlertableModel
    {
        public double Voltage { get; set; }

        //In Kilowatts
        public double Power { get; set; }

        public bool Deployed { get; set; }

        public string ComponentName => "SolarArray";

        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            yield break;
        }
    }
}