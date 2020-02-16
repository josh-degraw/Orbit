using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Orbit.Models;
using UnityEngine;


namespace Orbit.Unity
{
    public abstract class ComponentUpdater : MonoBehaviour
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


        protected abstract Alert GetLatestAlert();

        public void HandleUpdate()
        {
            try
            {
                if (pb?.loadingBar == null)
                {
                    Debug.LogWarning("Progress bar loadingBar is null", this);
                    return;
                }

                var next = GetLatestAlert();
                if (next == null)
                {
                    Debug.Log("No report found", this);
                     
                    pb.currentPercent = 0;
                    return;
                }

                this.pb.currentPercent = Convert.ToSingle(next.CurrentValue);


            }
            catch (Exception e)
            {
                Debug.LogError(e, this);
            }
        }
    }
}