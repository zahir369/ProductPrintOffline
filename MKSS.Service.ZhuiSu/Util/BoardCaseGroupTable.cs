using System.Collections.Generic;
using MKSS.Service.ZhuiSu.Util;

namespace MKSS.Service.ZhuiSu
{
    public class BoardCaseGroupTable
	{
		/// <summary>
		///  默认的
		/// </summary>
		public Dictionary<int, DataAgingSensorGroupDataModel> ProductModelGroupTable
		{
			get {
				return Table;
			}
		} 

		Dictionary<int, DataAgingSensorGroupDataModel> Table { get; set; }
		public BoardCaseGroupTable() {
			Table = new Dictionary<int, DataAgingSensorGroupDataModel>();
			for (int i = 0; i < ZhuiSuService.MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new DataAgingSensorGroupDataModel() { Address = 0, CaseNo = 0 });
			}
		}
	}

}
