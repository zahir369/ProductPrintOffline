using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.UIBiaoDing;
using MKSS.Services;
using MKSS.Model;
using System.Diagnostics;
using System.Data;
using System.Threading;
using System.IO.Ports;
using NModbus;
using NModbus.Serial;
using MKSS.APP.UIBiaoDing.Config;
using MKSS.APP.UIBiaoDing.Util;
using System.Xml.Serialization;
using System.IO;
using MKSS.APP.UIBiaoDing;
using System.Configuration;

namespace MKSS.Service.UIBiaoDing
{


    public class BiaoDingService
	{


		BatchServices _BatchGlobalServices = new BatchServices();
		BatchServices _BatchServices = null;
		SensorServices _SensorServices = null;
		SensorDataServices _SensorDataServices = null;

		public Dictionary<string, Sensor> BatchSensorDictionary { get; set; } 
		public Dictionary<string, MKSS.Model.SensorData> BatchSensorDataLastDictionary { get; set; }
 
		 
		public BatchServices BatchServices { get { return _BatchServices; } }
		/// <summary>
		///  获取最新数据
		/// </summary>
		/// <param name="boardNo"></param>
		/// <param name="slotNo"></param>
		/// <returns></returns>
		public MKSS.Model.SensorData this[int caseNo, int boardNo, int slotNo] {
			get {
				if (CurrentBatch == null) return null;
				string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, caseNo, boardNo, slotNo);
				return BatchSensorDataLastDictionary[F_SensorId];
			}
		}

		public Batch CurrentBatch { get; set; }
		public List<Sensor> CurrentSensors { get; set; }
		public List<SensorData> LastSensorData { get; set; }
		public BiaoDingService( ) {

		}

		~BiaoDingService() {
			BiaoDingSaver.Dispose();
		}

		public DateTime StartBase { get; set; } = DateTime.Now;
		public TimeSpan SpanBase { get; set; } = new TimeSpan();
		public TimeSpan SpanTotal { get; set; }

		  

		public void DeleteBatch(Batch batch)
		{

			try
			{

				bool b1 = _BatchGlobalServices.DeleteById(batch.F_BatchId).Result;

				//删除数据文件
				string data_file = BiaoDingSaver.DbNameof(batch);
				if (System.IO.File.Exists(data_file)) System.IO.File.Delete(data_file);

				_BatchServices = null;
				_SensorServices = null;
				_SensorDataServices = null; 
				CurrentBatch = null; 
				CurrentSensors = null;
				LastSensorData = null;


			}
			catch (Exception ex)
			{
				Log((string.Format("删除检测任务出错 {0} ......", ex.Message)));
				throw ex;
			}


		}


		void LogUI(string msg)
		{
			Log(msg);
		}

		public long NextIdBatch
		{
			get
			{
				int year = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
				long year_from = year * 100 + 0;
				long year_to = (year + 1) * 100 + 1;
				DataTable dt = this._BatchGlobalServices.QueryTable(
					string.Format("select max(F_BatchId) from pd_batch where F_BatchId >{0} and F_BatchId<{1}", year_from, year_to))
					.Result;
				long CurrentIdBatch =
					(dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value)
					? year_from : (Convert.ToInt64(dt.Rows[0][0]));
				CurrentIdBatch++;
				return CurrentIdBatch;
			}
		}

