using System.ComponentModel.DataAnnotations;

namespace Orbit.Models
{
    public class RangedAlert : Alert
    {
        public RangedAlert(string propertyName, string message, AlertLevel level, RangeAttribute range) : base(propertyName, message, level)
        {
            this._range = range;
            this.Data["Min"] = range.Minimum;
            this.Data["Max"] = range.Maximum;
        }

        private readonly RangeAttribute _range;

        public object Minimum => this._range.Minimum;

        public object Maximum => this._range.Maximum;

        public bool IsValid(object value) => this._range.IsValid(value);
    }
}