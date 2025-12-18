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
using MKSS.APP.UIBiaoDing.Util;

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

		public SerialNoParser Parser { get; set; }
		/// <summary>
		///  获取编号容器
		/// </summary>
		/// <param name="rule"></param>
		/// <returns></returns>
		public SerialNoParser SerialNoParser(string rule, SerialNoRule ruleDef, SerialNoNet nb) {

			if (ruleDef == null || nb == null) {
				if (rule.StartsWith("SCRW-"))
				{
					return new SerialNoParserScrwd(rule);
				}
				else
				{
					return new SerialNoParserUserDefine(rule);
				}
			}



            switch (ruleDef.Rule)
			{
				case SerialNoRuleEnnum.JK_12_CH4:
				case SerialNoRuleEnnum.JK_12_CO:
				case SerialNoRuleEnnum.JK_12_CH4_CO:
					return new SerialNoParserJK(rule, ruleDef, nb);
				case SerialNoRuleEnnum.JK_14_XO:
					return new SerialNoParserJKXO(rule, ruleDef, nb);
				case SerialNoRuleEnnum.COMMON_12:
				default:
					if (rule.StartsWith("SCRW-"))
					{
						return new SerialNoParserScrwd(rule);
					}
					else
					{
						return new SerialNoParserUserDefine(rule);
					} 
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


		public DataTable GetDataTable(string strSql)
		{ 
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			return table;
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


		public Dictionary<string, int> SerialValidCodeOfSerialNo(Dictionary<string,int> SerialNoList)
		{
			if (SerialNoList.Count == 0) return SerialNoList;
			StringBuilder sb = new StringBuilder();
            foreach (var item in SerialNoList.Keys)
            {
				if (string.IsNullOrEmpty(item)) continue;
				sb.Append(string.Format("OR F_SerialNO='{0}' ", item));
			}
			if (sb.Length == 0) return SerialNoList;

			string strSql = string.Format("select F_SerialNO,F_SerialValidCode from pd_sensor_scrwd where {0} ", sb.ToString().TrimStart("OR".ToArray()));
			int ret = new int();
			DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
			if (table.Rows.Count > 0)
			{
                for (int i = 0; i < table.Rows.Count; i++)
                {
					string F_SerialNO = table.Rows[i][0]+"";
					int F_SerialValidCode = int.Parse(table.Rows[i][1] + "");
					if (table.Rows[i][1] == DBNull.Value)
					{
						ret = GetRandomByGuid();
						strSql = string.Format("update pd_sensor_scrwd set F_SerialValidCode={1} where F_SerialNO='{0}' ", F_SerialNO, ret);
						var rr = db.Ado.ExecuteCommand(strSql);
						if (rr == 0) SerialNoList[F_SerialNO] = 9999;
						SerialNoList[F_SerialNO] = ret;
					}
					SerialNoList[F_SerialNO] = int.Parse(table.Rows[0][1] + "");
				}
				
			}
			return SerialNoList;
		}

		/// <summary>
		/// 使用Guid产生的种子生成真随机数
		/// </summary>
		static int GetRandomByGuid()
		{
			Random random = new Random(Guid.NewGuid().GetHashCode());
			return random.Next(1000, 9999);
		}

		private static object obj = new object();

		/// <summary>
		///  SCRW-2021-07-31-01430
		/// </summary>
		/// <param name="scrwd"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public List<ulong> NextSerialNo(SerialNoRuleEntity en,string scrwd, int count,long F_BoardCaseId,long F_BoardId,List<int> F_SlotNO_list)
        {

			lock (obj)
			{
				//锁定运行的代码段
				try
				{
					SerialNoParser e = SerialNoParser(scrwd, en.SerialNoRule, en.SerialNoNet);
					string strSql = string.Format("select max(F_SerialNO) F_SerialNO from pd_sensor_scrwd where F_SerialNO>={1} and F_SerialNO<={2} ", scrwd, e.GetMin(), e.GetMax());
					List<ulong> ret = new List<ulong>();
					DataTable table = db.Ado.GetDataTableAsync(strSql).Result;
					ulong min = e.GetMin();
					if (table != null && table.Rows.Count > 0 && table.Rows[0][0] != DBNull.Value)
					{
						min = Convert.ToUInt64(table.Rows[0][0]);
					}

					Dictionary<string, int> valid = new Dictionary<string, int>();
					while (ret.Count < count)
					{
						min++;
						var serialNo = e.GetNext(min);
						min = serialNo;
						int F_SlotNO = F_SlotNO_list[ret.Count];
						valid.Add(serialNo.ToString(), GetRandomByGuid());
						StringBuilder inserts = new StringBuilder();
						inserts.Append(string.Format("insert into pd_sensor_scrwd (F_SCRWD_OrderNumber,F_SerialNO,F_BoardCaseId,F_BoardId,F_SlotNO,F_CreateTime,F_SerialValidCode) VALUES ('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}');"
							, scrwd, serialNo, F_BoardCaseId, F_BoardId, F_SlotNO, DateTime.Now, valid[serialNo.ToString()]));
						try
						{
							int this_ret = db.Ado.ExecuteCommandAsync(inserts.ToString()).Result;
							if (this_ret > 0)
							{
								ret.Add(serialNo);
							}
						}
						catch (Exception)
						{

						}

					}

					//如果是不联网版本，直接记录到生产数据库中
					if (en.SerialNoNet != null && en.SerialNoNet.Net == SerialNoNetEnnum.NO_NET && ret.Count > 0)
					{
						StringBuilder querys = new StringBuilder();
						for (int i = 0; i < ret.Count; i++)
						{
							if (i == 0)
							{
								querys.Append(string.Format(" F_Id='{0}' ", ret[i]));
							}
							else
							{
								querys.Append(string.Format(" or F_Id='{0}' ", ret[i]));
							}
						}

						var ent = APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance;
						var bat = APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.DbService.CurrentBatch;
						var listDevice = new List<Device>();
						var listUpdate = new List<Device>();
						var dic = db.Queryable<Device>().Where(querys.ToString()).ToList().ToDictionary(w => w.F_Id, w => w);
						for (int i = 0; i < ret.Count; i++)
						{
							var serialNo = ret[i].ToString();
							if (!dic.ContainsKey(serialNo))
							{
								listDevice.Add(new Device()
								{
									F_Id = serialNo,
									F_CreateDate = DateTime.Now,
									F_DeviceNetType = 0,
									F_SCRWD_OrderNumber = bat.F_SCRWD_OrderNumber,
									F_SCRWD_ProductCode = bat.F_SCRWD_ProductCode,
									F_SCRWD_ProductFullName = bat.F_SCRWD_ProductFullName,
									F_SCRWD_ProductType = bat.F_SCRWD_ProductType,
									F_SerialNO = serialNo,
									F_SerialValidCode = valid[serialNo].ToString(),
									F_SerialNOTime = DateTime.Now,
									F_Status = 0
								});
							}
							else
							{
								var dev = dic[serialNo];
								dev.F_SerialNOTime = DateTime.Now;
								dev.F_SCRWD_OrderNumber = bat.F_SCRWD_OrderNumber;
								dev.F_SCRWD_ProductCode = bat.F_SCRWD_ProductCode;
								dev.F_SCRWD_ProductFullName = bat.F_SCRWD_ProductFullName;
								dev.F_SCRWD_ProductType = bat.F_SCRWD_ProductType;
								listUpdate.Add(dev);
							}
						}

                        try
                        {
							if (listDevice.Count > 0)
							{
								db.Insertable<Device>(listDevice).ExecuteCommand();
							}
							if (listUpdate.Count > 0)
							{
								db.Updateable<Device>(listUpdate).ExecuteCommand();
							}
						}
                        catch (Exception ex)
                        {
                            throw ex;
                        }
						

					}
					return ret;
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			

		}

		/// <summary>
		///  存储到生产数据库，是否合格
		/// </summary>
		/// <param name="en"></param>
		/// <param name="data"></param>
		/// <param name="hege"></param>
		public void SetHEGE(SerialNoRuleEntity en, SensorDataX data , bool hege)
		{
			if ( data ==null || string.IsNullOrEmpty(data.Serial)) return;
            //如果是不联网版本，直接记录到生产数据库中w
            //if (en.SerialNoNet != null && en.SerialNoNet.Net == SerialNoNetEnnum.NO_NET) 
            //{
            try
            {
				string sql = string.Format("update pd_device set F_BD_HEGE={0},F_BD_HEGE_TIME='{2}' where F_SerialNO='{1}';",
hege ? 1 : 0, data.Serial, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
				int this_ret = db.Ado.ExecuteCommandAsync(sql).Result;
			}
            catch (Exception ex)
            { 
            }

            //}
        }


		/// <summary>
		///  存储到生产数据库，是否合格
		/// </summary>
		/// <param name="en"></param>
		/// <param name="data"></param>
		/// <param name="hege"></param>
		public void PushBatchData()
		{

			try
			{

				var ProductModelGroupTable = APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable;
				List<Sensor> _Sensors = APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.DbService._SensorServices.QuerySql(@" select * from pd_sensor").Result;//刷新传感器数据
				_Sensors = _Sensors.Where(w => !string.IsNullOrEmpty(w.F_SerialNO) && !string.IsNullOrEmpty(w.F_GasTunnel)).OrderBy(w => w.F_SensorId).ToList();
				Dictionary<string, Dictionary<string, Sensor>> _dic = new Dictionary<string, Dictionary<string, Sensor>>();
				List<string> gasList = new List<string>();
				foreach (var _sensor in _Sensors)
				{
					string gas = _sensor.F_GasTunnel;
					string serial = _sensor.F_SerialNO;
					if (_sensor.IsEmpData) continue;
					if (!gasList.Contains(gas)) gasList.Add(gas);
					if (!_dic.ContainsKey(serial)) _dic.Add(serial, new Dictionary<string, Sensor>());
					if (!_dic[serial].ContainsKey(gas)) _dic[serial].Add(gas, _sensor);
					else _dic[serial][gas] = _sensor;
				}

				if (gasList.Count == 0) return;
				StringBuilder inserts = new StringBuilder();
				foreach (var serial in _dic.Keys)
				{
					var line = _dic[serial];
					bool v1 = gasList.Count >= 1;
					bool v2 = gasList.Count >= 2;
					Sensor s1 = v1 && line.ContainsKey(gasList[0]) ? line[gasList[0]] : new Sensor();
					Sensor s2 = v2 && line.ContainsKey(gasList[1]) ? line[gasList[1]] : new Sensor();
					if (!v1 && !v2) continue;
					bool v1Data = s1.V2 > 0 || s1.V4 > 0;
					bool v2Data = s2.V2 > 0 || s2.V4 > 0;
					bool vHege = s1.V_HEGE_TIME.Year > 2000;
					bool vDeviceTime = !(s1.V_DeviceDateTime+"").StartsWith("0000");
					bool vDeviceTimeTime = s1.F_BD_TIME_TIME.Year > 2000;
					inserts.Append(
						string.Format(
							"UPDATE `pd_device` SET " +
							(v1Data ? ("`F_BD_GasTunnel`='{1}', `F_BD_ZERO` = {2}, `F_BD_SPAN` = {3}, `F_BD_NONGDU` = {4}, `F_BD_AD` = {5}, `F_BD_TIME` = '{6}', `F_BD_TIME_SPAN` = '{7}',  ") : "") +
							(v2Data ? ("`F_BD_GasTunnel2`='{8}', `F_BD_ZERO2` = {9}, `F_BD_SPAN2` = {10}, `F_BD_NONGDU2` = {11}, `F_BD_AD2` = {12}, `F_BD_TIME2` = '{13}', `F_BD_TIME_SPAN2` = '{14}',  ") : "") +
							(vHege ? ("`F_BD_HEGE` = {15}, `F_BD_HEGE_TIME` = '{16}',") : "") +
							(vDeviceTime ? ("`F_BD_DeviceTime`='{17}', ") : "") +
							(vDeviceTimeTime ? ("`F_BD_DeviceTime_Time` ='{18}', ") : "") +
							(true ? ("`F_BD_BoardId`={19}, `F_BD_BoardCaseId` ={20}, ") : "") +
							"`F_BD_SlotNO` = {21}  " +
							"WHERE `F_SerialNO` = '{0}';",
							serial,
							s1.F_GasTunnel, s1.V2, s1.V4, s1.V7, s1.V1, s1.F_BD_ZERO_TIME, s1.F_BD_SPAN_TIME,
							s2.F_GasTunnel, s2.V2, s2.V4, s2.V7, s2.V1, s2.F_BD_ZERO_TIME, s2.F_BD_SPAN_TIME,
							s1.V_HEGE_INT(), s1.V_HEGE_TIME, 
							s1.V_DeviceDateTime, s1.F_BD_TIME_TIME, 
							s1.F_BoardId, s1.F_BoardCaseId, s1.F_SlotNO
					));
				}
				if (inserts.Length > 0) {
					var xx = db.Ado.ExecuteCommandAsync(inserts.ToString()).Result;
				} 
			}
			catch (Exception ex)
			{
				throw ex;
			}


		}


		/// <summary>
		///  读取合格状态
		/// </summary>
		/// <param name="list"></param>
		public void GetHEGE(Dictionary<string,bool> list_all)
		{
			if (list_all.Count == 0  ) return;

			List<string> tmp = new List<string>();
			List<List<string>> pub = new List<List<string>>();
			pub.Add(tmp);
			foreach (var item in list_all.Keys) {
				if (tmp.Count < 100)
				{
					tmp.Add(item);
				}
				else {
					tmp = new List<string>();
					pub.Add(tmp);
				}
			}

            foreach (var list in pub)
            {
				StringBuilder inserts = new StringBuilder();
				inserts.Append("select F_BD_HEGE,F_SerialNO from pd_device where ");
				bool first = true;
				foreach (var item in list)
				{
					if (!first) inserts.Append(String.Format(" or ", item));
					inserts.Append(String.Format(" F_SerialNO='{0}' ", item));
					first = false;
				}


				try
				{
					var xx = db.Ado.GetDataTable(inserts.ToString());
					for (int i = 0; i < xx.Rows.Count; i++)
					{
						string F_BD_HEGE = xx.Rows[i]["F_BD_HEGE"] + "";
						string F_SerialNO = xx.Rows[i]["F_SerialNO"] + "";
						if (list_all.ContainsKey(F_SerialNO)) list_all[F_SerialNO] = F_BD_HEGE == "1";
					}
				}
				catch (Exception ex)
				{
					throw ex;
				}
			}
			
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
 
