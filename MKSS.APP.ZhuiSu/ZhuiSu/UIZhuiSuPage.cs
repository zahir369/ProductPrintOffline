using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using System.Collections.Generic;

namespace MKSS.APP.ZhuiSu
{
    public class UIZhuiSuPage
    {
        public PageBoardItem From { get; set; }
        public PageBoardItem To { get; set; }
        public List<PageBoardItem> Data { get; set; }
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

            BoardCase _BoardCase = null;
            Board _Board = null;
            Batch _Batch = UIZhuiSuData.Instance.OfBatch(From, ref _BoardCase, ref _Board);

            if (_BoardCase != null && _Board != null) {
                string str = string.Format("{0}-{1}",
                       _BoardCase == null ? "" : _BoardCase.F_BoardCaseAddress,
                       _Board == null ? "" : _Board.F_FloorNO
                );
                return str;
            }
           

            return string.Format("{0}-{1}", start, end);
        }
    }

}
