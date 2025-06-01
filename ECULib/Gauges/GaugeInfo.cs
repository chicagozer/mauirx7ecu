using System;
using System.Collections.Generic;
using System.Text;

namespace RX7Interface.Gauges
{
    public class GaugeInfo
    {
        public float MinValue
        {
            get;
            private set;
        }

        public float MaxValue
        {
            get;
            private set;
        }

        public float ReccomendedValue
        {
            get;
            private set;
        }

        public float ReccomendedValueRange
        {
            get;
            private set;
        }


        public GaugeInfo(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;

            ReccomendedValue = float.NaN;
            ReccomendedValueRange = float.NaN;
        }

        public GaugeInfo(float minValue, float maxValue, float reccomendedRangeLower, float reccomendedRangeUpper) : this(minValue, maxValue)
        {
            ReccomendedValue = (reccomendedRangeLower + reccomendedRangeUpper) / 2f;
            ReccomendedValueRange = 100 * (reccomendedRangeUpper - reccomendedRangeLower) / (maxValue - minValue);
        }
    }
}
