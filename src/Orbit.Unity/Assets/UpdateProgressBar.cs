using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Orbit;
using Orbit.Models;
using Orbit.Util;
using UnityEngine;
using UnityEngine.UI;

public class UpdateProgressBar : MonoBehaviour
{
    public ProgressBarCircle pbc;

    public Text textComponent;

    // Start is called before the first frame update
    void Start()
    {
        //textComponent.text = "Water Level";
        InvokeRepeating(nameof(HandleUpdate), 0, 2f);
    }
    
    public async void HandleUpdate()
    {
        try
        {
            Debug.Log("Calling Update");
            using (var scope = OrbitServiceProvider.Instance.CreateScope())
            {
                var comp = scope.ServiceProvider.GetService<IMonitoredComponent<WaterProcessorData>>();

                var next = await comp.GetLatestReportAsync();

                if (next == null)
                {
                    Debug.Log("No report found");
                    this.pbc.BarValue = 0;
                    return;
                }

                Debug.Log(next.ProductTankLevel);

                this.pbc.BarValue = (float)next.ProductTankLevel;
            }

        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString(), this);
        }
    }
}
