using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    class WaterSystem
    {
        int urinePretreatmentLevel;
        int urineTankLevel;
        bool urineTankToDistillerPumpOn;
        string diverterValvePosition;
       
        bool distillerReady;
        int distillerMotorSpeed;
        double distillerVibration;
        int distillerTemp;
        bool distillerToBrineFilterPumpOn;
        int brineTankLevel;
        
        bool distillateToGasSeperatorPumpOn;
        bool distillateGasSeperatorOk;
        bool distillateToProcessorPumpOn;

        int grayWaterTankLevel;
        bool grayWaterToFiltersPumpOn;
        
        bool filterInflowOk;
        bool filterOutflowOk;

        int preHeaterTemp;
        bool catalystReactorOk;

        bool waterQualitySensorOk;
        string waterReprocessValvePosition;
        
        int cleanTankLevel;

        bool systemOnOff;
        bool systemReady;


    }
}
