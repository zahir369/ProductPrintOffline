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
    public class ScrwdService {
		UnitOfWorkScrwd work = null;
		public SqlSugarClient db = null;
		public ScrwdService()
		{
			work = new UnitOfWorkScrwd();
			db = work.GetDbClient();
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
            string strSql = string.Format("select max(F_SerialNO) F_SerialNO from pd_sensor_scrwd where F_SCRWD_OrderNumber='{0}' and F_SerialNO>={1} and F_SerialNO<={2} ", scrwd,e.GetMin(),e.GetMax());
            List<ulong> ret = new List<ulong>();
            DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			ulong min = ulong.Parse((e.DateTime.Year - 2000) + ""+e.MainNo.ToString("00000") + "00000");
			if (table != null && table.Rows.Count > 0 && table.Rows[0][0]!=DBNull.Value) {
				min = Convert.ToUInt64(table.Rows[0][0]);
			}
			StringBuilder inserts = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
				min++;
				ret.Add(min);
				inserts.Append(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO) VALUES ('{0}', {1});", scrwd, min));
			}
			var rezz = db.Ado.ExecuteCommandAsync(inserts.ToString()).Result;
			return ret;
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
 
