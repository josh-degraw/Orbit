using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class BatteryReport
    {
        public BatteryReport(Guid id, double currentVoltage)
        {
            this.Id = id;
            this.CurrentVoltage = currentVoltage;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; }

        public double CurrentVoltage { get; }

        public DateTimeOffset ReportDateTime { get; set; }

        [Required]
        public Limit Limit { get; set; }
    }
}