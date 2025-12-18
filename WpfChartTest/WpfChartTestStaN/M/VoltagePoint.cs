using System;

namespace WpfChartTest
{
    public class VoltagePoint
    {
        public TimeSpan F_AddTime { get; set; }

        public SensorDataItem SensorDataItem { get; set; }
        public double F_LoadDataValue { get; set; }

        public VoltagePoint(TimeSpan date, SensorDataItem voltage)
        {
            this.F_AddTime = date;
            this.SensorDataItem = voltage;
        }
    }

}