		public void StartBatch()
		{

			try
			{

				//添加之前判断，保留50条数据
				//while (true)
				//{
				//	List<Batch> batches = _BatchGlobalServices.BaseDal.Db.Queryable<Batch>().OrderBy("F_BatchId").ToList();
				//	if (batches.Count < 100)
				//	{
				//		break;
				//	}
				//	DeleteBatch(batches[0]);
				//}

				

				//根据选择开启批次任务
				Batch batch = new Batch()
				{

					F_AgingStartTime = DateTime.Now,
					F_AgingEndTime = 0,
					F_AgingLastUpdateTime = DateTime.Now,
					EnumAgingStatus = EnumAgingStatus.InAging,
					F_BatchId = NextIdBatch,
					F_BatchName = BiaoDingConfgig.Instance.ProductList,
					F_SensorName = BiaoDingConfgig.Instance.ProductList,
					F_SensorTypeName = BiaoDingConfgig.Instance.ProductList,
					F_SensorTypeId = BiaoDingConfgig.Instance.ProductList, 
				};
				if (string.IsNullOrEmpty(batch.F_BatchName)) batch.F_BatchName = batch.F_BatchId.ToString();
				int res = _BatchGlobalServices.Add(batch).Result;

				//复制数据文件，批次数据存储到这个文件中
				string data_file = BiaoDingSaver.DbNameof(batch);
				if (!System.IO.File.Exists(data_file))
					System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\MKSS.APP.BiaodingCommon.sqlite", data_file);

				this.CurrentBatch = batch;

				_BatchServices = new BatchServices();
				_SensorServices = new SensorServices();
				_SensorDataServices = new SensorDataServices();
				//切换要操作的数据库文件
				_BatchServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));
				_SensorServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));
				_SensorDataServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));


				//修改表名
				string create = @"
						CREATE TABLE ""pd_sensordata_" + batch.F_BatchId + @""" (
						  ""F_DataId"" text(50) NOT NULL,
						  ""F_BoardId"" integer NOT NULL,
						  ""F_SensorId"" text NOT NULL,
						  ""F_AddTime"" text NOT NULL,
						  ""V1"" integer,
						  ""V2"" integer,
						  ""V3"" integer,
						  ""V4"" integer,
						  ""V5"" integer,
						  ""V6"" integer,
						  ""V7"" integer,
						  ""V8"" integer,
						  ""V9"" integer,
						  ""V10"" integer,
						  PRIMARY KEY(""F_DataId"")
						);
					";
				var rr = _BatchServices.ExecuteCommand(create).Result;
				//Util.MainDb.GetDbClient().AddQueue(create);
				//SugarTable ca = TypeDescriptor.GetAttributes(typeof(SensorData)).OfType<SugarTable>().FirstOrDefault();
				//TypeDescriptor.AddAttributes(typeof(SensorData), new SugarTable("pd_sensordata_"+ batch.F_BatchId));
				//创建表 执行完数据库就有这个表了 /*设置varchar默认长度为50*/
				//Util.MainDb.GetDbClientAttribute().CodeFirst.SetStringDefaultLength(50).InitTables(typeof(SensorData)); 

				
				BiaoDingSaver.Start(batch);

			}
			catch (Exception ex)
			{
				Log((string.Format("添加检测任务出错 {0} ......", ex.Message)));
				throw ex;
			}


		}

		public void StartSensorData(List<MKSS.APP.UIBiaoDing.Util.Address> addrs) {

            try
            {
				Batch batch = CurrentBatch;
				//创建传感器
				List<Sensor> sens = new List<Sensor>();

				//验证是否存在
				if (CurrentSensors == null) CurrentSensors = new List<Sensor>();
				var dic = CurrentSensors.ToDictionary(w => w.F_SensorId, w => w);
				foreach (MKSS.APP.UIBiaoDing.Util.Address addr in addrs)
				{
					SensorGroupDataModel mdata = UISheBeiBiaoDingViewModel.Intance.GroupDataOfAddress(addr);
					for (int s = 1; s <= 15; s++)
					{
						string sensorId = Sensor.CreateCensorId(batch.F_BatchId, addr.APP, addr.V, s);
						if (!dic.ContainsKey(sensorId))
						{
							Sensor sen = new Sensor()
							{
								F_BatchId = batch.F_BatchId,
								F_BoardCaseId = addr.APP,
								F_BoardId = addr.V,
								F_SensorId = sensorId,
								F_SensorName = batch.F_SensorName,
								F_SensorTypeId = batch.F_SensorTypeId,
								F_SensorTypeName = batch.F_SensorTypeName,
								F_SlotNO = s
							};
							if (mdata != null) {
								sen.F_SerialNO = mdata.ProductTable[s - 1].Serial;
							}
							sens.Add(sen);
						}

					}
				}
				if (sens.Count > 0)
				{
					int xxas = _SensorServices.Add(sens).Result;
					CurrentSensors.AddRange(sens);
				}
			}
            catch (Exception ex)
            {
                throw ex;
            }
			

		}


		public void StopBatch() {

			if (this.CurrentBatch != null) {


				BatchConfig config = new BatchConfig();
				BatchConfig.CopyEntity(BiaoDingConfgig.Instance, config);
				foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
				{
					if (item.Name == BiaoDingConfgig.Instance.ProductList)
					{
						BatchConfig.CopyEntity(item, config);
					}
				}
				config.StaQuallified = UISheBeiBiaoDingViewModel.Intance.StaQuallified;
				config.StaUnqualified = UISheBeiBiaoDingViewModel.Intance.StaUnqualified;
				config.StaTotal = UISheBeiBiaoDingViewModel.Intance.StaTotal;
				config.StaRate = UISheBeiBiaoDingViewModel.Intance.StaRate;

				XmlSerializer serializer = new XmlSerializer(config.GetType());
				string contentBatchConfig = string.Empty;
				//serialize
				using (StringWriter writer = new StringWriter())
				{
					serializer.Serialize(writer, config);
					contentBatchConfig = writer.ToString();
					this.CurrentBatch.F_XmlConfig = contentBatchConfig;
				}

				this.CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
				this.CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
				this.CurrentBatch.F_AgingEndTime = (DateTime.Now - this.CurrentBatch.F_AgingStartTime).TotalSeconds;
				this.CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
				var res = _BatchGlobalServices.Update(this.CurrentBatch).Result;

				if (_BatchServices != null) {
					var find = _BatchServices.QueryById(this.CurrentBatch.F_BatchId).Result;
					if (find == null)
					{
						var findc = _BatchServices.Add(this.CurrentBatch).Result;
					}
					else
					{
						var ress = _BatchServices.Update(this.CurrentBatch).Result;
					}
				}

			}

			BiaoDingSaver.Dispose();
			//_BatchServices = null;
			//_SensorServices = null;
			//_SensorDataServices = null;历史数据查询需要用到，不能 null 
			//CurrentBatch = null;历史数据查询需要用到，不能 null 
			//CurrentSensors = null;
			//LastSensorData = null;

		}

		public void PushData(SensorGroupData pm, ReadMode mode)
		{

			bool savedb  = (ConfigurationManager.AppSettings["savedb"]+"").ToLower()=="true";
			if (!savedb) return;

			StringBuilder sb = new StringBuilder();//Address addr, int Position
			foreach (MKSS.APP.UIBiaoDing.Util.SensorDataX item in pm.SingleAddressData)
			{
				string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, pm.Address.APP, pm.AddressInt, item.Position);
				switch (mode)
				{
					case ReadMode.WX_StartRead:
						if (item.V01 == 0 || item.V01 == 170) break;//170 无用数据不再存储
						sb.Append(
							string.Format(
								"insert into pd_sensordata_{0} (F_DataId, F_SensorId, F_AddTime,F_BoardId ,V1,V2,V3,V4,V5,V6,V7,V8,V9,V10)VALUES ('{1}', '{2}','{3}',{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14});",
								CurrentBatch.F_BatchId,
								Guid.NewGuid().ToString(), F_SensorId, pm.Time, pm.AddressInt,
								item.V01, item.V02, item.V03, item.V04, item.V05, item.V06, item.V07, item.V08, item.V09, item.V10
						));
						break;
					case ReadMode.RX_ReadLiangCheng: 
						sb.Append(
							string.Format(
								"update pd_sensor set V_LiangCheng='{0}' where F_SensorId='{1}';",
								item.LiangCheng, F_SensorId));
						break;
					case ReadMode.RX_ReadVoltageRange:
						sb.Append(
							string.Format(
								"update pd_sensor set V_VoltageRange='{0}' where F_SensorId='{1}';",
								item.DianYaRange, F_SensorId));
						break;
					case ReadMode.RX_ReadAutoAdjustStatus:
						sb.Append(
							string.Format(
								"update pd_sensor set V_AutoAdjustStatus='{0}' where F_SensorId='{1}';",
								item.AutoAdjustState, F_SensorId));
						break;
					case ReadMode.RX_ReadSerialNo:
						sb.Append(
							string.Format(
								"update pd_sensor set V_SerialNo='{0}' where F_SensorId='{1}';",
								item.Serial, F_SensorId));
						break;
					case ReadMode.RX_ReadDeviceDateTime:
						sb.Append(
							string.Format(
								"update pd_sensor set V_DeviceDateTime='{0}' where F_SensorId='{1}';",
								item.Serial, F_SensorId));
						break;
					case ReadMode.RX_ReadOutPutVolage:
						sb.Append(
							string.Format(
								"update pd_sensor set V_OutPutVolage='{0}' where F_SensorId='{1}';",
								item.OutPutVolage, F_SensorId));
						break;
				}
			}

			if (sb.Length > 0)
			{
				BiaoDingSaver.EnqueueTask(sb.ToString());
			}

		}

		public void SetHEGE(Address addr,int Position, bool hege) {
			if (CurrentBatch == null) return;
			string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, addr.APP, addr.V, Position);
			string sql = string.Format("update pd_sensor set V_HEGE='{0}' where F_SensorId='{1}';", hege.ToString(), F_SensorId);
			
			if (BiaoDingSaver.Running)
			{
				BiaoDingSaver.EnqueueTask(sql);
			}
			else
			{

				var xxx = _SensorServices.ExecuteCommand(sql).Result;
				BatchConfig config = null;
				using (StringReader sr = new StringReader(CurrentBatch.F_XmlConfig))
				{
					XmlSerializer serializer1 = new XmlSerializer(typeof(BatchConfig));
					config = serializer1.Deserialize(sr) as BatchConfig;
				}

				config.StaTotal = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.StaTotal;
				config.StaUnqualified = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.StaUnqualified;
				config.StaQuallified = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.StaQuallified;
				config.StaRate = UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.StaRate == double.NaN ? "" : UISheBeiBiaoDingViewModel.Intance.ConnectionPool.Qualified.StaRate.ToString("P1");
				
				
				 
				XmlSerializer serializer = new XmlSerializer(config.GetType());
				using (StringWriter writer = new StringWriter())
				{
					serializer.Serialize(writer, config);
					string contentBatchConfig = writer.ToString();
					this.CurrentBatch.F_XmlConfig = contentBatchConfig;
				}
				var ress11 = _BatchGlobalServices.Update(this.CurrentBatch).Result;
				var ress = _BatchServices.Update(this.CurrentBatch).Result;

			}
		}


		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public DataTable QueryDataOf(int addr,string pname) {
			if (_SensorDataServices == null || CurrentBatch==null) return null;
			try
			{
				DataTable table = _SensorDataServices.QueryTable("select F_AddTime D," + pname + " V,F_SensorId S from pd_sensordata_" + CurrentBatch.F_BatchId + " where F_BoardId="+ addr + " order by F_SensorId").Result;
				return table;

			}
			catch (Exception ex)
			{
				Log(string.Format("刷新数据时出错 {0}", ex.Message));
				Log(ex.Message);
				return null;
			}
			finally
			{
				 
			}

		}

		public DataTable QueryDataOfTime(int addr, string pname)
		{
			if (_SensorDataServices == null) return null;
			try
			{
				DataTable table = _SensorDataServices.QueryTable("select F_AddTime D," + pname + " V,F_SensorId S from pd_sensordata_" + CurrentBatch.F_BatchId + " where F_BoardId=" + addr + " order by F_AddTime").Result;
				return table;

			}
			catch (Exception ex)
			{
				Log(string.Format("刷新数据时出错 {0}", ex.Message));
				Log(ex.Message);
				return null;
			}
			finally
			{

			}

		}


		public void OpenBatch(Batch batch) {

		
			CurrentBatch = batch;
			_BatchServices = new BatchServices();
			_SensorServices = new SensorServices();
			_SensorDataServices = new SensorDataServices();
			//切换要操作的数据库文件
			_BatchServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));
			_SensorServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));
			_SensorDataServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", BiaoDingSaver.DbNameof(batch));
			CurrentSensors = _SensorServices.QuerySql(@" select * from pd_sensor").Result;
			LastSensorData = _SensorDataServices.QuerySql(
					@"
					select b.* from 
						(SELECT F_SensorId,MAX(F_AddTime) F_AddTime  FROM pd_sensordata_" + batch.F_BatchId + @" group by F_SensorId) a
					left join pd_sensordata_" + batch.F_BatchId + @" b
						on a.F_SensorId = b.F_SensorId and a.F_AddTime = b.F_AddTime
					left join pd_sensor s
						on a.F_SensorId = s.F_SensorId
					").Result;
		}
		 



		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public List<BatchExt> QueryAllBatch()
		{
			try
			{
				List<BatchExt> vre = new List<BatchExt>();
                foreach (var item in this._BatchGlobalServices.Query().Result)
                {
					vre.Add(new BatchExt(item));
				}
				return vre.OrderByDescending(w=>w.F_BatchId).ToList();
			}
			catch (Exception ex)
			{
				Log(string.Format("刷新数据时出错 {0}", ex.Message));
				Log(ex.Message);
				return new List<BatchExt>();
			}
			finally
			{

			}

		}

		void Log(string str)
        {

        }
 

	}

	public class BatchExt : Batch {
		Batch Batch { get; set; }
		BatchConfig BatchConfig { get; set; }
		public int StaQuallified { get; set; }
		public int StaUnqualified { get; set; }
		public string StaRate { get; set; }
		public double StaTotal { get; set; }

		public string SerialPorts { get; set; }
		public string TCPIP { get; set; }
		public int PortsModeSelect { get; set; }
		public string SensorGrougAddress { get; set; }
		public bool StaFilterEmpSata { get; set; }
		public int RefreshInterval { get; set; }
		public bool StaZeroSpanTitleExchange { get; set; }
		public double YMin { get; set; } = -100;
		public double YMax { get; set; } = 10000;
		public double XMax { get; set; } = 620;
		public double XMin { get; set; } = -20;
		public BatchExt() { 
		
		}
		public BatchExt(Batch v) {
			Batch = v;
			if (string.IsNullOrEmpty(Batch.F_XmlConfig))
			{
				BatchConfig = new BatchConfig();
			}
			else {
				using (StringReader sr = new StringReader(Batch.F_XmlConfig))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(BatchConfig));
					BatchConfig = serializer.Deserialize(sr) as BatchConfig;
					BatchConfig.CopyEntity(BatchConfig, this);
				}
			}
			BatchConfig.CopyEntity(v,this);
		}

        public override bool Equals(object obj)
        {
            return ToString().Equals(obj+"");
        }

        public override int GetHashCode()
        {
            return (int)this.F_BatchId;
        }
        public override string ToString()
        {
            return this.F_BatchId.ToString();
        }

    }

	public class BatchConfig : ProductConfig {
		public int StaQuallified { get; set; }
		public int StaUnqualified { get; set; }
		public string StaRate { get; set; }
		public double StaTotal { get; set; } 
		public string SerialPorts { get; set; }
		public string TCPIP { get; set; }
		public int PortsModeSelect { get; set; }
		public string SensorGrougAddress { get; set; }
		public bool StaFilterEmpSata { get; set; }
		public int RefreshInterval { get; set; }
		public bool StaZeroSpanTitleExchange { get; set; }
		public double YMin { get; set; } = -100;
		public double YMax { get; set; } = 10000;
		public double XMax { get; set; } = 620;
		public double XMin { get; set; } = -20;
		public static void CopyEntity(object src, object target)
		{
			foreach (PropertyInfo item in src.GetType().GetProperties())
			{
				PropertyInfo tarItem = target.GetType().GetProperty(item.Name);
				if (tarItem != null) { 
					if (tarItem.CanWrite) tarItem.SetValue(target, item.GetValue(src, null), null);
				}
			}
		}

	}
}
 
