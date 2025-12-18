using MKSS.Service.LaoHua.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.LaoHua;
using MKSS.Service.LaoHua.Util;
using MKSS.Services;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using System.Diagnostics;
using System.Data;
using System.Threading;
using MKSS.APP.LaoHua;

namespace MKSS.Service.LaoHua
{


    [LogTagClass(Title = "老化任务池")]
	public partial class LaohuaService
	{

		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 10;//200毫秒钟读一次数据

		DateTime StartTime { get; set; }
		BatchServices _BatchServices = new BatchServices();
		BoardServices _BoardServices = new BoardServices();
		BoardCaseServices _BoardCaseServices = new BoardCaseServices();
		BatchRelServices _BatchRelServices = new BatchRelServices();
		SensorServices _SensorServices = new SensorServices();
		DataBusServices _DataBusServices = new DataBusServices();
		SensorDataServices _SensorDataServices = new SensorDataServices();
		LaoHuaGuanChaDataTimer _LaoHuaGuanChaDataTimer = new LaoHuaGuanChaDataTimer();
		public event NewDataEventHandler OnNewData;

		Dictionary<long, BoardCase> _BoardCaseDic { get; set; }
		Dictionary<long, Dictionary<string, Sensor>> BatchSensorDictionary { get; set; }

		/// <summary> 
		///  传感器实时数据
		/// </summary>
		public Dictionary<long, Dictionary<string, MKSS.Model.SensorData>> BatchSensorDataDictionary { get; set; }
		/// <summary>
		///   连接池
		/// </summary>
		Dictionary<long, ModBusBoardConnection> ConnectionPool = new Dictionary<long, ModBusBoardConnection>();

		List<TaskInfoBase> Tasks { get; set; }


		/// <summary>
		///  添加任务
		/// </summary>
		/// <param name="task"></param>
		public void PushTask(List<TaskInfoBase> task)
        {
			Tasks.AddRange(task);
			if (RefreshTaskFree && StartLahuaTaskFree) {
				RefreshTask();
				StartLahuaTask();
			}
		}


