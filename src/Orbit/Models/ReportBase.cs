using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
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
        
        public Limit? Limit { get; set; }
    }
}