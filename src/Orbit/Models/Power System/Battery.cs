 using System;
using System.Collections.Generic;
 using System.ComponentModel.DataAnnotations.Schema;
 using System.Text;

namespace Orbit.Models
{
    public class Battery: IAlertableModel
    {
        public double Temperature { get; set; }
        public double ChargeLevel { get; set; }

        [NotMapped]
        public string ComponentName => "Battery";
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            yield break;
        }
    }
}
