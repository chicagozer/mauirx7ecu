using System;
using System.Collections.Generic;
using System.Text;

namespace RX7Interface.Gauges
{
    class Gauges
    {
        public GaugeInfo Rpm
        {
            get
            {
                return new GaugeInfo(0, 9000, 8000, 9000);
            }
        }

        public GaugeInfo BatteryVoltage
        {
            get
            {
                return new GaugeInfo(8, 20, 10, 15);
            }
        }
    }
}
