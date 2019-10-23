using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Orbit.Models
{
    public interface IReport
    {
        double CurrentValue { get; }
        DateTimeOffset ReportDateTime { get; }
        ReportType ReportType { get; }
    }

    public enum ReportType
    {
        None,
        Battery
    }

    public abstract class ReportBase : IReport
    {
        protected ReportBase(DateTimeOffset reportDateTime, double currentValue)
        {
            this.ReportDateTime = reportDateTime;
            this.CurrentValue = currentValue;
        }
        
        [Column]
        public virtual DateTimeOffset ReportDateTime { get; private set; }
        
        [Column]
        public virtual double CurrentValue { get; private set; }

        [Column]
        public abstract ReportType ReportType { get; }
        
        public Limit Limit { get; set; }
    }

    public class BatteryReport : ReportBase
    {
        public BatteryReport(DateTimeOffset reportDateTime, double currentValue) : base(reportDateTime, currentValue)
        {
        }

        public override ReportType ReportType { get; } = ReportType.Battery;

    }
}