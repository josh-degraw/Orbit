using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class Limit
    {
        public Limit(Guid id, double upperErrorLimit, double lowerErrorLimit, double warningTolerance)
        {
            this.Id = id;
            this.UpperErrorLimit = upperErrorLimit;
            this.LowerErrorLimit = lowerErrorLimit;
            this.WarningTolerance = Math.Abs(warningTolerance);
        }

        public Guid Id { get; private set; }

        [Required]
        [DefaultValue(double.MaxValue)]
        public double UpperErrorLimit { get; private set; }

        [Required]
        [DefaultValue(double.MinValue)]
        public double LowerErrorLimit { get; private set; }

        [Range(0, double.MaxValue)]
        [DefaultValue(0)]
        public double WarningTolerance { get; private set; }

        [NotMapped]
        public double UpperWarningLevel => this.UpperErrorLimit - this.WarningTolerance;

        [NotMapped]
        public double LowerWarningLevel => this.UpperErrorLimit + this.WarningTolerance;

    }
}