using System;

namespace Orbit.Models
{
    public class ValueOutOfSafeRangeEventArgs : EventArgs
    {
        public ValueOutOfSafeRangeEventArgs(string componentName, BoundedValue value)
        {
            this.ComponentName = componentName;
            this.Value = value;
        }

        public string ComponentName { get; }
        public BoundedValue Value { get; }

        public void Deconstruct(out string componentName, out BoundedValue value) => (componentName, value) = (ComponentName, Value);

        public static implicit operator ValueOutOfSafeRangeEventArgs((string name, BoundedValue val) report) => new ValueOutOfSafeRangeEventArgs(report.name, report.val);
    }
}