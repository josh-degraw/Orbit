using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
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
}