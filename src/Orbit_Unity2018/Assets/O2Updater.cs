using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
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
        try
        {
            bool matches = alert.Query<OxygenGenerator>().Matches(a => a.OxygenLevel);
            if (matches)
            {
                MakePercentage(alert.Alert);
            }
            return matches;
        }
        catch (Exception e)
        {
            Debug.LogError(e, this);
            return false;
        }
    }
}
