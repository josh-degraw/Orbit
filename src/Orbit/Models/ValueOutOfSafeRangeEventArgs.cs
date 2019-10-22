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
    }
}