		public LaohuaService() {

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
		 

        ~LaohuaService()
		{
			SM_DataEventSaver.Dispose();
		}


		bool RefreshTaskFree = true;
		[LogTagClass(Title = "刷新老化任务")]
		public void RefreshTask() {


			RefreshTaskFree = false;
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
				ULogger.Log(string.Format("初始老化柜 {0} 个......", _BoardCaseDic.Count));


				//统计变动的批次
				List<Batch> _BatchAll = _BatchServices.QuerySql(string.Format("select * from pd_batch where F_STA_AgingStatus is null or F_AgingStatus!=F_STA_AgingStatus")).Result;
				foreach (var item in _BatchAll)
				{
					if (item.EnumAgingStatus == EnumAgingStatus.InAging) continue;//老化中不在这里处理
					List<Sensor> _SensorAll = _SensorServices.QuerySql(string.Format("select * from pd_sensor where F_BatchId =" + item.F_BatchId)).Result;
					_LaoHuaGuanChaDataTimer.TimerStart(item, _SensorAll);
				}
				
				List<Batch> _Batchs = _BatchServices.QuerySql(string.Format("select * from pd_batch where F_AgingStatus='{0}' ", (int)EnumAgingStatus.InAging )).Result;
				foreach (var item in _Batchs)
				{
					if (item.F_STA_TIME != DateTime.MinValue && DateTime.Now - item.F_STA_TIME < new TimeSpan(0, 20, 0)) continue;// 20 分钟统计一次
					List<Sensor> _SensorAll = _SensorServices.QuerySql(string.Format("select * from pd_sensor where F_BatchId =" + item.F_BatchId)).Result;
					_LaoHuaGuanChaDataTimer.TimerStart(item, _SensorAll);
				}
				//ULogger.Log(string.Format("刷新老化任务，找到 {0} 个批次需要老化......", _Batchs.Count));


				Dictionary<long, List<TaskInfo>> BoardListTemp = new Dictionary<long, List<TaskInfo>>();
				foreach (long conn in ConnectionPool.Keys)
				{
					BoardListTemp.Add(conn, new List<TaskInfo>());
				}

				List<BoardCase> _BoardCaseList = new List<BoardCase>();
				List<Board> add_task = new List<Board>();
				foreach (Batch _Batch in _Batchs)
				{
					 

					ULogger.Log(string.Format("批次需要老化/标定：{0}......", _Batch.F_BatchName));

					try
					{

						List<BatchRel> _BatchRels = _BatchRelServices.QuerySql(string.Format("select * from pd_batch_rel where F_BatchId='{0}'", _Batch.F_BatchId)).Result;
						List<Sensor> _Sensors = _SensorServices.QuerySql(string.Format("select * from pd_sensor where F_BatchId='{0}'", _Batch.F_BatchId)).Result;

						string sql1 = "SELECT t.F_DataValue,t.F_SensorId,t.F_AddTime FROM (SELECT F_SensorId,max(F_AddTime) as F_AddTime FROM `pd_sensordata_" + _Batch.F_BatchId + "` where F_DataValue>170 GROUP BY F_SensorId) a LEFT JOIN `pd_sensordata_" + _Batch.F_BatchId + "` t ON t.F_SensorId=a.F_SensorId and t.F_AddTime=a.F_AddTime";
						DataTable tab1 = _SensorServices.QueryTable(sql1).Result;
						Dictionary<string, DataRow> dic1 = new Dictionary<string, DataRow>();
						for (int i = 0; i < tab1.Rows.Count; i++)
						{
							DataRow dr = tab1.Rows[i];
							string F_SensorId = dr["F_SensorId"] + "";
							if (!dic1.ContainsKey(F_SensorId)) dic1.Add(F_SensorId, dr);
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
								t_data.F_AddTime = Convert.ToDateTime(dic1[item.F_SensorId]["F_AddTime"]);
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
										//if (_BoardCase.EnumUseInFree != EnumUseInFree.InUse) continue;
										_BoardCaseList.Add(_BoardCase);
									}
								}
							}

							//挑出有效的板子
							if (_BoardDic.ContainsKey(r.F_BoardId))
							{

								Board _Board = _BoardDic[r.F_BoardId];
								//已经结束的板子直接跳过
								//if (_Board.EnumUseInFree != EnumUseInFree.InUse  ) continue;

								List<byte> addr = _Board.AddrList;
								List<long> databus = _Board.DataBusList;
								if (addr.Count != databus.Count)
								{
									ULogger.Error(string.Format("地址，链路不匹配：{0},{1}", addr.Count, databus.Count));
									continue;
								}

								//将批次信息保存到连接中
								for (int i = 0; i < databus.Count; i++)
								{
									if (ConnectionPool.ContainsKey(databus[i]))
									{
										if (BoardListTemp[databus[i]].Count(w => w.BoardId == _Board.F_BoardId) > 0)
										{
											//已经添加过，更新
											BoardListTemp[databus[i]].FirstOrDefault(w => w.BoardId == _Board.F_BoardId).Batch = _Batch;
											add_task.Add(_Board);
										}
										else
										{
											Dictionary<string, Sensor> all = BatchSensorDictionary[_Batch.F_BatchId];
											Dictionary<string, Sensor> allallSensor = all.Where(w => w.Value.F_BoardId == _Board.F_BoardId).ToDictionary(w=>w.Key,w=>w.Value);
											BoardListTemp[databus[i]].Add(new TaskInfo()
											{
												Batch = _Batch,
												Board = _Board,
												BoardCase = _BoardCaseList.First(w=>w.F_BoardCaseId==_Board.F_BoarCaseId),
												SensorDictionary = allallSensor,
												BatchId = _Batch.F_BatchId,
												BoardId = _Board.F_BoardId,
												DataBusId = databus[i],
												DataBus = ConnectionPool[databus[i]].DataBus,
												TaskType = TaskInfoEnum.ReadVoltageOnce 
											});// (_Board, _Batch);
											add_task.Add(_Board);
										}

									}
								}

							}

						}

					}
					catch (Exception ex)
					{
						ULogger.Log(ex.Message);
						ULogger.Log(ex.StackTrace);
					}


				}

