namespace Orbit.Models
{
    public sealed class CurrentValueReport
    {
        public CurrentValueReport(string componentName, IBoundedReport report, BoundedValue value)
        {
            this.ComponentName = componentName;
            this.Report = report;
            this.Value = value;
        }

        public string ComponentName { get; }
        public IBoundedReport Report { get; }
        public BoundedValue Value { get; }

        public override string ToString() => $"{ComponentName} - {Value}";
    }
}