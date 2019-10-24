using System;

namespace Orbit.Models
{
    public interface IReport
    {
        double CurrentValue { get; }
        DateTimeOffset ReportDateTime { get; }
        ReportType ReportType { get; }
    }
}