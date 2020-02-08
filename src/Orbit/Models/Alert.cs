using System.Collections.Generic;

namespace Orbit.Models
{
    public class Alert
    {
        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        internal Alert(string propertyName, string message, AlertLevel level, PropertyMetadata? information = null, object? currentValue = null)
        {
            this.PropertyName = propertyName;
            this.Message = message;
            this.AlertLevel = level;
            this.Metadata = information ?? new PropertyMetadata();
            this.CurrentValue = currentValue;
        }

        public string PropertyName { get; }

        public string Message { get; }

        public AlertLevel AlertLevel { get; }

        public PropertyMetadata Metadata { get; }

        /// <summary>
        /// The value of the relevant property at the time of the alert.
        /// </summary>
        public object? CurrentValue { get; }

        /// <summary>
        ///   Optionally specify additional data via key-value pairs.
        /// </summary>
        public IReadOnlyDictionary<string, object> Data => _data;

        public static Alert Safe(string propertyName) => new Alert(propertyName, "", AlertLevel.Safe);

        public bool IsSafe => this.Message.Length == 0 && this.AlertLevel == AlertLevel.Safe;

        public override string ToString() => $"({PropertyName} {AlertLevel}): {Message}";

        internal Alert WithData(string key, object value)
        {
            _data.Add(key, value);
            return this;
        }

        internal Alert WithData(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var item in data)
            {
                _data[item.Key] = item.Value;
            }
            return this;
        }
    }
}