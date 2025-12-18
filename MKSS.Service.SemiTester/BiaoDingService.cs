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

namespace MKSS.Service.LaoHua
{


	[LogTagClass(Title = "标定服务")]
	public class BiaoDingService
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

		Dictionary<long, BoardCase> _BoardCaseDic { get; set; }
		Dictionary<long, Dictionary<string, Sensor>> BatchSensorDictionary { get; set; }
		public Dictionary<long, Dictionary<string, MKSS.Model.SensorData>> BatchSensorDataDictionary { get; set; }
		/// <summary>
		///   连接池
		/// </summary>
		Dictionary<long, ModBusBoardConnection> ConnectionPool = new Dictionary<long, ModBusBoardConnection>();


		public BiaoDingService( ) {

			ConnectionPool = new Dictionary<long, ModBusBoardConnection>();
			BatchSensorDictionary = new Dictionary<long, Dictionary<string, Sensor>>();
			BatchSensorDataDictionary = new Dictionary<long, Dictionary<string, MKSS.Model.SensorData>>();


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

		[LogTagClass(Title = "刷新标定任务")]
		public void RefreshTask() {


			//刷新通信链路状态
			Dictionary<long, DataBus> _DataBusDic =
				_DataBusServices.QuerySql(string.Format("select * from pd_databus "))
				.Result.ToDictionary(w => w.F_DataBusId, w => w);
			foreach (var item in _DataBusDic.Values)
			{
				if (this.ConnectionPool.ContainsKey(item.F_DataBusId)) {
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
			ULogger.Log(string.Format("初始标定柜 {0} 个......", _BoardCaseDic.Count));

            foreach (ModBusBoardConnection conn in ConnectionPool.Values)
            {
				conn.BoardList.Clear();
			}

			//List<BoardCase> _BoardCaseList = new List<BoardCase>();
			//List<Board> add_task = new List<Board>();
			//string strResult = _BoardCaseList.Count==0?"0":_BoardCaseList.Select(w=>w.F_BoardCaseAddress+"").Aggregate((a, b) => a + "#" + b + "#");
			//ULogger.Log(string.Format("标定任务刷新完成，检测到{0}柜{1}托盘新任务......", strResult, add_task.Count));
			
		}


		[LogTagClass(Title = "创建网络连接")]
		async Task Open()
		{
			 
			try
			{

				int count = 0;
				ReadVoltageOnceStart = DateTime.Now;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					{
						Task.Run(delegate ()
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
								ULogger.Log(string.Format("通道 {0}:{1} 锁定占用中，{2} {3}", Connection.IP, Connection.PORT, ds.EnumUseStatus, ds.F_UseStartTime));
								return;
							}

							//ULogger.Log(string.Format("正在连接 {0}:{1} ......", Connection.IP, Connection.PORT));
							try
							{

								//占用通道
								ds.EnumUseStatus = DataBusUseStatus.LaoHua;
								ds.F_UseStartTime = DateTime.Now;
								_DataBusServices.Update(ds);
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

							count++;

						});

					}
				}
				ULogger.Log(string.Format("正在连接......", count));


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

				int count = 0;
				ReadVoltageOnceStart = DateTime.Now;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					Task.Run(delegate ()
					{

						if (!Connection.IsOpen) Connection.ModBusBoard.CloseTCPIP();

						DataBus ds = Connection.DataBus;
						//释放通道
						ds.EnumUseStatus = DataBusUseStatus.LaoHua;
						ds.F_UseStartTime = DateTime.Now;
						_DataBusServices.Update(ds);

					});
				} 

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("关闭连接出错 {0} ......", ex.Message));
			}

		}


		public DateTime ReadVoltageOnceStart = DateTime.Now;
		[LogTagClass(Title = "创建读取任务")]
		public async Task ReadVoltageOnce()
		{

			try
			{

				int count = 0;
				ReadVoltageOnceStart = DateTime.Now;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					Task.Run(delegate ()
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
							ULogger.Log(string.Format("通道 {0}:{1} 锁定占用中，{2} {3}", Connection.IP, Connection.PORT, ds.EnumUseStatus, ds.F_UseStartTime));
							return;
						}

						//ULogger.Log(string.Format("正在连接 {0}:{1} ......", Connection.IP, Connection.PORT));
						try
						{

							//占用通道
							ds.EnumUseStatus = DataBusUseStatus.LaoHua;
							ds.F_UseStartTime = DateTime.Now;
							_DataBusServices.Update(ds);
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

						count++;
						FinishBatchAssert(Connection);//判断是否结束
													  //ULogger.Log(string.Format("读取任务 {0} 个xxxxx......", count));
						Connection.ModBusBoard.ReadVoltageOnce().Wait();
						FinishBatchAssert(Connection);//判断是否结束

						if (!Connection.IsOpen) Connection.ModBusBoard.CloseTCPIP();

						//释放通道
						ds.EnumUseStatus = DataBusUseStatus.LaoHua;
						ds.F_UseStartTime = DateTime.Now;
						_DataBusServices.Update(ds);

					});
				}
				ULogger.Log(string.Format("正在读取......", count));
				 
			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取标定数据出错 {0} ......", ex.Message));
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
					ULogger.Error(string.Format("结束标定出错：{0}", ex.Message));
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
					ULogger.Error(string.Format("结束标定出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}


		}


		[LogTagClass(Title = "接收标定数据")]
		private void SM_DataEvent(object sender, ModBusBoard.DataEventAgingArgs e)
		{ 
			foreach (Board _Board in e.ProductData.Connection.BoardList.Keys.ToList())
            {
				if (_Board.AddrList.Contains((byte)e.Address))
				{

					Batch _Batch = e.ProductData.Connection.BoardList[_Board];
					BoardCase _BoardCase = this._BoardCaseDic[_Board.F_BoarCaseId];

					List<MKSS.Model.Sensor> sens_insert = new List<MKSS.Model.Sensor>();
					List<MKSS.Model.SensorData> datas_realtime_insert = new List<MKSS.Model.SensorData>();
					List<MKSS.Model.SensorData> datas_history = new List<MKSS.Model.SensorData>();
					if (!this.BatchSensorDictionary.ContainsKey(_Batch.F_BatchId)) this.BatchSensorDictionary.Add(_Batch.F_BatchId, new Dictionary<string, Sensor>());
					if (!this.BatchSensorDataDictionary.ContainsKey(_Batch.F_BatchId)) this.BatchSensorDataDictionary.Add(_Batch.F_BatchId, new Dictionary<string, MKSS.Model.SensorData>());
					Dictionary<string, Sensor> sens = this.BatchSensorDictionary[_Batch.F_BatchId];
					Dictionary<string, MKSS.Model.SensorData> sensData = this.BatchSensorDataDictionary[_Batch.F_BatchId];


					int start = 15 * _Board.AddrList.IndexOf((byte)e.Address);
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
							F_SensorId = F_SensorId,
							F_DataId = Guid.NewGuid().ToString()
						};
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


					if (sens_insert.Count > 0) {
						this._SensorServices.Add(sens_insert).Wait();
                        foreach (var item in sens_insert)
                        {
							sens.Add(item.F_SensorId, item);
                        }
					}

					int insert_act = 0;
					if (datas_history.Count > 0)
					{
						StringBuilder sb = new StringBuilder();
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

						if (sb.Length > 0) {
							var rr = _SensorServices.QueryTable(sb.ToString()).Result;
						} 

					}

					ULogger.Log(string.Format("收到数据 {0},{1} {4}=> {2}-{3} ,{5}秒......",
						e.ProductData.Connection, 
						_BoardCase.F_BoardCaseAddress + "#" + _Board.F_FloorNO,
						e.ProductData.SingleAddressData.Min(w => w.Value.Value), 
						e.ProductData.SingleAddressData.Max(w => w.Value.Value),
						string.Format("插入 pd_sensordata_{0} {2}/{1} 条", _Batch.F_BatchId, datas_history.Count, insert_act),
						(DateTime.Now - ReadVoltageOnceStart).TotalSeconds.ToString("f2"))
						);


				}
			}

		} 

	}

}
