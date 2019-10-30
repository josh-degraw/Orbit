using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{


    public class BatteryReport : ReportBase
    {
        public BatteryReport(DateTimeOffset reportDateTime, double currentValue) : base(reportDateTime, currentValue)
        {
        }

        public override string ReportType { get; protected set; } = "Battery";
    }

}