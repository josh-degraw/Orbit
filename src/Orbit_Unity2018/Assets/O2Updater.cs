using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Orbit;
using Orbit.Models;
using Orbit.Unity;
using Orbit.Util;
using UnityEngine;
using UnityEngine.Events;

public class O2Updater : ComponentUpdater
{
    protected override bool AlertMatches(AlertEventArgs alert)
    {
        bool matches = alert.Matches<OxygenGenerator>(a => a.OxygenLevel);
        if (matches)
        {
            MakePercentage(alert.Alert);
        }
        return matches;
    }
}
