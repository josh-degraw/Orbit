using System;

namespace Orbit.Util
{
    public class ValueOutOfSafeRangeEventArgs : EventArgs
    {
        public ValueOutOfSafeRangeEventArgs(BoundedValue value)
        {
            this.Value = value;
        }

        public BoundedValue Value { get; }
    }
}