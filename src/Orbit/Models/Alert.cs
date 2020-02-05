using System.Collections.Generic;
using System.Linq.Expressions;

namespace Orbit.Models
{
    public class Alert
    {
        // TODO: Creator methods, e.g. TooHigh(msg), TooLow(msg), WayTooHigh(msg), etc.?

        public Alert(string propertyName, string message, AlertLevel level)
        {
            this.PropertyName = propertyName;
            this.Message = message;
            this.AlertLevel = level;
        }

        public string PropertyName { get; }

        public string Message { get; }

        public AlertLevel AlertLevel { get; }

        /// <summary>
        ///   Optionally specify additional data via key-value pairs.
        /// </summary>
        public IDictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public static Alert Safe(string propertyName) => new Alert(propertyName, "", AlertLevel.Safe);

        public bool IsDefault() => this.Message.Length == 0 && this.AlertLevel == AlertLevel.Safe;

        public override string ToString() => $"({PropertyName} {AlertLevel}): {Message}";
    }
}