using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class ShuntUnit: IAlertableModel
    {
        /// shunt unit modulates voltages between solar array or battery source, and station
        /// solar array voltage out put should be ~160v
        /// batteries take ~160v to charge
        /// station requires ~124 volts for its power bus
        /// <summary>
        /// 
        /// </summary>
        public double InputVoltage { get; set; }
        public double OutputVoltage { get; set; }
        
        [NotMapped]
        public string ComponentName => "ShuntUnit";

        /// <summary>
        /// Voltage output to station after leaving the DC to DC conversion units
        /// Nominal is ~124V
        /// Acceptable Range: 98 - 136Vdc; transient switching variance of 93 - 141 should return to acceptable range within 5msec
        ///  transient switching values used values for warning ranges
        /// </summary>
        [Range(0, 173)]
        public double VoltageReducerOutput { get; set; }

        [NotMapped]
        public double voltageReducerOutputUpperLimit = 141;
        [NotMapped]
        public double voltageReducerOutputLowerLimit = 93;
        [NotMapped]
        public double voltageReducerOutputTolerance = 5;

        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        private IEnumerable<Alert> CheckVoltageReducerOutput()
        {
            if (VoltageReducerOutput > voltageReducerOutputUpperLimit)
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output has exceeded maximum allowed voltage", AlertLevel.HighError);
            }
            else if (VoltageReducerOutput >= (voltageReducerOutputUpperLimit - voltageReducerOutputTolerance))
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output is high", AlertLevel.HighWarning);
            }
            else if (VoltageReducerOutput < voltageReducerOutputLowerLimit)
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output has exceeded minimum allowed voltage", AlertLevel.LowError);
            }
            else if (VoltageReducerOutput <= (voltageReducerOutputLowerLimit + voltageReducerOutputTolerance))
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(VoltageReducerOutput));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckVoltageReducerOutput();
        }
    }
}
