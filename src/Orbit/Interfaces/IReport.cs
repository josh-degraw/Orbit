using System;

namespace Orbit.Models
{
    public interface IReport
    {
        double CurrentValue { get; }
        DateTimeOffset ReportDateTime { get; }
        string ReportType { get; }
    }
    public interface IBoundedReport : IReport
    {
        Limit? Limit { get; }
    }

}