				foreach (long conn in ConnectionPool.Keys)
				{
					ConnectionPool[conn].SetAddr(BoardListTemp[conn]);
					FinishBatchAssert(ConnectionPool[conn]);//判断是否结束 
				}

				string strResult = _BoardCaseList.Count == 0 ? "0" : _BoardCaseList.Select(w => w.F_BoardCaseAddress + "").Aggregate((a, b) => a + "#" + b + "#");
				ULogger.Log(string.Format("老化任务刷新完成，检测到{0}柜{1}托盘新任务......", strResult, add_task.Count));



			}
			catch (Exception ex)
			{

				throw ex;
			}
			finally {
				RefreshTaskFree = true; 
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

		 
		public static int DEBUG
        {
			set {
				ULogger.Log(string.Format("Debug _____________------> {0} ", value));
			}
		}

		public enum TaskTypeEnum { QueryTask }
		int StartLahuaTaskCount = 0; 
		bool StartLahuaTaskFree { get { return StartLahuaTaskCount ==0; } }
		public DateTime ReadVoltageOnceStart = DateTime.Now;
		[LogTagClass(Title = "创建读取任务")]
		public async Task StartLahuaTask()
		{

			if (!StartLahuaTaskFree) {
				ULogger.Log(string.Format("上次任务没结束，直接跳过......"));
			}

			StartLahuaTaskCount = ConnectionPool.Count;
			try
			{

				int count = 0; ////DEBUG =1;
				ReadVoltageOnceStart = DateTime.Now;
				Dictionary<long, TaskTypeEnum> TaskTypeEnumBatch = new Dictionary<long, TaskTypeEnum>();
				Dictionary<ModBusBoardConnection, TaskTypeEnum> TaskTypeEnumDic = new Dictionary<ModBusBoardConnection, TaskTypeEnum>();
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					List<TaskAddress> all = Connection.ModBusBoard.AddressTemp;
					TaskInfo task_first = Connection.TaskList.FirstOrDefault();
					if (task_first == null) continue;

					if (TaskTypeEnumBatch.ContainsKey(task_first.Batch.F_BatchId)) {
						TaskTypeEnumDic.Add(Connection, TaskTypeEnumBatch[task_first.Batch.F_BatchId]);
						continue;
					}
					 
					TaskTypeEnumDic.Add(Connection, TaskTypeEnum.QueryTask);
					TaskTypeEnumBatch.Add(task_first.Batch.F_BatchId, TaskTypeEnumDic[Connection]);
				}
				 
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{

					List<TaskAddress> all = Connection.ModBusBoard.AddressTemp;
					TaskInfo task_first = Connection.TaskList.FirstOrDefault();
					if (task_first == null) continue;
					ULogger.Log(string.Format("添加["+ Connection + "]["+ task_first.ToString() + ":"+ TaskTypeEnumDic[Connection] + "]......", count));

					Task.Run(delegate ()
					{
						StartLahuaTaskInner(Connection, TaskTypeEnumDic[Connection], ref count);
					});
				}

				ULogger.Log(string.Format("正在读取......", count));

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取老化数据出错 {0} ......", ex.Message));
				ULogger.Log(ex.StackTrace);
			}

		}


		void StartLahuaTaskInner(ModBusBoardConnection Connection, TaskTypeEnum _TaskTypeEnum, ref int count) {

			try
			{

				if (Connection.Address.Count == 0) return;
				if (!Connection.IsOpen)
				{
					Ping pingSender = new Ping();
					PingReply reply = pingSender.Send(Connection.IP, 120);//第一个参数为ip地址，第二个参数为ping的时间
					if (reply.Status != IPStatus.Success)
					{
						ULogger.Log(string.Format("IP {0} 不通", Connection.IP));
						return;
					}
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

					////DEBUG =1001;
					//占用通道
					ds.EnumUseStatus = DataBusUseStatus.LaoHua;
					ds.F_UseStartTime = DateTime.Now;
					_DataBusServices.Update(ds);
					////DEBUG =1002;
					Connection.ModBusBoard.OpenTcp();
					////DEBUG =1003;
					if (Connection.ModBusBoard.IsOpen)
					{
						Connection.ModBusBoard.DataEventAging -= this.SM_DataEvent;
						Connection.ModBusBoard.DataEventAging += this.SM_DataEvent;
						Connection.ModBusBoard.DataEvent -= this.ModBusBoard_DataEvent;
                        Connection.ModBusBoard.DataEvent += this.ModBusBoard_DataEvent;
					}
					////DEBUG =1000;
					//ULogger.Log(string.Format("连接 {0}:{1} {2}......", Connection.IP, Connection.PORT, Connection.ModBusBoard.IsOpen ? "成功" : "失败"));
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("端口 {0}:{1} 不通，{2}", Connection.IP, Connection.PORT, ex.Message));
					ULogger.Log(ex.StackTrace);
				}

				count++;
				List<TaskAddress> all = Connection.ModBusBoard.AddressTemp;

				TaskInfo task_first =  Connection.TaskList.FirstOrDefault();
				if (task_first == null) return;
				 

                try
				{

					if (_TaskTypeEnum == TaskTypeEnum.QueryTask)
					{
						List<TaskAddress> _ReadVoltageOnce = all.Where(w => w.TaskInfo.TaskType == TaskInfoEnum.ReadVoltageOnce).ToList();
						if (_ReadVoltageOnce.Count > 0)
						{
							Connection.ModBusBoard.ReadSignalStartReadData();
							FinishBatchAssert(Connection);//判断是否结束 
							Connection.ModBusBoard.ReadVoltageOnce(_ReadVoltageOnce).Wait();
							FinishBatchAssert(Connection);//判断是否结束
						}
						return;//不执行额外任务了
					}

				}
                catch (Exception ex)
				{
					ULogger.Log(string.Format("任务："+  _TaskTypeEnum + " {0} 出错......", ex.Message));
				}


				

			}
			catch (Exception ex)
			{
				////DEBUG =1200;
				ULogger.Log(string.Format("出错 {0} ......", ex.Message));
				ULogger.Log(ex.StackTrace);
			}
			finally
			{


				//完成后释放连接
				Connection.ModBusBoard.CloseTCPIP();

				DataBus ds = Connection.DataBus;
				//释放通道
				ds.EnumUseStatus = DataBusUseStatus.None;
				ds.F_UseStartTime = DateTime.Now;
				_DataBusServices.Update(ds);
				StartLahuaTaskCount--;
			}


		}
		 


		/// <summary>
		///  检查已是否完成或删除任务，终止
		/// </summary>
		/// <param name="DataSrc"></param>
		public void FinishBatchAssert(ModBusBoardConnection Connection)
		{

			foreach (TaskInfo _task in Connection.TaskList)
			{
				try
				{
					Board _Board = _task.Board;
					Batch _Batch = _task.Batch;
					//标定过程不需要结束断言
					BoardCase _BoardCase = this._BoardCaseDic.Values.FirstOrDefault(w => w.F_BoardCaseId == _Board.F_BoarCaseId);
					FinishBatchAssertInner(_Batch, _Board, _BoardCase);
				}
				catch (Exception ex)
				{
					ULogger.Error(string.Format("结束老化出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}


		}


		public void FinishBatchAssertInner(Batch _Batch, Board _Board, BoardCase _BoardCase)
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
							_Board.EnumUseType = EnumUseType.LaoHua;
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
					ULogger.Error(string.Format("结束老化出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}


		}


		public TimeSpan SM_DataEventSaveDbIntrval { get; set; } = new TimeSpan(0,1,0);
		
		[LogTagClass(Title = "接收老化数据")]
		private void SM_DataEvent(object sender, CommFactory.DataEventAgingArgs e)
		{

			if (e.ReadMode == ReadMode.WX_StartRead) {

				string DEBUG = "A";
				foreach (TaskInfo _task in e.ProductData.Connection.TaskList)
				{
					Board _Board = _task.Board;
					if (_Board.F_BoardId == e.Address.TaskInfo.BoardId)
					{

						Batch _Batch = _task.Batch;
						BoardCase _BoardCase = this._BoardCaseDic[_Board.F_BoarCaseId];

						List<MKSS.Model.Sensor> sens_insert = new List<MKSS.Model.Sensor>();
						List<MKSS.Model.SensorData> datas_realtime_insert = new List<MKSS.Model.SensorData>();
						List<MKSS.Model.SensorData> datas_history = new List<MKSS.Model.SensorData>();
						if (!this.BatchSensorDictionary.ContainsKey(_Batch.F_BatchId)) this.BatchSensorDictionary.Add(_Batch.F_BatchId, new Dictionary<string, Sensor>());
						if (!this.BatchSensorDataDictionary.ContainsKey(_Batch.F_BatchId)) this.BatchSensorDataDictionary.Add(_Batch.F_BatchId, new Dictionary<string, MKSS.Model.SensorData>());
						Dictionary<string, Sensor> sens = this.BatchSensorDictionary[_Batch.F_BatchId];
						Dictionary<string, MKSS.Model.SensorData> sensData = this.BatchSensorDataDictionary[_Batch.F_BatchId];


						int start = 15 * _Board.AddrList.IndexOf((byte)e.Address.Address);
						for (int i = 0; i < e.ProductData.SingleAddressData.Length; i++)
						{

							int F_SlotNO = start + i + 1;
							DataAgingSensor item = e.ProductData.SingleAddressData[i];

							//插入数据库
							string F_SensorId = Sensor.CreateCensorId(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _Board.F_FloorNO, F_SlotNO);
							MKSS.Model.SensorData data = new MKSS.Model.SensorData()
							{
								F_AddTime = e.ProductData.Time,
								F_DataValue = item.Value.Value,
								F_DataValueValid = item.ValueDbl.Value,
								F_SensorId = F_SensorId,
								F_DataId = Guid.NewGuid().ToString()
							};
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
								sensData[F_SensorId].Cache.Insert(0,data);
								datas_history.Add(data);//入库
								DEBUG = DEBUG + "N";
							}
							else
							{


								sensData[F_SensorId].Cache.Insert(0, data);
								//跳变 170 的处理
								if (data.F_DataValueEmp170)
								{
									sensData[F_SensorId].F_DataValueEmpCount++;
									if (!sensData[F_SensorId].F_DataValueEmp && sensData[F_SensorId].F_DataValueEmpCount <= 2)
									{
										//历史数据不空，空值连续 2 次以内，不认可空值 
										DEBUG = DEBUG + "_";
									}
									else
									{
										sensData[F_SensorId].F_DataValue = data.F_DataValue;
										sensData[F_SensorId].F_AddTime = data.F_AddTime;
										datas_history.Add(data);//入库
										DEBUG = DEBUG + "-";
									}
								}
								else
								{
									sensData[F_SensorId].F_DataValue = data.F_DataValue;
									sensData[F_SensorId].F_AddTime = data.F_AddTime;
									sensData[F_SensorId].F_DataValueEmpCount = 0;
									datas_history.Add(data);//入库
									DEBUG = DEBUG + "*";
								}



							}

						}


						if (sens_insert.Count > 0)
						{
							this._SensorServices.Add(sens_insert).Wait();
							foreach (var item in sens_insert)
							{
								sens.Add(item.F_SensorId, item);
							}
						}

						if (datas_history.Count > 0)
						{
							if (OnNewData != null) OnNewData(_Batch, datas_history);
						}


						DEBUG = DEBUG + "G";
						int insert_act = 0;
						if (datas_history.Count > 0)
						{


							DEBUG = DEBUG + "H";
							StringBuilder sb = new StringBuilder();

							var conn = e.ProductData.Connection;
							DateTime last = conn.GetSM_DataEventSaveDbLast(_Board, (byte)e.Address.Address);

							//滤波缓存
							foreach (var item in datas_history)
							{
								string F_SensorId = item.F_SensorId;
								//滤波缓存最多 10 条
								if (sensData[F_SensorId].Cache.Count > 10) sensData[F_SensorId].Cache.RemoveAt(sensData[F_SensorId].Cache.Count - 1);

								//滤波：1-2 * 4， 如果第3位出现剧变，并且校验值不剧变，采用第3位校验值
								if (sensData[F_SensorId].Cache.Count > 3)
								{
									var c = sensData[F_SensorId].Cache.Count;
									var d3 = sensData[F_SensorId].Cache[2];
									var d2 = sensData[F_SensorId].Cache[1];
									var d1 = sensData[F_SensorId].Cache[0];
									decimal[] v_normal = new decimal[] { d3.F_DataValue, d2.F_DataValue, d1.F_DataValue  };
									decimal[] v_valid = new decimal[] { d3.F_DataValue, d2.F_DataValueValid, d1.F_DataValue };
									//测量值剧变，校验值很小波动，用校验值替代入库数据
									if (Math.Abs(v_normal.Max() - v_normal.Min()) > 35
										&& Math.Abs(v_valid.Max() - v_valid.Min()) < 20
										)
									{
										sb.Append(
										string.Format(
											"update pd_sensordata_{0} set F_DataValue={1} where F_DataId='{2}';",
											_Batch.F_BatchId, d2.F_DataValueValid, d2.F_DataId));
										ULogger.Log(string.Format("################################################################"));
										ULogger.Log(string.Format("滤波 {0},{1} {2} , ±{3}，修复 {4}=====》{5}......",
											e.ProductData.Connection,
											_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address, 
											d2.F_AddTime,
											Math.Abs(v_normal.Max() - v_normal.Min()),
											d2.F_DataValue,
											d2.F_DataValueValid)
										);
										ULogger.Log(string.Format("################################################################"));
									}
								}
							}

							foreach (var item in datas_history)
							{
								if (item.F_DataValue == 0 || item.F_DataValue == 170) continue;//170 无用数据不再存储
								insert_act++;
								sb.Append(
									string.Format(
										"insert into pd_sensordata_{0} (F_DataId, F_SensorId, F_DataValue,F_AddTime)VALUES ('{1}', '{2}', {3},'{4}');",
										_Batch.F_BatchId, item.F_DataId, item.F_SensorId, item.F_DataValue, item.F_AddTime.ToString("yyyy-MM-dd HH:mm.ss")
								));
							}

							sb.Append(
									string.Format(
										"update pd_batch set F_AgingLastUpdateTime='{1}' where F_BatchId={0};",
										_Batch.F_BatchId, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")
								));



							DEBUG = DEBUG + "I";
							if (sb.Length > 0 &&
								(
									last == DateTime.MinValue
									||
									DateTime.Now - last > this.SM_DataEventSaveDbIntrval
								))
							{
								DEBUG = DEBUG + "S";
								conn.SetSM_DataEventSaveDbLast(_Board, (byte)e.Address.Address, DateTime.Now);
								//高频刷新低频存储
								SM_DataEventSaver.EnqueueTask(sb.ToString());
								//_SensorServices.QueryTable(sb.ToString()).Start();
								ULogger.Log(string.Format("存储数据 {0},{1} {4}=> {2}-{3} ,{5} 秒[" + DEBUG + "]......",
									e.ProductData.Connection,
									_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
									e.ProductData.SingleAddressData.Min(w => w.Value.Value),
									e.ProductData.SingleAddressData.Max(w => w.Value.Value),
									string.Format("插入 pd_sensordata_{0} {2}/{1} 条", _Batch.F_BatchId, datas_history.Count, insert_act),
									(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
									);

							}
							else
							{
								if (last != DateTime.MinValue)
								{
									DEBUG = DEBUG + "[" + (this.SM_DataEventSaveDbIntrval - (DateTime.Now - last)).TotalSeconds.ToString("f0") + "]";
								}
								DEBUG = DEBUG + "R";
								ULogger.Log(string.Format("刷新数据 {0},{1} {2}-{3} ,{4} 秒 {5}<---{6}，正常[" + DEBUG + "]......",
										e.ProductData.Connection,
										_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
										e.ProductData.SingleAddressData.Min(w => w.Value.Value),
										e.ProductData.SingleAddressData.Max(w => w.Value.Value),
										(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"), sb.Length, last)

										);
							}



						}
						else
						{


							DEBUG = DEBUG + "Z";
							ULogger.Log(string.Format("刷新数据 {0},{1} {2}-{3} ,{4} 秒，跳过[" + DEBUG + "]......",
									e.ProductData.Connection,
									_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
									e.ProductData.SingleAddressData.Min(w => w.Value.Value),
									e.ProductData.SingleAddressData.Max(w => w.Value.Value),
									(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
									);
						}


					}
				}

				//DEBUG =6102;

			}

		}


		[LogTagClass(Title = "接收标定数据")]
		private void ModBusBoard_DataEvent(object sender, CommFactory.DataEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			string DEBUG = "A";
			TaskInfo _task = e.ProductData.Address.TaskInfo;
			Board _Board = _task.Board;
			Batch _Batch = _task.Batch;
			BoardCase _BoardCase = this._BoardCaseDic[_Board.F_BoarCaseId];
			Dictionary<string, Sensor> sens = this.BatchSensorDictionary[_Batch.F_BatchId];
			if (e.ReadMode == ReadMode.RX_ReadDeviceDateTime) {

				int start = 15 * _Board.AddrList.IndexOf((byte)e.Address.Address);
				for (int i = 0; i < e.ProductData.SingleAddressData.Length; i++)
				{
					int F_SlotNO = start + i + 1;
					MKSS.Service.LaoHua.Util.SensorData item = e.ProductData.SingleAddressData[i];
					if (string.IsNullOrEmpty(item.DeviceDateTime)|| item.DeviceDateTime=="0000-00-00 00:00" || item.DeviceDateTime == "0-00-00 00:00") continue;
					string F_SensorId = Sensor.CreateCensorId(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _Board.F_FloorNO, F_SlotNO);
					if (sens.ContainsKey(F_SensorId)) {
						Sensor s = sens[F_SensorId];
						sb.Append(
							string.Format(
								"update pd_sensor set F_DeviceHistoryClock='{1}',F_DeviceHistoryClockDetectTime='{2}' where F_SensorId={0};",
								F_SensorId, item.DeviceDateTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")
						));
					}
				}


				if (sb.Length > 0)
				{
					DEBUG = DEBUG + "S";
					//高频刷新低频存储
					SM_DataEventSaver.EnqueueTask(sb.ToString());
					//_SensorServices.QueryTable(sb.ToString()).Start();
					ULogger.Log(string.Format("存储时钟 {0},{1} {2}-{3} ,{4} 秒，[" + DEBUG + "]......",
							e.ProductData.Connection,
							_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
							e.ProductData.SingleAddressData.Min(w => w.DeviceDateTime),
							e.ProductData.SingleAddressData.Max(w => w.DeviceDateTime),
							(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
							);

				}
				else
				{
					DEBUG = DEBUG + "R";
					ULogger.Log(string.Format("刷新时钟 {0},{1} {2}-{3} ,{4} 秒，[" + DEBUG + "]......",
							e.ProductData.Connection,
							_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
							e.ProductData.SingleAddressData.Min(w => w.DeviceDateTime),
							e.ProductData.SingleAddressData.Max(w => w.DeviceDateTime),
							(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
							);

				}

			}
			if (e.ReadMode == ReadMode.RX_ReadSerialNo)
			{
				
				ULogger.Log(string.Format("刷新串号 {0},{1} {2}-{3} ,{4} 秒，[" + DEBUG + "]......",
							e.ProductData.Connection,
							_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
							e.ProductData.SingleAddressData.Where(w => w.Serial!=null && (w.Serial.Length == 12 || w.Serial.Length == 14)).Min(w => w.Serial),
							e.ProductData.SingleAddressData.Where(w => w.Serial != null && (w.Serial.Length == 12 || w.Serial.Length == 14)).Max(w => w.Serial),
							(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
							);

				int start = 15 * _Board.AddrList.IndexOf((byte)e.Address.Address);
				for (int i = 0; i < e.ProductData.SingleAddressData.Length; i++)
				{
					int F_SlotNO = start + i + 1;
					MKSS.Service.LaoHua.Util.SensorData item = e.ProductData.SingleAddressData[i];
					if (string.IsNullOrEmpty(item.Serial)) continue;
					string F_SensorId = Sensor.CreateCensorId(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _Board.F_FloorNO, F_SlotNO);
					if (sens.ContainsKey(F_SensorId))
					{
						Sensor s = sens[F_SensorId];
						sb.Append(
							string.Format(
								"update pd_sensor set F_SerialNO='{1}',F_SerialNoDetectTime='{2}' where F_SensorId={0};",
								F_SensorId, item.Serial, DateTime.Now.ToString("yyyy-MM-dd HH:mm.ss")
						));
					}
				}


				if (sb.Length > 0)
				{
					DEBUG = DEBUG + "S";
					//高频刷新低频存储
					SM_DataEventSaver.EnqueueTask(sb.ToString());
					//_SensorServices.QueryTable(sb.ToString()).Start();
					ULogger.Log(string.Format("存储串号 {0},{1} {2}-{3} ,{4} 秒，[" + DEBUG + "]......",
							e.ProductData.Connection,
							_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
							e.ProductData.SingleAddressData.Min(w => w.DeviceDateTime),
							e.ProductData.SingleAddressData.Max(w => w.DeviceDateTime),
							(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
							);

				}
				else
				{
					DEBUG = DEBUG + "R";
					ULogger.Log(string.Format("刷新串号 {0},{1} {2}-{3} ,{4} 秒，[" + DEBUG + "]......",
							e.ProductData.Connection,
							_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO + "@" + (byte)e.Address.Address,
							e.ProductData.SingleAddressData.Min(w => w.DeviceDateTime),
							e.ProductData.SingleAddressData.Max(w => w.DeviceDateTime),
							(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
							);

				}

			}


		}


	}


	/// <summary>
	///  委托数据
	/// </summary>
	/// <param name="_Batch">批次</param>
	/// <param name="datas_history">数据历史</param>
	public delegate void NewDataEventHandler(Batch _Batch, List<MKSS.Model.SensorData> datas_history);


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

		public static void Start()
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

				try
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

					if (works.Count > 0)
						SaveData(works);  // 任务不为null时，处理并保存数据
					else
						_wh.WaitOne();   // 没有任务了，等待信号
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("出错 {0} ......", ex.Message));
					ULogger.Log(ex.StackTrace);
					_wh.WaitOne();   // 没有任务了，等待信号
				}
				
			}
		}

		/// <summary>插入任务</summary>
		public static void EnqueueTask(string task)
		{
            try
            {
				lock (_locker)
					_tasks.Enqueue(task);  // 向队列中插入任务 

				_wh.Set();  // 给工作线程发信号
			}
            catch (Exception ex)
            {
				ULogger.Log(string.Format("出错 {0} ......", ex.Message));
				ULogger.Log(ex.StackTrace);
			}
			
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
				ULogger.Log(string.Format("插入 数据出错 {0}", ex.Message));
			}
		}
	}




}
