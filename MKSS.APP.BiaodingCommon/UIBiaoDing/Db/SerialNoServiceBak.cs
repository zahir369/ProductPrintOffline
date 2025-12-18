using MKSS.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using MKSS.Model.Laohua;

namespace MKSS.Service.UIBiaoDing
{
    public class SerialNoServiceBak {
		UnitOfWorkScrwd work = null;
		public SqlSugarClient db = null;
		public SerialNoServiceBak()
		{
			work = new UnitOfWorkScrwd();
			db = work.GetDbClient();
		}

		public int SerialValidCodeOfSerialNo(string SerialNo) {
			string strSql = string.Format("select F_SerialValidCode from pd_sensor_scrwd where F_SerialNO='{0}' ", SerialNo);
			int ret = new int(); 
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			if (table.Rows.Count > 0) {
				if (table.Rows[0][0] == DBNull.Value) {
					ret = GetRandomByGuid();
					strSql = string.Format("update pd_sensor_scrwd set F_SerialValidCode={1} where F_SerialNO='{0}' ", SerialNo, ret);
					var rr = db.Ado.ExecuteCommand(strSql);
					if (rr == 0) return 9999;
					return ret;
				}
				return int.Parse(table.Rows[0][0]+"");
			}
			return 9999;
		}


		/// <summary>
		/// 使用Guid产生的种子生成真随机数
		/// </summary>
		static int GetRandomByGuid()
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			return random.Next(1000, 9999);
			//Print(array);// 输出生成的随机数
		}

		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<ulong> NextSerialNo(string scrwd, int count)
        {
			ScrwdNoEntity e = new ScrwdNoEntity(scrwd);
			string strSql = string.Format("select max(F_SerialNO) F_SerialNO from pd_sensor_scrwd where F_SCRWD_OrderNumber='{0}' and F_SerialNO>={1} and F_SerialNO<={2} ", scrwd, e.GetMin(), e.GetMax());
			List<ulong> ret = new List<ulong>();
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			ulong min = ulong.Parse((e.DateTime.Year - 2000) + "" + e.MainNo.ToString("00000") + "00000");
			if (table != null && table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
			{
				min = Convert.ToUInt64(table.Rows[0][0]);
			}

			while (ret.Count < count)
			{
				min++;
				StringBuilder inserts = new StringBuilder();
				inserts.Append(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO) VALUES ('{0}', {1});", scrwd, min));
				try
				{
					int this_ret = db.Ado.ExecuteCommandAsync(inserts.ToString()).Result;
					if (this_ret > 0)
					{
						ret.Add(min);
					}
				}
				catch (Exception)
				{

				}

			}

			return ret;
		}


		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public void NewSerialNo(string scrwd, long NewSerialNo )
		{ 
			var rezz = db.Ado.ExecuteCommandAsync(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO) VALUES ('{0}', {1});", scrwd, NewSerialNo)).Result;
		}

		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public bool Exists(string scrwd, long NewSerialNo, long OldSerialNo)
		{
			ScrwdNoEntity e = new ScrwdNoEntity(scrwd);
			string strSql = string.Format("select F_SerialNO from pd_sensor_scrwd where F_SCRWD_OrderNumber='{0}' and F_SerialNO={1}  and F_SerialNO!={2} ", scrwd, NewSerialNo, OldSerialNo);
			List<ulong> ret = new List<ulong>();
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			ulong min = ulong.Parse((e.DateTime.Year - 2000) + "" + e.MainNo.ToString("00000") + "00000");
			if (table != null && table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
			{
				return true;
			}
			return false;
		}


		public void SetUpBoardCase()
		{

		}


    }

	public class ScrwdNoEntity {

		/// <summary>
		///  SCRW-2021-07-31-01430
		///  990036990666
		/// </summary>
		/// <param name="str"></param>
		public ScrwdNoEntity(string str) {
			StrValue = str;
			DateTime = DateTime.Parse(StrValue.Substring(5, 10));
			MainNo = int.Parse(StrValue.Substring(16, 5));
		}
		public string StrValue { get; set; }
		public DateTime DateTime { get; set; }
		public int MainNo { get; set; }
		public long GetMin() {
			return long.Parse((this.DateTime.Year - 2000) + "" + this.MainNo.ToString("00000") + "00000");
		}
		public long GetMax()
		{
			return long.Parse((this.DateTime.Year - 2000) + "" + this.MainNo.ToString("00000") + "99999");
		}
	}
}
 
