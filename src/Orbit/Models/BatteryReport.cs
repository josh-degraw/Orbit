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
        string ReportType { get; }
    }

    public interface IBoundedReport:IReport
    {
        Limit Limit { get; }
    }

    public enum ReportType
    {
        None,
        Battery
    }

    public abstract class ReportBase : IBoundedReport
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
        public abstract string ReportType { get; }
        
        public Limit Limit { get; set; }
    }

    public class BatteryReport : ReportBase
    {
        public BatteryReport(DateTimeOffset reportDateTime, double currentValue) : base(reportDateTime, currentValue)
        {
        }

        public override string ReportType { get; } = "Battery";
    }
}