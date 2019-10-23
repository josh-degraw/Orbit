using System;

namespace Orbit.Models
{
    public readonly struct BoundedValue
    {
        private BoundedValue(double value, AlertLevel alertAlertLevel)
        {
            this.Value = value;
            this.AlertLevel = alertAlertLevel;
        }

        /// <summary>
        /// The reported value
        /// </summary>
        public double Value { get; }
        
        public AlertLevel AlertLevel { get; }

        public bool IsSafe => this.AlertLevel == AlertLevel.Safe;

        public static BoundedValue Create(double currentValue, Limit range)
        {
            if(currentValue >= range.UpperErrorLimit)
            {
                return new BoundedValue(currentValue, AlertLevel.HighError);
            }
            if(currentValue >= range.UpperWarningLevel)
            {
                return new BoundedValue(currentValue, AlertLevel.HighWarning);
            }
            if(currentValue < range.LowerErrorLimit)
            {
                return new BoundedValue(currentValue, AlertLevel.LowError);
            }
            if(currentValue <= range.LowerWarningLevel)
            {
                return new BoundedValue(currentValue, AlertLevel.LowWarning);
            }

            return new BoundedValue(currentValue, AlertLevel.Safe);
        }
    }

    public enum AlertLevel
    {
        Safe,
        HighWarning,
        LowWarning,
        HighError,
        LowError,
    }
}