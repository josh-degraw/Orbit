﻿using System;
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

        /// <summary>
        ///   Create a new <see cref="Alert"/> with the provided properties.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="model">The model data object.</param>
        /// <param name="propSelector">An expression (lambda method) selecting the property which the alert is for.</param>
        /// <param name="message">The message for the alert.</param>
        /// <param name="level">The level of the alert.</param>
        /// <returns>The newly created alert.</returns>
        public static Alert CreateAlert<TModel, TProperty>(this TModel model, Expression<Func<TModel, TProperty>> propSelector, string message = "", AlertLevel level = AlertLevel.Safe) where TModel : class, IAlertableModel
        {
            var body = (MemberExpression)propSelector.Body;
            var member = body.Member;
            var value = propSelector.Compile()(model);

            var info = GetMetadata(member);
            return new Alert(member.Name, message, level, info, value);
        }
    }
}