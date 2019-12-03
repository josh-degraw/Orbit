using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    public class ShuntUnit: IAlertableModel
    {
        public double InputVoltage { get; set; }
        public double OutputVoltage { get; set; }
        
        [NotMapped]
        public string ComponentName => "ShuntUnit";

        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            yield break;
        }
    }
}
