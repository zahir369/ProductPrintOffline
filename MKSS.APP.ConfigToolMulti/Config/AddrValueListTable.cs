using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace MKSS.APP.ConfigToolMulti
{
    public class AddrValueListTable : List<AddrValueList>
	{

		int ColIniCount = 0;
		public AddrValueListTable() {
			Table = new DataTable("结果数据");
			DataColumn key = Table.Columns.Add("地址", typeof(int));
			Table.PrimaryKey = new DataColumn[] { key };
			ColIniCount = Table.Columns.Count;
		}
		public DataTable Table { get; private set; }
		public new void Add(AddrValueList d)
		{
			base.Add(d);
			if (Table.Columns.Count== ColIniCount)
			{
				foreach (var item in d)
                {
					Table.Columns.Add(item.Name, typeof(string));
				}
				Table.Columns.Add("刷新时间", typeof(string));
			}
			DataRow dr = Table.NewRow();
			dr["地址"] = d.DeviceAddr;
			dr["刷新时间"] = DateTime.Now.ToString("HH:mm:ss.ff");
			foreach (var item in d)
			{
				dr[item.Name] = item.ShowValue; 
			}
			Table.Rows.Add(dr);
		}
		public new void Remove(AddrValueList d) {
			base.Remove(d);
			DataRow dr = Table.Rows.Find(d.DeviceAddr);
			Table.Rows.Remove(dr);
		}

		public void RefreshData(AddrValueList d) {
			DataRow dr = Table.Rows.Find(d.DeviceAddr);
			dr["刷新时间"] = DateTime.Now.ToString("HH:mm:ss.ff");
			foreach (var item in d)
			{
				dr[item.Name] = item.ShowValue;
			}
		}

		public AddrValueList Of(DataRow dr) {
			int device = Convert.ToInt32(dr["地址"]);
			AddrValueList d  = this.FirstOrDefault(w => w.DeviceAddr == device);
			return d;
		}

	}

}
