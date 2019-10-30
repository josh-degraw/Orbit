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
        public abstract string ReportType { get; protected set; }
        
        [Column, ForeignKey(nameof(Limit))]
        public Guid LimitId { get; set; }

        public Limit? Limit { get; set; }
    }
}