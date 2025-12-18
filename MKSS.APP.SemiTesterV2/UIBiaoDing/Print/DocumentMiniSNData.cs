using System.Collections.Generic;
using System.Linq;

namespace MKSS.APP.UIBiaoDing.Print
{
    public class DocumentMiniSNData
    {
        public List<SN_ITEM> SN_LIST { get; set; }
    }

    public class SN_ITEM
    {
        public int APP { get; set; }
        public int ADDR { get; set; }
        public int POS { get; set; }
        public string SN { get; set; }
        public int F_SerialValidCode { get; set; }
    }
}
