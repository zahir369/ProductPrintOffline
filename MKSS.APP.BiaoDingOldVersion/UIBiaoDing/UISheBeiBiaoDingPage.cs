using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System.Collections.Generic;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    public class UISheBeiBiaoDingPage
    {
        public SensorGroupDataModel From { get; set; }
        public SensorGroupDataModel To { get; set; }
        public List<SensorGroupDataModel> Data { get; set; }
        public override string ToString()
        {
            string start = "";
            string end = "";
            foreach (var item in Data)
            {
                if (item.Address > 0) {
                    if (string.IsNullOrEmpty(start)) start = item.AddressString;
                    end = item.AddressString;
                }
            }
            return string.Format("{0}-{1}", start, end);
        }
    }

}
