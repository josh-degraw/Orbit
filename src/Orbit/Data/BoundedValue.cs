namespace Orbit.Util
{
    public readonly struct BoundedValue
    {
        public BoundedValue(string label, double value, bool isSafe)
        {
            this.Label = label;
            this.Value = value;
            this.IsSafe = isSafe;
        }

        public string Label { get; }

        public double Value { get; }

        public bool IsSafe { get; }
    }
}