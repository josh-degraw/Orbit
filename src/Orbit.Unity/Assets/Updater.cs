using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Microsoft.Extensions.DependencyInjection;
using Orbit;
using Orbit.Models;
using Orbit.Util;
using UnityEngine;

public class Updater : MonoBehaviour
{
    ProgressBar pb;

    void Start()
    {
        try
        {
            pb = GetComponent<ProgressBar>();

            //textComponent.text = "Water Level";
            InvokeRepeating(nameof(HandleUpdate), 0, 2f);
        }
        catch (Exception e)
        {
            Debug.LogError(e, this);
        }
    }

    public void HandleUpdate()
    {
        try
        {
            if(pb?.loadingBar == null)
            {
                Debug.LogWarning("Progress bar loadingBar is null", this);
                return;
            }
            Debug.Log("Calling Update",this);
            using (var scope = OrbitServiceProvider.Instance.CreateScope())
            {
                var comp = scope.ServiceProvider.GetService<IMonitoredComponent<WaterProcessorData>>();

                var next = comp.GetLatestAlert(d => d.ProductTankLevel);

                if (next == null)
                {
                    Debug.Log("No report found", this);

                    pb.currentPercent = 0;
                    return;
                }

                Debug.LogFormat(this,"Current Value: {0}", next.CurrentValue);
                this.pb.currentPercent = Convert.ToSingle(next.CurrentValue);
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e, this);
        }
    }

}
