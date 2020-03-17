using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Orbit.Models;
using Orbit.Util;

namespace Orbit
{
    /// <summary>
    ///   Indicates a component that will be monitored regularly via <see cref="EventMonitor"/>
    /// </summary>
    public interface IMonitoredComponent<T> : IMonitoredComponent
        where T : class, IModel
    {
        /// <summary>
        ///   Get the latest available report, returned as the explicit type.
        /// </summary>
        new ValueTask<T?> GetLatestReportAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///   Synchronously returns the latest available report of type <typeparamref name="T"/> available.
        /// </summary>
        T? GetLatestReport();

        /// <summary>
        ///   Returns an enumeration of reports as the concrete type of the report. Can be utilized if a component
        ///   retrieves more than one kind of data.
        /// </summary>
        new ValueTask<IReadOnlyCollection<T>> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }

    /// <summary>
    ///   Non-generic version of <see cref="IMonitoredComponent{T}"/>, used by <see cref="EventMonitor"/> to avoid it
    ///   needing to know the concrete report type.
    /// </summary>
    public interface IMonitoredComponent : IModuleComponent
    {
        /// <summary>
        ///   Get the latest available report.
        /// </summary>
        /// <returns></returns>
        ValueTask<IModel?> GetLatestReportAsync(CancellationToken cancellationToken = default);

        /// <summary>
        ///   Returns an enumeration of reports. Can be utilized if a component retrieves more than one kind of data.
        /// </summary>
        ValueTask<IReadOnlyCollection<IModel>> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }

    public static class ComponentExtensions
    {
        /// <summary>
        ///   Get all (usually one) of the latest alerts from the given component.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProp">The type of the member to check the alert for.</typeparam>
        /// <param name="component">The component.</param>
        /// <param name="selector">A lambda method returning the property to get the alerts for.</param>
        /// <returns></returns>
        public static IEnumerable<Alert> GetLatestAlerts<TModel>(this IMonitoredComponent<TModel> component) where TModel : class, IAlertableModel
        {
            var report = component.GetLatestReport();
            return report?.GenerateAlerts() ?? Enumerable.Empty<Alert>();
        }

        /// <summary>
        ///   Get all (usually one) of the latest alerts from the given component.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TProp">The type of the member to check the alert for.</typeparam>
        /// <param name="component">The component.</param>
        /// <param name="selector">A lambda method returning the property to get the alerts for.</param>
        /// <returns></returns>
        public static Alert GetLatestAlert<TModel, TProp>(this IMonitoredComponent<TModel> component, Expression<Func<TModel, TProp>> selector) where TModel : class, IAlertableModel
        {
            string name = ((MemberExpression)selector.Body).Member.Name;
            return component.GetLatestAlerts().SingleOrDefault(a => a.PropertyName == name);
        }

        /// <summary>
        ///   Use this to query <see cref="AlertEventArgs"/> using Expressions for readability.
        /// </summary>
        public static Matcher<TModel> Query<TModel>(this AlertEventArgs args) where TModel : class, IAlertableModel => new Matcher<TModel>(args);

        public sealed class Matcher<TModel>
        {
            private readonly AlertEventArgs _args;

            public Matcher(AlertEventArgs args)
            {
                this._args = args;
            }

            /// <summary>
            ///   Validate that the arguments match the provided expression.
            /// </summary>
            /// <typeparam name="TProperty"></typeparam>
            /// <param name="selector">A lambda method returning the property to get the alerts for.</param>
            /// <returns>
            ///   True if the report on the arguments is of the correct type and the selector is the referenced property.
            /// </returns>
            public bool Matches<TProperty>(Expression<Func<TModel, TProperty>> selector)
            {
                return this._args.Report is TModel
                       // calculate inline to allow lazy execution to short-circuit if the report type is not correct
                       && this._args.Alert.PropertyName == ((MemberExpression)selector.Body).Member.Name; 
            }
        }
    }
}