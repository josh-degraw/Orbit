using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class WaterProcessorData : IAlertableModel
    {
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
        public bool FiltersOK { get; set; }

        /// <summary>
        /// Heats water to temp before entering the reactor
        /// </summary>
        public bool HeaterOn { get; set; }

        /// <summary>
        /// temp of water leaving heater and before entering reactor
        /// Nominal is 130.5
        /// </summary>
        [Range(32,150)]
        public double PostHeaterTemp { get; set; }

        private const double postHeaterTempUpperLimit = 130.5;
        private const double postHeaterTempLowerLimit = 120.5;
        private const double postHeaterTempTolerance = 5;


        /// <summary>
        /// this is a sensor(s) which is assumed will provide detailed water quality info on Gateway. 
        /// for now I'm assuming it returns the results as a pass/fail check
        /// </summary>
        public bool PostReactorQualityOK { get; set; }

        /// <summary>
        /// valve diverts water to product tank if PostRectorQualityOK is true, or back into process assembly if false
        /// </summary>
        public DiverterValvePositions DiverterValvePosition { get; set; } = DiverterValvePositions.Reprocess;

        /// <summary>
        /// Stores clean water ready for consumption
        /// </summary>
        [Range(0, 100)]
        public int ProductTankLevel { get; set; }

        private const int productTankLevelUpperLimit = 100;
        private const int productTankLevelTolerance = 5;

        
        #region ValueCheckMethods
        private IEnumerable<Alert> CheckProductTankLevel()
        {
            if(ProductTankLevel >= productTankLevelUpperLimit)
            {
                yield return new Alert(nameof(ProductTankLevel), "Clean water tank is at capacity", AlertLevel.HighError);
            }
            else if(ProductTankLevel >= (productTankLevelUpperLimit -  productTankLevelTolerance))
            {
                yield return new Alert(nameof(ProductTankLevel), "Clean water tank is nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(ProductTankLevel));
            }
        }
        private IEnumerable<Alert> CheckFiltersOK()
        {
            if(FiltersOK == true)
            {
                yield return Alert.Safe(nameof(FiltersOK));
            }
            else
            {
                yield return new Alert(nameof(FiltersOK), "Filters in need of changing", AlertLevel.HighWarning);
            }
        }
            
        private IEnumerable<Alert> CheckPostHeaterTemp()
        {
            if(PostHeaterTemp >= postHeaterTempUpperLimit)
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is above maximum", AlertLevel.HighError);
            }
            else if(PostHeaterTemp >= (postHeaterTempUpperLimit - postHeaterTempTolerance))
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is too high", AlertLevel.HighWarning);

            }
            else if(PostHeaterTemp <= postHeaterTempLowerLimit)
            {
                yield return new Alert(nameof(PostHeaterTemp), "Pre reactor water temp is below minimum", AlertLevel.LowError);
            }
            else if(PostHeaterTemp <= (postHeaterTempLowerLimit + postHeaterTempTolerance))
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
            if(PostReactorQualityOK == true)
            {
                yield return Alert.Safe(nameof(PostReactorQualityOK));
            }
            else
            {
                yield return new Alert(nameof(PostReactorQualityOK), "Post reactor water quality is below limit(s). Reprocessing", AlertLevel.HighWarning);
            }
        }

        #endregion ValueCheckMethods


        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            //TODO: Implement
            return CheckProductTankLevel().Concat(CheckFiltersOK()).Concat(CheckPostHeaterTemp()).Concat(CheckPostReactorQuality());
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "WaterProcessor";

        #endregion Implementation of IModuleComponent
    }
}