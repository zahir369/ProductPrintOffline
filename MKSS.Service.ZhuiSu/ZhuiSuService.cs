using MKSS.Service.ZhuiSu.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.ZhuiSu;
using MKSS.Service.ZhuiSu.Util;
using MKSS.Services;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using System.Diagnostics;
using System.Data;
using System.Threading;

namespace MKSS.Service.ZhuiSu
{


	[LogTagClass(Title = "检测任务池")]
	public class ZhuiSuService
	{

		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 1000 ;//200毫秒钟读一次数据

		DateTime StartTime { get; set; }
		public event NewDataEventHandler OnNewData;
		BatchServices _BatchServices = new BatchServices();
		BoardServices _BoardServices = new BoardServices();
		BoardCaseServices _BoardCaseServices = new BoardCaseServices();
		BatchRelServices _BatchRelServices = new BatchRelServices();
		SensorServices _SensorServices = new SensorServices();
		DataBusServices _DataBusServices = new DataBusServices();
		SensorDataServices _SensorDataServices = new SensorDataServices();

		Dictionary<long, BoardCase> _BoardCaseDic { get; set; }
		Dictionary<long, Dictionary<string, Sensor>> BatchSensorDictionary { get; set; }
		public Dictionary<long, Dictionary<string, MKSS.Model.SensorData>> BatchSensorDataDictionary { get; set; }
		/// <summary>
		///   连接池
		/// </summary>
		Dictionary<long, ModBusBoardConnection> ConnectionPool = new Dictionary<long, ModBusBoardConnection>();


		public ZhuiSuService( ) {

			ConnectionPool = new Dictionary<long, ModBusBoardConnection>();
			BatchSensorDictionary = new Dictionary<long, Dictionary<string, Sensor>>();
			BatchSensorDataDictionary = new Dictionary<long, Dictionary<string, MKSS.Model.SensorData>>();

			SM_DataEventSaver.Start();

			//初始连接池
			Dictionary<long, DataBus> _DataBusDic =
				_DataBusServices.QuerySql(string.Format("select * from pd_databus "))
				.Result.ToDictionary(w => w.F_DataBusId, w => w);
			foreach (var item in _DataBusDic.Values)
			{
				ModBusBoardConnection x = new ModBusBoardConnection(item);
				this.ConnectionPool.Add(item.F_DataBusId, x);
			}

			ULogger.Log(string.Format("初始连接池 {0} 个......", _DataBusDic.Count));


			_BoardCaseDic =
				_BoardCaseServices.QuerySql(string.Format("select * from pd_boardcase "))
				.Result.ToDictionary(w => w.F_BoardCaseId, w => w);


		}

		~ZhuiSuService() {
			SM_DataEventSaver.Dispose();
		}

