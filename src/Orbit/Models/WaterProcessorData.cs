using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class WaterProcessorData : IAlertableModel, IEquatable<WaterProcessorData>
    {
        #region Limits

        private const double postHeaterTempUpperLimit = 130.5;
        private const double postHeaterTempLowerLimit = 120.5;
        private const double postHeaterTempTolerance = 5;

        private const int productTankLevelUpperLimit = 100;
        private const int productTankLevelTolerance = 20;

        #endregion Limits

        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        /// <summary>
        /// indicator of overall system status (Standby, Processing, Failure...)
        /// </summary>
        public SystemStatus SystemStatus { get; set; }

        /// <summary>
        /// draws water from dirty storage tank and pushes into the water processing system
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// This sensor is located between two identical filter beds. It will trigger if contaminates are detected which
        /// indicates the first filter bed is saturated and needs to be changed by personal. The second filter is then
        /// moved to the first position and a new filter is then installed into the second filter position.
        /// </summary>
        public bool FiltersOk { get; set; }

        /// <summary>
        /// Heats water to temp before entering the reactor
        /// </summary>
        public bool HeaterOn { get; set; }

        /// <summary>
        /// this is a sensor(s) which is assumed will provide detailed water quality info on Gateway. for now I'm
        /// assuming it returns the results as a pass/fail check temp of water leaving heater and before entering
        /// reactor Nominal is 130.5
        /// </summary>
        [Range(32, 150)]
        public double PostHeaterTemp { get; set; }

        /// <summary>
        /// this is a sensor(s) which is assumed will provide detailed water quality info on Gateway. for now I'm
        /// assuming it returns the results as a pass/fail check
        /// </summary>
        public bool PostReactorQualityOk { get; set; }

        /// <summary>
        /// valve diverts water to product tank if PostRectorQualityOK is true, or back into process assembly if false
        /// </summary>
        public DiverterValvePositions DiverterValvePosition { get; set; } = DiverterValvePositions.Reprocess;

        /// <summary>
        /// Stores clean water ready for consumption
        /// </summary>
        [Range(0, 100)]
        public double ProductTankLevel { get; set; }

        public void ProcessData(double wasteTankLevel, double heaterTemp)
        {
            PostHeaterTemp = heaterTemp;
            const int smallIncrement = 2;
            const int largeIncrement = 5;
            const int highLevel = productTankLevelUpperLimit - productTankLevelTolerance;

            if (SystemStatus == SystemStatus.Standby)
            {
                if (wasteTankLevel >= highLevel
                    && ProductTankLevel < productTankLevelUpperLimit)
                {
                    SystemStatus = SystemStatus.Processing;
                    PumpOn = true;
                    HeaterOn = true;
                    ProductTankLevel += largeIncrement;
                }
                else
                {
                    if (ProductTankLevel <= smallIncrement)
                    {
                        ProductTankLevel = 0;
                    }
                    else
                    {
                        ProductTankLevel -= smallIncrement;
                    }
                }
            }
            else if (SystemStatus == SystemStatus.Processing)
            {
                if (wasteTankLevel <= 0)
                {
                    SystemStatus = SystemStatus.Standby;
                    PumpOn = false;
                    HeaterOn = false;
                    ProductTankLevel -= (smallIncrement * 2);
                }
                else if (ProductTankLevel >= productTankLevelUpperLimit)
                {
                    SystemStatus = SystemStatus.Standby;
                    PumpOn = false;
                    HeaterOn = false;
                    ProductTankLevel = productTankLevelUpperLimit;
                }
                else
                {
                    ProductTankLevel += largeIncrement;
                }
            }
            else //(wasteTankLevel <= 0)
            {
                SystemStatus = SystemStatus.Standby;
                PumpOn = false;
                HeaterOn = false;
                ProductTankLevel -= smallIncrement;
            }
        }

        #region ValueCheckMethods

        private IEnumerable<Alert> CheckProductTankLevel()
        {
            if (ProductTankLevel >= productTankLevelUpperLimit)
            {
                yield return new Alert(nameof(ProductTankLevel), "Clean water tank is at capacity", AlertLevel.HighError);
            }
            else if (ProductTankLevel >= (productTankLevelUpperLimit - productTankLevelTolerance))
            {
                yield return new Alert(nameof(ProductTankLevel), "Clean water tank is nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(ProductTankLevel));
            }
        }

        private IEnumerable<Alert> CheckFiltersOk()
        {
            if (this.FiltersOk)
            {
                yield return Alert.Safe(nameof(this.FiltersOk));
            }
            else
            {
                yield return new Alert(nameof(this.FiltersOk), "Filters in need of changing", AlertLevel.HighWarning);
            }
        }

        private IEnumerable<Alert> CheckPostHeaterTemp()
        {
            if (PostHeaterTemp >= postHeaterTempUpperLimit)
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is above maximum", AlertLevel.HighError);
            }
            else if (PostHeaterTemp >= (postHeaterTempUpperLimit - postHeaterTempTolerance))
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is too high", AlertLevel.HighWarning);
            }
            else if (PostHeaterTemp <= postHeaterTempLowerLimit)
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is below minimum", AlertLevel.LowError);
            }
            else if (PostHeaterTemp <= (postHeaterTempLowerLimit + postHeaterTempTolerance))
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(PostHeaterTemp));
            }
        }

        private IEnumerable<Alert> CheckPostReactorQuality()
        {
            if (this.PostReactorQualityOk)
            {
                yield return Alert.Safe(nameof(this.PostReactorQualityOk));
            }
            else
            {
                yield return new Alert(nameof(this.PostReactorQualityOk), "Post reactor water quality is below limit(s). Reprocessing", AlertLevel.HighWarning);
            }
        }

        private IEnumerable<Alert> CheckSystemStatus()
        {
            if (ProductTankLevel > 0)
            {
                if (this.SystemStatus != SystemStatus.Trouble)
                {
                    this.SystemStatus = SystemStatus.Processing;
                }
            }

            if (this.SystemStatus == SystemStatus.Trouble)
            {
                yield return new Alert(nameof(SystemStatus), "Potential issue in Water processor",
                    AlertLevel.HighError);
            }
            else
            {
                yield return Alert.Safe(nameof(SystemStatus));
            }
        }

        #endregion ValueCheckMethods

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckProductTankLevel()
                .Concat(this.CheckFiltersOk())
                .Concat(this.CheckPostHeaterTemp())
                .Concat(this.CheckPostReactorQuality())
                .Concat(this.CheckSystemStatus());
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "WaterProcessor";

        #endregion Implementation of IModuleComponent

        #region Equality members

        /// <inheritdoc/>
        public bool Equals(WaterProcessorData other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return this.ReportDateTime.Equals(other.ReportDateTime)
                   && this.SystemStatus == other.SystemStatus
                   && this.PumpOn == other.PumpOn
                   && this.FiltersOk == other.FiltersOk
                   && this.HeaterOn == other.HeaterOn
                   && this.PostHeaterTemp.Equals(other.PostHeaterTemp)
                   && this.PostReactorQualityOk == other.PostReactorQualityOk
                   && this.DiverterValvePosition == other.DiverterValvePosition
                   && this.ProductTankLevel.Equals(other.ProductTankLevel);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is WaterProcessorData other && this.Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                    this.ReportDateTime,
                    this.SystemStatus,
                    this.PumpOn,
                    this.FiltersOk,
                    this.HeaterOn,
                    this.PostHeaterTemp,
                    this.PostReactorQualityOk,

                    // Have to use tuple here because for some reason the method is capped at 8 args
                    (this.DiverterValvePosition, this.ProductTankLevel)
                );
        }

        public static bool operator ==(WaterProcessorData left, WaterProcessorData right) => Equals(left, right);

        public static bool operator !=(WaterProcessorData left, WaterProcessorData right) => !Equals(left, right);

        #endregion Equality members
    }
}