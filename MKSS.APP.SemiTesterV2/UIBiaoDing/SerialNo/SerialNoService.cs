using MKSS.Common;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using MKSS.Model;
using System.Windows;
using System.Configuration;

namespace MKSS.Service.UIBiaoDing
{


    public class SerialNoService {


		UnitOfWorkScrwd work = null;
		public SqlSugarClient db = null;
		public Batch CurrentBatch { get; set; }

		/// <summary>
		///  编号规则
		/// </summary>
		public SerialNoTypeEnum SerialNoType
		{
			get
			{
				SerialNoTypeEnum ret = SerialNoTypeEnum.SCRWD;
				string SerialNoType = ConfigurationManager.AppSettings["SerialNoType"];
				Enum.TryParse<SerialNoTypeEnum>(SerialNoType, true, out ret);
				return ret;
			}
		}

		public SerialNoService()
		{
			work = new UnitOfWorkScrwd();
			db = work.GetDbClient();
		}


		/// <summary>
		///  获取编号容器
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public SerialNoConfig SerialNoBtnConfig
		{
			get {
				switch (SerialNoType)
				{
					case SerialNoTypeEnum.SCRWD:
						return new SerialNoConfig() { BtnTxt = "任务单", BtnTootip = "请选择所属任务单" };
					case SerialNoTypeEnum.USER_DEF:
					default:
						return new SerialNoConfig() { BtnTxt = "自定义串号", BtnTootip = "定义串号规则" };
				}
			}
		}

		/// <summary>
		///  获取编号容器
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public SerialNoParser SerialNoParser(string rule) {
            switch (SerialNoType)
            {
                case SerialNoTypeEnum.SCRWD:
                    return new SerialNoParserScrwd(rule);
                case SerialNoTypeEnum.USER_DEF:
				default:
					return new SerialNoParserUserDefine(rule);
			}
		}

		/// <summary>
		///  编号规则弹窗
		/// </summary>
		/// <returns></returns>
		public Window SerialNoWindow() {
			switch (SerialNoType)
			{
				case SerialNoTypeEnum.SCRWD:
					return new APP.LaoHua.UILaoHua.UIScrwSelect();
				case SerialNoTypeEnum.USER_DEF:
				default:
					return new APP.UIBiaoDing.UISerialNoDefine();
			}
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
		}

		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<ulong> NextSerialNo(string scrwd, int count,long F_BoardCaseId,long F_BoardId,List<int> F_SlotNO_list)
        {
			SerialNoParser e = SerialNoParser(scrwd);
			string strSql = string.Format("select max(F_SerialNO) F_SerialNO from pd_sensor_scrwd where F_SCRWD_OrderNumber='{0}' and F_SerialNO>={1} and F_SerialNO<={2} ", scrwd, e.GetMin(), e.GetMax());
			List<ulong> ret = new List<ulong>();
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			ulong min = e.GetMin();
			if (table != null && table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
			{
				min = Convert.ToUInt64(table.Rows[0][0]);
			}

			while (ret.Count < count)
			{
				min++;
				int F_SlotNO = F_SlotNO_list[ret.Count];
				StringBuilder inserts = new StringBuilder();
				inserts.Append(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO,F_BoardCaseId,F_BoardId,F_SlotNO,F_CreateTime,F_SerialValidCode) VALUES ('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}');"
					, scrwd, min, F_BoardCaseId, F_BoardId, F_SlotNO,DateTime.Now, GetRandomByGuid()));
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
		public void NewSerialNo(string scrwd, long NewSerialNo, long F_BoardCaseId, long F_BoardId, int F_SlotNO)
		{ 
			var rezz = db.Ado.ExecuteCommandAsync(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO,F_BoardCaseId,F_BoardId,F_SlotNO,F_CreateTime,F_SerialValidCode) VALUES ('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}');"
				, scrwd, NewSerialNo, F_BoardCaseId, F_BoardId, F_SlotNO, DateTime.Now, GetRandomByGuid())).Result;
		}

		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public bool Exists(string scrwd, long NewSerialNo, long OldSerialNo)
		{
			string strSql = string.Format("select F_SerialNO from pd_sensor_scrwd where F_SCRWD_OrderNumber='{0}' and F_SerialNO={1}  and F_SerialNO!={2} ", scrwd, NewSerialNo, OldSerialNo);
			List<ulong> ret = new List<ulong>();
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result; 
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
}
 
