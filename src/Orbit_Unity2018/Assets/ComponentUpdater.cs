using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.UI.ModernUIPack;
using Orbit.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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


        protected abstract Alert GetLatestAlertValue();


        private void SetColor(Color color)
        {
            var text = pb.textPercent.GetComponent<TextMeshProUGUI>();

            text.color = color;
        }

        private void OnEnable()
        {
                
        }
        private void OnDisable()
        {
            
        }

        public void HandleUpdate()
        {
            try
            {
                if (pb?.loadingBar == null)
                {
                    Debug.LogWarning("Progress bar loadingBar is null", this);
                    return;
                }

                var next = GetLatestAlertValue();
                if (next == null)
                {
                    Debug.Log("No report found", this);

                    pb.currentPercent = 0;
                    return;
                }

                this.pb.currentPercent = Convert.ToSingle(next.CurrentValue);

                switch (next.AlertLevel)
                {
                    case AlertLevel.HighWarning:
                    case AlertLevel.LowWarning:
                        SetColor(pb.warningColor);
                                  
                        break;

                    case AlertLevel.HighError:
                    case AlertLevel.LowError:
                        SetColor(pb.errorColor);
                        break;

                    default:
                        SetColor(pb.barColor);
                        break;
                }

            }
            catch (Exception e)
            {
                Debug.LogError(e, this);
            }
        }
    }
}