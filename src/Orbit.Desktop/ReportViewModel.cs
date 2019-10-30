using System;
using Orbit.Models;

namespace Orbit.Desktop
{
    public class ReportViewModel : ViewModelBase
    {
        public ReportViewModel(string componentName, BoundedValue value)
        {
            this.ComponentName = componentName;
            this.CurrentValue = value;
            this.ReportDate = DateTimeOffset.UtcNow;
        }
        
        private DateTimeOffset _reportDate;
        private BoundedValue _currentValue;

        public string ComponentName { get; }

        public DateTimeOffset ReportDate {
            get => _reportDate;
            set => OnPropertyChanged(ref _reportDate, value);
        }

        public BoundedValue CurrentValue {
            get => _currentValue;
            set => OnPropertyChanged(ref _currentValue, value);
        }
    }
}