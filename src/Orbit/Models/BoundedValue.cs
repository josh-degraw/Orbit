namespace Orbit.Models
{
    public readonly struct BoundedValue
    {
        public BoundedValue(double value, bool isSafe)
        {
            this.Value = value;
            this.IsSafe = isSafe;
        }

        public double Value { get; }

        public bool IsSafe { get; }
    }
}