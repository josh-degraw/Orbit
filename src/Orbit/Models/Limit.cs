using System.ComponentModel;

namespace Orbit.Models
{
    public class Limit
    {
        public Limit(int id, double upperLimit, double lowerLimit)
        {
            this.Id = id;
            this.UpperLimit = upperLimit;
            this.LowerLimit = lowerLimit;
        }

        public int Id { get; }

        [DefaultValue(double.MaxValue)]
        public double UpperLimit { get; }

        [DefaultValue(double.MinValue)]
        public double LowerLimit { get; }
    }
}