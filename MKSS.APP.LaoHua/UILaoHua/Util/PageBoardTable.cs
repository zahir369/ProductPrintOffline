using System.Collections.Generic;
using MKSS.APP.LaoHua.Util;

namespace MKSS.APP.LaoHua
{
    public class PageBoardTable
	{
        internal sta_job_status status;

        /// <summary>
        ///  默认的
        /// </summary>
        public Dictionary<int, PageBoardItem> ProductModelGroupTable
		{
			get {
				return Table;
			} 
		} 

		Dictionary<int, PageBoardItem> Table { get; set; }
		public PageBoardTable() {
			Table = new Dictionary<int, PageBoardItem>();
			for (int i = 0; i < UILaoHuaGuanChaModel.MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new PageBoardItem() { Address = 0, CaseNo = 0 });
			}
		}
	}

}
