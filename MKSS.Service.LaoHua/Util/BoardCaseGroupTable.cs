using System.Collections.Generic;
using MKSS.Service.LaoHua.Util;

namespace MKSS.Service.LaoHua
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
			for (int i = 0; i < LaohuaService.MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new DataAgingSensorGroupDataModel() { Address = 0, CaseNo = 0 });
			}
		}
	}

}