		Env _EnvLast = null;
		bool RefreshTaskDoing = false;
		[LogTagClass(Title = "刷新检测任务")]
		public void RefreshTask() {


			if (RefreshTaskDoing) return;
			RefreshTaskDoing = true;



			try
			{

				//刷新通信链路状态
				Dictionary<long, DataBus> _DataBusDic =
					_DataBusServices.QuerySql(string.Format("select * from pd_databus "))
					.Result.ToDictionary(w => w.F_DataBusId, w => w);
				foreach (var item in _DataBusDic.Values)
				{
					if (this.ConnectionPool.ContainsKey(item.F_DataBusId))
					{
						ModBusBoardConnection conn = this.ConnectionPool[item.F_DataBusId];
						conn.DataBus.F_UseStatus = item.F_UseStatus;
						conn.DataBus.F_UseStartTime = item.F_UseStartTime;
						conn.DataBus.F_UseEndTime = item.F_UseEndTime;
					}
				}

				ModBusBoardConnection connEnv = null;
				try
				{
					//环形气箱数据采集入库
					connEnv = this.ConnectionPool.FirstOrDefault(w => w.Value.DataBus.F_DataBusName == "循环气箱环境数据采集通信").Value;
					if (connEnv != null)
					{
						connEnv.ModBusBoard.OpenTcp(connEnv.IP, connEnv.PORT);
						if (connEnv.ModBusBoard.IsOpen)
						{
							float[] ds = connEnv.ModBusBoard.ReadEnv().Result;
							if (ds != null) {
								Env _Env = new Env() { 
									F_EnvId = DateTime.Now.Ticks, 
									F_DateTime = DateTime.Now, F_EndDateTime = DateTime.Now.AddSeconds(30),
									F_Temperature = ds[0], F_Humidity = ds[1] };
								if (_EnvLast == null || Math.Abs(_EnvLast.F_Temperature - _Env.F_Temperature) > 0.1 || Math.Abs(_EnvLast.F_Humidity - _Env.F_Humidity) > 0.1)
								{
									_DataBusServices.BaseDal.Db.Insertable<Env>(_Env).ExecuteCommand();
									if (_EnvLast != null) {
										_EnvLast.F_EndDateTime = DateTime.Now;
										_DataBusServices.BaseDal.Db.Updateable<Env>(_EnvLast).ExecuteCommand();
									}
									_EnvLast = _Env;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("采集环境数据出错 {0} ......", ex.Message));
				}
				finally {
					if (connEnv != null && connEnv.IsOpen) {
						connEnv.ModBusBoard.CloseTCPIP();
					}
				}

				Dictionary<long, long> board_rel_case = null;
				Dictionary<long, Board> _BoardDic = null;
				_BoardCaseDic =
					_BoardCaseServices.QuerySql(string.Format("select * from pd_boardcase "))
					.Result.ToDictionary(w => w.F_BoardCaseId, w => w);
				board_rel_case =
					_BoardServices.QuerySql(string.Format("select * from pd_board"))
					.Result.ToDictionary(w => w.F_BoardId, w => w.F_BoarCaseId);
				_BoardDic =
					_BoardServices.QuerySql(string.Format("select * from pd_board "))
					.Result.ToDictionary(w => w.F_BoardId, w => w);
				//ULogger.Log(string.Format("初始实验柜 {0} 个......", _BoardCaseDic.Count));

				int aging = (int)EnumAgingStatus.InAging;
				List<Batch> _Batchs = _BatchServices.QuerySql(string.Format("select * from pd_batch where F_AgingStatus='{0}'", aging)).Result;
				//ULogger.Log(string.Format("刷新检测任务，找到 {0} 个批次需要检测......", _Batchs.Count));


				//刷新任务状态
				foreach (ModBusBoardConnection conn in ConnectionPool.Values)
				{
					FinishBatchAssert(conn);
				}

				//删除完成的任务
				foreach (ModBusBoardConnection conn in ConnectionPool.Values)
				{
					foreach (Board b in conn.BoardList.Keys.ToList())
					{
						var bid = (conn.BoardList[b].F_BatchId);
						if (_Batchs.Count(w => w.F_BatchId == bid) == 0)
						{
							conn.BoardList.Remove(b);
						}
					}
					if (conn.BoardList.Count == 0)
					{
						if (!conn.IsOpen)
						{
							conn.ModBusBoard.IsRead = false;//退出读取循环
							conn.ModBusBoard.CloseTCPIP();
						}
					}
				}


				List<BoardCase> _BoardCaseList = new List<BoardCase>();
				List<Board> add_task = new List<Board>();
				foreach (Batch _Batch in _Batchs)
				{

					//是否已经在任务池中
					if (ConnectionPool.Values.Count(w => w.BoardList.Values.Count(b => b.F_BatchId == _Batch.F_BatchId) > 0) > 0)
					{
						ULogger.Log(string.Format("已经在任务池中：{0}......", _Batch.F_BatchName));
						continue;
					}

					ULogger.Log(string.Format("批次需要检测：{0}......", _Batch.F_BatchName));
					List<BatchRel> _BatchRels = _BatchRelServices.QuerySql(string.Format("select * from pd_batch_rel where F_BatchId='{0}'", _Batch.F_BatchId)).Result;
					List<Sensor> _Sensors = _SensorServices.QuerySql(string.Format("select * from pd_sensor where F_BatchId='{0}'", _Batch.F_BatchId)).Result;

					string sql1 = "SELECT distinct t.F_DataValue,t.F_SensorId,t.F_AddTime FROM (SELECT F_SensorId,max(F_AddTime) as F_AddTime FROM `pd_sensordata_" + _Batch.F_BatchId + "` where F_DataValue>170 GROUP BY F_SensorId) a LEFT JOIN `pd_sensordata_" + _Batch.F_BatchId + "` t ON t.F_SensorId=a.F_SensorId and t.F_AddTime=a.F_AddTime";
					DataTable tab1 = _SensorServices.QueryTable(sql1).Result;
					Dictionary<string, DataRow> dic1 = new Dictionary<string, DataRow>();
					for (int i = 0; i < tab1.Rows.Count; i++)
					{
						DataRow dr = tab1.Rows[i];
						string F_SensorId = dr["F_SensorId"] + "";
						dic1.Add(F_SensorId, dr);
					}

					if (BatchSensorDictionary.Keys.Count(w => w == _Batch.F_BatchId) == 0)
					{
						BatchSensorDictionary.Add(_Batch.F_BatchId, new Dictionary<string, Sensor>());
						BatchSensorDataDictionary.Add(_Batch.F_BatchId, new Dictionary<string, MKSS.Model.SensorData>());
					}
					foreach (var item in _Sensors)
					{
						if (!BatchSensorDictionary[_Batch.F_BatchId].ContainsKey(item.F_SensorId))
						{
							BatchSensorDictionary[_Batch.F_BatchId].Add(item.F_SensorId, item);
							BatchSensorDataDictionary[_Batch.F_BatchId].Add(
									item.F_SensorId,
									new MKSS.Model.SensorData()
									{
										F_SensorId = item.F_SensorId,
									});
						}

						var t_data = BatchSensorDataDictionary[_Batch.F_BatchId][item.F_SensorId];
						if (dic1.ContainsKey(item.F_SensorId))
						{
							t_data.F_DataValue = Convert.ToInt32(dic1[item.F_SensorId]["F_DataValue"]);
							t_data.F_AddTime = Convert.ToDouble(dic1[item.F_SensorId]["F_AddTime"]);
						}
						else
						{
							t_data.F_DataValue = 0;
						}

					}

					foreach (BatchRel r in _BatchRels)
					{

						//已经结束的板子直接跳过
						if (r.EnumUseInFree != EnumUseInFree.InUse) continue;

						//挑出有效的柜子
						if (board_rel_case.ContainsKey(r.F_BoardId))
						{
							long case_id = board_rel_case[r.F_BoardId];
							if (_BoardCaseDic.ContainsKey(case_id))
							{
								BoardCase _BoardCase = _BoardCaseDic[case_id];
								if (!_BoardCaseList.Contains(_BoardCase))
								{
									//已经结束的柜子直接跳过
									if (_BoardCase.EnumUseInFree != EnumUseInFree.InUse) continue;
									_BoardCaseList.Add(_BoardCase);
								}

							}
						}

						//挑出有效的板子
						if (_BoardDic.ContainsKey(r.F_BoardId))
						{

							Board _Board = _BoardDic[r.F_BoardId];
							//已经结束的板子直接跳过
							if (_Board.EnumUseInFree != EnumUseInFree.InUse) continue;



							//将批次信息保存到连接中
							{
								if (ConnectionPool.ContainsKey(_Board.DataBusInt))
								{
									if (ConnectionPool[_Board.DataBusInt].BoardList.ContainsKey(_Board))
									{
										//已经添加过，更新
										ConnectionPool[_Board.DataBusInt].BoardList[_Board] = (_Batch);
										add_task.Add(_Board);
									}
									else
									{
										ConnectionPool[_Board.DataBusInt].BoardList.Add(_Board, _Batch);
										add_task.Add(_Board);
									}

								}
							}

						}

					}

				}


				string strResult = _BoardCaseList.Count == 0 ? "0" : _BoardCaseList.Select(w => w.F_BoardCaseAddress + "").Aggregate((a, b) => a + "#" + b + "#");
				ULogger.Log(string.Format("检测任务刷新完成，检测到{0}柜{1}托盘新任务......", strResult, add_task.Count));


				bool need = false;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					if (Connection.IsOpen) continue;
					if (Connection.BoardList.Count == 0) continue;
					if (Connection.LastPingFailer != DateTime.Now && (DateTime.Now - Connection.LastPingFailer < new TimeSpan(0, 0, 30)))
					{
						continue;//半分钟内不重新连接，上次网络测试失败时间
					}
					need = true;
				}
				if (need)
				{
					ReadVoltageRecycle();
				}

				//foreach (ModBusBoardConnection Connection in ConnectionPool.Values) {
				//	Connection.BoardList.Clear();
				//}

			}
			catch (Exception ex)
			{

				ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));

			}
			finally {

				RefreshTaskDoing = false;

			}
			

		}


