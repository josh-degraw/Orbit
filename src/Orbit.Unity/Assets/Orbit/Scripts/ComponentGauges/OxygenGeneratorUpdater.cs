using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Orbit;
using Orbit.Models;
using Orbit.Unity;
using Orbit.Util;
using UnityEngine;


public class OxygenGeneratorUpdater : ComponentUpdater
{
    protected override Alert GetLatestAlertValue()
    {
        using (var scope = OrbitServiceProvider.Instance.CreateScope())
        {
            var comp = scope.ServiceProvider.GetService<IMonitoredComponent<OxygenGenerator>>();

            var next = comp.GetLatestAlert(d => d.OxygenLevel);

            if (next == null)
                return null;

            double val =  Convert.ToDouble(next.CurrentValue);
            next.CurrentValue = Convert.ToSingle((float)next.Metadata.TotalRange.ToPercentage(val));
            return next;
        }
    }
}
