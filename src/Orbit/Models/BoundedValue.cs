using System;

namespace Orbit.Models
{
    public readonly struct BoundedValue : IEquatable<BoundedValue>, IComparable<BoundedValue>
    {
        public BoundedValue(double value, AlertLevel alertAlertLevel)
        {
            this.Value = value;
            this.AlertLevel = alertAlertLevel;
        }

        /// <summary>
        /// The reported value
        /// </summary>
        public double Value { get; }

        public AlertLevel AlertLevel { get; }

        public override string ToString() => $"{Value:N2} ({AlertLevel})";

        public bool IsSafe => this.AlertLevel == AlertLevel.Safe;

        public static BoundedValue Create(double currentValue, Limit range)
        {
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            if (currentValue >= range.UpperErrorLimit)
            {
                return new BoundedValue(currentValue, AlertLevel.HighError);
            }
            if (currentValue >= range.UpperWarningLevel)
            {
                return new BoundedValue(currentValue, AlertLevel.HighWarning);
            }
            if (currentValue < range.LowerErrorLimit)
            {
                return new BoundedValue(currentValue, AlertLevel.LowError);
            }
            if (currentValue <= range.LowerWarningLevel)
            {
                return new BoundedValue(currentValue, AlertLevel.LowWarning);
            }

            return new BoundedValue(currentValue, AlertLevel.Safe);
        }

        #region Equality and comparison members

        public bool Equals(BoundedValue other) => this.Value.Equals(other.Value) && this.AlertLevel == other.AlertLevel;

        public int CompareTo(BoundedValue other) => this.AlertLevel.CompareTo(other.AlertLevel) + this.Value.CompareTo(other.Value);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            return this.Equals((BoundedValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Value.GetHashCode() * 397) ^ (int)this.AlertLevel;
            }
        }

        public static bool operator ==(BoundedValue left, BoundedValue right) => left.Equals(right);

        public static bool operator !=(BoundedValue left, BoundedValue right) => !left.Equals(right);

        public static bool operator <(BoundedValue left, BoundedValue right) => left.Value < right.Value;

        public static bool operator >(BoundedValue left, BoundedValue right) => left.Value > right.Value;

        public static bool operator <=(BoundedValue left, BoundedValue right) => left.Value <= right.Value;

        public static bool operator >=(BoundedValue left, BoundedValue right) => left.Value >= right.Value;

        #endregion Equality and comparison members
    }
}