		[LogTagClass(Title = "创建网络连接")]
		async Task Open()
		{
			 
			try
			{

                foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
                {
					 
				}

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("连接出错 {0} ......", ex.Message));
			}

		}


		[LogTagClass(Title = "断开网络连接")]
		async Task Close()
		{

			try
			{

				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{ 
					if (!Connection.IsOpen) continue;
					int port = Connection.PORT;
					Connection.ModBusBoard.CloseTCPIP();
				}

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("关闭连接出错 {0} ......", ex.Message));
			}

		}
		 
		[LogTagClass(Title = "创建读取任务")]
		void ReadVoltageRecycle()
		{

			try
			{
				 
				//循环读取状态
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					if (Connection.IsOpen) continue;
					if (Connection.BoardList.Count == 0) continue;
					if (Connection.LastPingFailer != DateTime.Now && (DateTime.Now - Connection.LastPingFailer < new TimeSpan(0, 0, 30)))
					{
						continue;//半分钟内不重新连接，上次网络测试失败时间
					}
					ReadVoltageRecycleInner(Connection).Start();
				} 

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取检测数据出错 {0} ......", ex.Message));
			}
			finally {

			}

		}


		async Task ReadVoltageRecycleInner(ModBusBoardConnection Connection)
		{

			try
			{

				if (Connection.BoardList.Count == 0) return;
				if (Connection.IsOpen) return;
				
				Ping pingSender = new Ping();
				PingReply reply = pingSender.Send(Connection.IP, 120);//第一个参数为ip地址，第二个参数为ping的时间
				if (reply.Status != IPStatus.Success)
				{
					ULogger.Log(string.Format("IP {0} 不通", Connection.IP));
					Connection.LastPingFailer = DateTime.Now;
					return;
				}
				int port = Connection.PORT;

				DataBus ds = Connection.DataBus;
				if (Connection.DataBus.EnumUseStatus != DataBusUseStatus.None)
				{
					//ULogger.Log(string.Format("通道 {0}:{1} 锁定占用中，{2} {3}", Connection.IP, Connection.PORT, ds.EnumUseStatus, ds.F_UseStartTime));
					//return;
				} 

				//ULogger.Log(string.Format("正在连接 {0}:{1} ......", Connection.IP, Connection.PORT));
				try
				{
					//占用通道
					//ds.EnumUseStatus = DataBusUseStatus.ZhuiSu;
					//ds.F_UseStartTime = DateTime.Now;
					// _DataBusServices.Update(ds).Start();
					Batch _bat = Connection.BoardList.Values.FirstOrDefault();
					if (_bat == null)
					{
						ULogger.Log(string.Format("批次空 {0}:{1} ", Connection.IP, Connection.PORT ));
						return;
					}
					Connection.ModBusBoard.ReadSpeed = (int)(_bat.F_Intraval * 1000);
					Connection.ModBusBoard.OpenTcp(Connection.IP, port);
					if (Connection.ModBusBoard.IsOpen)
					{
						Connection.ModBusBoard.DataEventAging -= this.SM_DataEvent;
						Connection.ModBusBoard.DataEventAging += this.SM_DataEvent;
					}
					//ULogger.Log(string.Format("连接 {0}:{1} {2}......", Connection.IP, Connection.PORT, Connection.ModBusBoard.IsOpen ? "成功" : "失败"));
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("端口 {0}:{1} 不通，{2}", Connection.IP, Connection.PORT, ex.Message));
				}  

				Connection.ModBusBoard.ReadVoltageRecycle().Start();

			}
			catch (Exception)
			{
				throw;
			}
			finally
			{

				DataBus ds = Connection.DataBus;
				//释放通道
				ds.EnumUseStatus = DataBusUseStatus.None;
				ds.F_UseStartTime = DateTime.Now;
				_DataBusServices.Update(ds);

			}
		}



		public DateTime ReadVoltageOnceStart = DateTime.Now;
		[LogTagClass(Title = "创建读取任务")]
		public async Task ReadVoltageOnce()
		{

			try
			{
				 
				ReadVoltageOnceStart = DateTime.Now;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					var t1 = new Task(() => ReadVoltageOnceInner(Connection));
					t1.Start();
				}
				ULogger.Log(string.Format("正在读取......" ));
				 
			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取检测数据出错 {0} ......", ex.Message));
			}

		}


		void ReadVoltageOnceInner(ModBusBoardConnection Connection) {
			try
			{

				if (Connection.BoardList.Count == 0) return;

				if (Connection.IsOpen) return;
				Ping pingSender = new Ping();
				PingReply reply = pingSender.Send(Connection.IP, 120);//第一个参数为ip地址，第二个参数为ping的时间
				if (reply.Status != IPStatus.Success)
				{
					ULogger.Log(string.Format("IP {0} 不通", Connection.IP));
					return;
				}
				int port = Connection.PORT;


				DataBus ds = Connection.DataBus;
				if (Connection.DataBus.EnumUseStatus != DataBusUseStatus.None)
				{
					//ULogger.Log(string.Format("通道 {0}:{1} 锁定占用中，{2} {3}", Connection.IP, Connection.PORT, ds.EnumUseStatus, ds.F_UseStartTime));
					//return;
				}

				//ULogger.Log(string.Format("正在连接 {0}:{1} ......", Connection.IP, Connection.PORT));
				try
				{

					//占用通道
					ds.EnumUseStatus = DataBusUseStatus.ZhuiSu;
					ds.F_UseStartTime = DateTime.Now;
					var c1 = _DataBusServices.Update(ds).Result;
					Connection.ModBusBoard.OpenTcp(Connection.IP, port);
					if (Connection.ModBusBoard.IsOpen)
					{
						Connection.ModBusBoard.DataEventAging -= this.SM_DataEvent;
						Connection.ModBusBoard.DataEventAging += this.SM_DataEvent;
					}
					//ULogger.Log(string.Format("连接 {0}:{1} {2}......", Connection.IP, Connection.PORT, Connection.ModBusBoard.IsOpen ? "成功" : "失败"));
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("端口 {0}:{1} 不通，{2}", Connection.IP, Connection.PORT, ex.Message));
				}
				 
				FinishBatchAssert(Connection);//判断是否结束
											  //ULogger.Log(string.Format("读取任务 {0} 个xxxxx......", count));
				Connection.ModBusBoard.ReadVoltageOnce().Wait();
				FinishBatchAssert(Connection);//判断是否结束

				if (!Connection.IsOpen) Connection.ModBusBoard.CloseTCPIP();


			}
			catch (Exception)
			{
				throw;
			}
			finally
			{

				DataBus ds = Connection.DataBus;
				//释放通道
				ds.EnumUseStatus = DataBusUseStatus.None;
				ds.F_UseStartTime = DateTime.Now;
				_DataBusServices.Update(ds);

			}
		}


		/// <summary>
		///  检查已是否完成或删除任务，终止
		/// </summary>
		/// <param name="DataSrc"></param>
		public void FinishBatchAssert(ModBusBoardConnection Connection)
		{

			foreach (Board _Board in Connection.BoardList.Keys.ToArray())
			{
				try
				{
					Batch _Batch = Connection.BoardList[_Board];
					BoardCase _BoardCase = this._BoardCaseDic.Values.FirstOrDefault(w => w.F_BoardCaseId == _Board.F_BoarCaseId);
					FinishBatch(_Batch, _Board, _BoardCase);
				}
				catch (Exception ex)
				{
					ULogger.Error(string.Format("结束检测出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}


		}


		public void FinishBatch(Batch _Batch,Board _Board,BoardCase _BoardCase)
		{

			{
				try
				{
					{

						//判断 是否需要结束
						if (_Batch.F_AgingEndTime <= DateTime.Now)
						{

							//结束板子
							_Board.F_LastUseUpdate = DateTime.Now;
							_Board.EnumUseType = EnumUseType.ZhuiSu;
							_Board.EnumUseInFree = EnumUseInFree.Free;
							bool ress = _BoardServices.Update(_Board).Result;

							//更新关联关系状态
							List<BatchRel> rels = _BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId && w.F_BoardId == _Board.F_BoardId)).Result;
							foreach (var r in rels)
							{
								r.F_LastUseUpdate = DateTime.Now;
								r.EnumUseInFree = EnumUseInFree.Free;
								ress = _BatchRelServices.Update(r).Result;
							}

							//判断柜子是否有使用中板子
							int count = _BoardServices.Query(wx => wx.F_BoarCaseId == _BoardCase.F_BoardCaseId && wx.F_UseStatus == 1).Result.Count;
							if (count == 0)
							{
								_BoardCase.EnumUseInFree = EnumUseInFree.Free;
								_BoardCase.F_LastUseUpdate = DateTime.Now;
								ress = _BoardCaseServices.Update(_BoardCase).Result;
							}

							//判断是否所有板子已经结束
							rels = _BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId && w.F_UseStatus == 1)).Result;
							if (rels.Count == 0)
							{
								_Batch.F_AgingEndTimeActual = DateTime.Now;
								_Batch.F_AgingLastUpdateTime = DateTime.Now;
								_Batch.EnumAgingStatus = EnumAgingStatus.Finished;
								ress = _BatchServices.Update(_Batch).Result;
								this.BatchSensorDictionary.Remove(_Batch.F_BatchId);
								this.BatchSensorDataDictionary.Remove(_Batch.F_BatchId);
							}

						}

					}

				}
				catch (Exception ex)
				{
					ULogger.Error(string.Format("结束检测出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}

		}


		[LogTagClass(Title = "接收检测数据")]
		private void SM_DataEvent(object sender, ModBusBoard.DataEventAgingArgs e)
		{ 
			foreach (Board _Board in e.ProductData.Connection.BoardList.Keys.ToList())
            {
				if (_Board.AddrByte==((byte)e.Address))
				{

					Batch _Batch = e.ProductData.Connection.BoardList[_Board];
					BoardCase _BoardCase = this._BoardCaseDic[_Board.F_BoarCaseId];

					List<MKSS.Model.Sensor> sens_insert = new List<MKSS.Model.Sensor>();
					List<MKSS.Model.SensorData> datas_realtime_insert = new List<MKSS.Model.SensorData>();
					List<MKSS.Model.SensorData> datas_history = new List<MKSS.Model.SensorData>();
					if (!this.BatchSensorDictionary.ContainsKey(_Batch.F_BatchId)) 
						this.BatchSensorDictionary.Add(_Batch.F_BatchId, new Dictionary<string, Sensor>());
					if (!this.BatchSensorDataDictionary.ContainsKey(_Batch.F_BatchId))
						this.BatchSensorDataDictionary.Add(_Batch.F_BatchId, new Dictionary<string, MKSS.Model.SensorData>());
					Dictionary<string, Sensor> sens = this.BatchSensorDictionary[_Batch.F_BatchId];
					Dictionary<string, MKSS.Model.SensorData> sensData = this.BatchSensorDataDictionary[_Batch.F_BatchId];


					int start = 0;
					for (int i = 0; i < e.ProductData.SingleAddressData.Length; i++)
					{

						int F_SlotNO = start + i + 1;
						DataAgingSensor item = e.ProductData.SingleAddressData[i];

						//插入数据库
						string F_SensorId = Sensor.CreateCensorId(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _Board.F_FloorNO, F_SlotNO);
						MKSS.Model.SensorData data = new MKSS.Model.SensorData()
						{
							F_AddTime = (e.ProductData.Time - _Batch.F_AgingStartTime).TotalSeconds,
							F_DataValue = item.Value.Value,
							F_SensorId = F_SensorId,
							F_DataId = Guid.NewGuid().ToString()
						};
						if (_EnvLast != null && (DateTime.Now - _EnvLast.F_EndDateTime).TotalSeconds < 10) {
							data.HTDateTime = _EnvLast.F_DateTime;
							data.Temperature = _EnvLast.F_Temperature;
							data.Humidity = _EnvLast.F_Humidity;
						}
						datas_history.Add(data);
						if (!sens.ContainsKey(F_SensorId))
						{
							MKSS.Model.Sensor sen = new MKSS.Model.Sensor()
							{
								F_SensorId = F_SensorId,
								F_BatchId = _Batch.F_BatchId,
								F_BoardCaseId = _BoardCase.F_BoardCaseId,
								F_BoardId = _Board.F_BoardId,
								F_SensorName = _Batch.F_SensorName,
								F_SensorTypeId = _Batch.F_SensorTypeId,
								F_SensorTypeName = _Batch.F_SensorTypeName,
								F_SlotNO = F_SlotNO
							};
							sens_insert.Add(sen);
							sensData.Add(F_SensorId, data);
						}
						else {

							//跳变 170 的处理
							if (data.F_DataValueEmp)
							{
								sensData[F_SensorId].F_DataValueEmpCount++;
								if (!sensData[F_SensorId].F_DataValueEmp && sensData[F_SensorId].F_DataValueEmpCount <= 5)
								{
									//历史数据不空，空值连续 5 次以内，不认可空值
								}
								else
								{
									sensData[F_SensorId].F_DataValue = data.F_DataValue;
									sensData[F_SensorId].F_AddTime = data.F_AddTime; 
								}
							}
							else {
								sensData[F_SensorId].F_DataValue = data.F_DataValue;
								sensData[F_SensorId].F_AddTime = data.F_AddTime;
								sensData[F_SensorId].F_DataValueEmpCount = 0;
							}
							 
							
						}

					}

					if (datas_history.Count > 0)
					{ 
						if (OnNewData != null) OnNewData(_Batch, datas_history);
					}


					if (sens_insert.Count > 0)
					{
						foreach (var item in sens_insert)
						{
							sens.Add(item.F_SensorId, item);
						}
						this._SensorServices.Add(sens_insert).Wait();
					}


					//Task.Run(delegate ()
					//{

					//}).Start();
					try
					{
						int insert_act = 0;
						if (datas_history.Count > 0)
						{

							if (OnNewData != null) OnNewData(_Batch, datas_history);

							StringBuilder sb = new StringBuilder();
							foreach (var item in datas_history)
							{
								if (item.F_DataValue == 0 || item.F_DataValue == 170) continue;//170 无用数据不再存储
								insert_act++;
								sb.Append(
									string.Format(
										"insert into pd_sensordata_{0} (F_DataId, F_SensorId, F_DataValue,F_AddTime)VALUES ('{1}', '{2}', {3},'{4}');",
										_Batch.F_BatchId, item.F_DataId, item.F_SensorId, item.F_DataValue, item.F_AddTime 
								));
							}

							sb.Append(
									string.Format(
										"update pd_batch set F_AgingLastUpdateTime='{1}' where F_BatchId={0};",
										_Batch.F_BatchId, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")
								));

							if (sb.Length > 0)
							{
								SM_DataEventSaver.EnqueueTask(sb.ToString());
								//_SensorServices.QueryTable(sb.ToString()).Start();
							}

						}
					}
					catch (Exception ex)
					{
						ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
					}


					ULogger.Log(string.Format("收到数据 {0},{1} {4}=> {2}-{3} ,{5}秒......",
						e.ProductData.Connection, 
						_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO,
						e.ProductData.SingleAddressData.Min(w => w.Value.Value), 
						e.ProductData.SingleAddressData.Max(w => w.Value.Value),
						string.Format("插入 pd_sensordata_{0} {2}/{1} 条", _Batch.F_BatchId, datas_history.Count, datas_history.Count),
						(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
						);


				}
			}

		}

		/// <summary>
		///  委托数据
		/// </summary>
		/// <param name="_Batch">批次</param>
		/// <param name="datas_history">数据历史</param>
		public delegate void NewDataEventHandler(Batch _Batch, List<MKSS.Model.SensorData> datas_history);

	}

	public class SM_DataEventSaver
	{
		// 任务队列
		static Queue<string> _tasks = new Queue<string>();

		// 为保证线程安全，使用一个锁来保护_task的访问
		readonly static object _locker = new object();

		// 通过 _wh 给工作线程发信号
		static EventWaitHandle _wh = new AutoResetEvent(false);
		static SensorServices _SensorServices = new SensorServices();
		static Thread _worker;

		public static void Start( )
		{
			// 任务开始，启动工作线程
			_worker = new Thread(Work);
			_worker.Start();
  
		}

		/// <summary>执行工作</summary>
		static void Work()
		{
			while (true)
			{

				List<string> works = new List<string>(); 
				lock (_locker)
				{
					while (works.Count <= 6 && _tasks.Count > 0)
					{
						string work = _tasks.Dequeue(); // 有任务时，出列任务
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.Add(work);
					}
				}

				if (works.Count>0)
					SaveData(works);  // 任务不为null时，处理并保存数据
				else
					_wh.WaitOne();   // 没有任务了，等待信号
			}
		}

		/// <summary>插入任务</summary>
		public static void EnqueueTask(string task)
		{
			lock (_locker)
				_tasks.Enqueue(task);  // 向队列中插入任务 

			_wh.Set();  // 给工作线程发信号
		}

		/// <summary>结束释放</summary>
		public static void Dispose()
		{
			EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
			_worker.Join();         // 等待工作线程完成
			_wh.Close();            // 释放资源
		}


		/// <summary>处理保存</summary>
		static void SaveData(List<string> data)
		{
            try
            {
				string s = "";
                foreach (var item in data)
                {
					s = s + item;
				} 
				var xxx = _SensorServices.QueryTable(s).Result;
			}
            catch (Exception ex)
            {
				ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
			}
		}
	}
	 
}
 
