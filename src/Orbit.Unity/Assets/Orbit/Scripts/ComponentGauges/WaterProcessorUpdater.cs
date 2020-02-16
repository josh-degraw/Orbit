using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Microsoft.Extensions.DependencyInjection;
using Orbit;
using Orbit.Models;
using Orbit.Unity;
using Orbit.Util;
using UnityEngine;

public class WaterProcessorUpdater : ComponentUpdater
{
    protected override Alert GetLatestAlert()
    {
        using (var scope = OrbitServiceProvider.Instance.CreateScope())
        {
            var comp = scope.ServiceProvider.GetService<IMonitoredComponent<WaterProcessorData>>();

            return comp.GetLatestAlert(d => d.ProductTankLevel);
        }
    }
}
