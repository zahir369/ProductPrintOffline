using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System.Collections.Generic;
using System.Linq;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    public class UISheBeiBiaoDingPage
    {
        public int From { get; set; }
        public int To { get; set; } 
        public override string ToString()
        {
            string start = "";
            string end = "";
            var Data = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
            for (int i = From; i <= To; i++)
            {
                if (i == From && Data[i].Address.V > 0) start = Data[i].AddressString;
                if (i <= To && Data[i].Address.V>0) end = Data[i].AddressString;
            }
            return string.Format("{0}-{1}", start, end);
        }
    }

}
