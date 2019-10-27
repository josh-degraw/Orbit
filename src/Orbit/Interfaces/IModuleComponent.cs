﻿using System.Collections.Generic;
using System.Threading;
using Orbit.Models;

namespace Orbit
{
    /// <summary>
    ///  Base interface for module components.
    /// </summary>
    public interface IModuleComponent
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        string ComponentName { get; }

        /// <summary>
        /// Generate one or more informational reports for a component.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<CurrentValueReport> BuildCurrentValueReport(CancellationToken cancellationToken = default);
    }
}