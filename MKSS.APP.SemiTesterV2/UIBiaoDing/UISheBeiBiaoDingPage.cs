using MKSS.APP.UIBiaoDing.Util;
using System.Collections.Generic;
using System.Linq;

namespace MKSS.APP.UIBiaoDing
{
    public class UISheBeiBiaoDingPage
    {
        public int From { get; set; }
        public int To { get; set; }
        public bool Contains(Address addr) {
            var Data = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
            for (int i = From; i < To; i++)
            {
                if (Data[i].Address == addr) {
                    return true;
                }
            }
            return false;
        }
        public override string ToString()
        {
            string start = "";
            string end = "";
            var Data = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
            for (int i = From; i < To; i++)
            {
                if (i == From && Data[i].Address.V > 0) start = Data[i].Address.ToString();
                if (i <= To && Data[i].Address.V>0) end = Data[i].Address.ToString();
            }
            return string.Format("{0}-{1}", start, end);
        }
    }

}
