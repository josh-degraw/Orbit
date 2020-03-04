using UnityEngine;

namespace AwesomeCharts {
    [System.Serializable]
    public class BarSingleAxisConfig : SingleAxisConfig {

        [SerializeField]
        private BarAxisValueFormatterConfig valueFormatterConfig = new BarAxisValueFormatterConfig ();
        [SerializeField]
        private AxisLabelGravity labelsAlignment = AxisLabelGravity.START;

        public AxisLabelGravity LabelsAlignment {
            get { return labelsAlignment; }
            set { labelsAlignment = value; }
        }

        public BarAxisValueFormatterConfig ValueFormatterConfig {
            get { return valueFormatterConfig; }
            set { valueFormatterConfig = value; }
        }
    }
}