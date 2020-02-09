using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Orbit.Annotations;

namespace Orbit.Models
{
    internal static class AlertExtensions
    {
        private static readonly ConcurrentDictionary<int, PropertyMetadata> _metadata =
            new ConcurrentDictionary<int, PropertyMetadata>();

        private static PropertyMetadata GetMetadata(MemberInfo member)
        {
            // Indexed by hashcode to reduce the memory footprint needed
            var info = _metadata.GetOrAdd(member.GetHashCode(), _ =>
             {
                 var attributes = member.GetCustomAttributes().ToArray();

                 var range = attributes.OfType<RangeAttribute>().SingleOrDefault();
                 var idealRange = attributes.OfType<IdealRangeAttribute>().SingleOrDefault();
                 var idealValue = attributes.OfType<IdealValueAttribute>().SingleOrDefault();
                 var unit = attributes.OfType<UnitTypeAttribute>().SingleOrDefault();

                 return new PropertyMetadata(range, idealRange, idealValue, unit);
             });

            return info;
        }

        public static Alert CreateAlert<T, TProperty>(this T model, Expression<Func<T, TProperty>> propSelector, string message = "", AlertLevel level = AlertLevel.Safe) where T : class, IAlertableModel
        {
            var body = (MemberExpression)propSelector.Body;
            var member = body.Member;
            var value = propSelector.Compile()(model);

            var info = GetMetadata(member);
            return new Alert  (member.Name, message, level, info, value);
        }
    }
}