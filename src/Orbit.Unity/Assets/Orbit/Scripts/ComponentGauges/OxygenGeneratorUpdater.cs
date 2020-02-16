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
    protected override Alert GetLatestAlert()
    {
        using (var scope = OrbitServiceProvider.Instance.CreateScope())
        {
            var comp = scope.ServiceProvider.GetService<IMonitoredComponent<OxygenGenerator>>();

            return comp.GetLatestAlert(d => d.SystemOutput);
        }
    }
}
