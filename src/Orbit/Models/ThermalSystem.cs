using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    class ThermalSystem
    {
        double actualCabinTemp;
        double selectedCabinTemp;

        double internalCoolantTemp;
        bool internalCoolantPumpOn;
        double internalCoolantPumpTemp;
        double internalCoolantPumpVibration;
        int internalCoolantTankLevel;
        bool cabinCoolantLeakSensorTripped;

        double externalCoolantTemp;
        bool externalcoolantPumpOn;
        double externalCoolantPumpTemp;
        double externalCoolantPumpVibration;
        int externalCoolantTankLevel;

    }
}
