using DeviceDataMonitorWPF.UIBiaodingJiuJing.Util;
using System.Collections.Generic;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    public class UILaoHuaGuanChaPage
    {
        public DataAgingSensorGroupDataModel From { get; set; }
        public DataAgingSensorGroupDataModel To { get; set; }
        public List<DataAgingSensorGroupDataModel> Data { get; set; }
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
