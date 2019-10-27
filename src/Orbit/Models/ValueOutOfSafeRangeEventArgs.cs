using System;

namespace Orbit.Models
{
    public class ValueOutOfSafeRangeEventArgs : EventArgs
    {
        public ValueOutOfSafeRangeEventArgs(CurrentValueReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));

            this.ComponentName = report.ComponentName;
            this.Value = report.Value;
        }

        public string ComponentName { get; }

        public BoundedValue Value { get; }

        public void Deconstruct(out string componentName, out BoundedValue value) => (componentName, value) = (ComponentName, Value);
    }
}