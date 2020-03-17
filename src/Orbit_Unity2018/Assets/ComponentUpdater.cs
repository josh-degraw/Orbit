using System;
using Michsky.UI.ModernUIPack;

using Orbit.Models;
using Orbit.Util;
using TMPro;

using UnityEngine;

namespace Orbit.Unity
{
    public abstract class ComponentUpdater : MonoBehaviour
    {
        public ProgressBar pb;
        public TextMeshProUGUI text;

        private void Start()
        {
            try
            {
                if(pb == null)
                {
                    pb = GetComponent<ProgressBar>();
                }
                if(text == null)
                {
                    text = pb.textPercent.GetComponent<TextMeshProUGUI>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e, this);
            }
        }

        private void OnEnable()
        {
            EventMonitor.Instance.AlertReported += this.Instance_AlertReported;
        }

        private void OnDisable()
        {
            EventMonitor.Instance.AlertReported -= this.Instance_AlertReported;
        }

        private void Instance_AlertReported(object sender, AlertEventArgs e)
        {
            if (AlertMatches(e))
            {
                HandleUpdate(e.Alert);
            }
        }

        public void HandleUpdate(Alert alert)
        {
            this.pb.currentPercent = Convert.ToSingle(alert.CurrentValue);

            switch (alert.AlertLevel)
            {
                case AlertLevel.HighWarning:
                case AlertLevel.LowWarning:
                    text.color = pb.warningColor;

                    break;

                case AlertLevel.HighError:
                case AlertLevel.LowError:
                    text.color = pb.errorColor;
                    break;

                default:
                    text.color = pb.barColor;
                    break;
            }
        }

        protected abstract bool AlertMatches(AlertEventArgs alert);

        protected void MakePercentage(Alert next)
        {
            double val = Convert.ToDouble(next.CurrentValue);
            next.CurrentValue = Convert.ToSingle((float)next.Metadata.TotalRange.ToPercentage(val));
        }
    }
}