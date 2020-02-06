using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Orbit.Models
{
    using Properties;
    internal static class AlertExtensions
    {
        private static readonly ConcurrentDictionary<(Type Type, string Prop), RangeAttribute?> AttributeMap = new ConcurrentDictionary<(Type, string), RangeAttribute?>();
        private static RangeAttribute? GetRangeAttribute<T>(string propertyName)
        {
            // Basically cache the attributes here, because reflection can be computationally expensive
            return AttributeMap.GetOrAdd((typeof(T), propertyName), k => k.Type.GetProperty(k.Prop)?.GetCustomAttribute<RangeAttribute>());
        }

        public static RangedAlert CreateRangedAlertSafe<T>(this T _, string propertyName)
        {
            var range = GetRangeAttribute<T>(propertyName);
            if (range != null)
            {
                var alert = new RangedAlert(propertyName, "", AlertLevel.Safe, range);
                return alert;
            }

            throw new InvalidOperationException(string.Format(Resources.Error_RangedPropertyAttributeNotFound, propertyName));
        }
        
        /// <summary>
        ///   Create a new <see cref="RangedAlert"/>
        /// </summary>
        /// <typeparam name="T"> The type of the model. </typeparam>
        /// <param name="_">
        ///   This is here only as a way to make it simpler to identify the type to be able to get the attribute via reflection.
        /// </param>
        /// <param name="propertyName"> The name of the property that the alert is for. </param>
        /// <param name="message"> The alert message. </param>
        /// <param name="level"> The alert level </param>
        /// <returns> A new alert. </returns>
        /// <exception cref="InvalidOperationException">
        ///   If the property that is attempted to be accessed does not exist, or that property does not have a
        ///   <see cref="RangeAttribute"/> applied to it.
        /// </exception>
        public static RangedAlert CreateRangedAlert<T>(this T _, string propertyName, string message, AlertLevel level) where T : IAlertableModel
        {
            var range = GetRangeAttribute<T>(propertyName);
            if (range != null)
            {
                var alert = new RangedAlert(propertyName, message, level, range);
                return alert;
            }

            throw new InvalidOperationException(string.Format(Resources.Error_RangedPropertyAttributeNotFound, propertyName));
        }
    }
}