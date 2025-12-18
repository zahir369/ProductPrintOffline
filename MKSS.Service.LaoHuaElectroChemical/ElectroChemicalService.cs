using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.LaoHuaElectroChemical;
using MKSS.Services;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using System.Diagnostics; 
using System.Data;
using System.Threading;
using System.IO.Ports;
using NModbus;
using NModbus.Serial;
using MKSS.Service.LaoHua;
using System.Net.Sockets;

namespace MKSS.Service.LaoHuaElectroChemical
{

    [LogTagClass(Title = "检测任务池")]
	public class ElectroChemicalService
	{

		/// <summary>
		///  调试模式，随机数
		/// </summary>
		public static bool Debug { get; set; } = false;
		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 1000 ;//200毫秒钟读一次数据
		public static string[] Regions = new string[] { "A", "B", "C", "D" };
		public static int RegionSize = 16;
		public static int FloorSize = 15;
		public static event NewDataEventHandler OnNewData;
		public static event FinishEventHandler OnFinish;
		public static event CircleOnceEventHandler CircleOnce;
		public static event StatusChangedEventHandler StatusChanged;
		public static event ErrorReadHandler OnError;
		public static event SucessReadHandler OnSucess;
		

		DateTime StartTime { get; set; }
		
		BatchServices _BatchServices = new BatchServices();

		public MKSS.Model.SensorGroupData BatchSensorDataLast { get; set; }
		public SensorGroupData BatchSensorDataLastDictionary { get; set; }
		  

		/// <summary>
		///  获取最新数据
		/// </summary>
		/// <param name="boardNo"></param>
		/// <param name="slotNo"></param>
		/// <returns></returns>
		public SensorDataItem this[PosEnum posenum]
		{
			get
			{
				if (CurrentBatch == null) return null;
				return BatchSensorDataLastDictionary.F_DataValue(posenum);
			}
		}

		/// <summary>
		///  设置层是否可见，可采集
		/// </summary>
		/// <param name="floor"></param>
		/// <param name="vivlble"></param>
		public void SetVisible(int floor,bool vivlble)
		{
			this.Connection.FloorsVisible[floor] = vivlble;
		}

		public bool IsOpen
		{
			get {
				if (Debug) return true;
				return this.Connection.IsOpen; 
			}
		}
		public Batch CurrentBatch { get; set; }
		ModbusFactory modbusFactory;
		public IModbusMaster Master;
		public BoardConnection Connection { get; private set; }
		public bool IsRead { get { return false; } }
		public ElectroChemicalService(BoardConnection conn,int speed) {
			Connection = conn;
		}

		~ElectroChemicalService() {
			 Operate = LaoHuaTaskOperate.Stop;
			ElectroChemicalSaver.Dispose();
		}

		public double Min { get; set; } = double.MaxValue;
		public double Max { get; set; } = double.MinValue;
		public DateTime StartBase { get; set; } = DateTime.Now; 
		public TimeSpan SpanTotal { get { return DateTime.Now - StartBase; } }
		LaHuaTaskStatus _Status = LaHuaTaskStatus.Stoped;
		public int FullQueryTimes { get; set; }
		public LaHuaTaskStatus Status {
			get { return _Status; }
			set {
				_Status = value;
				if (StatusChanged != null) StatusChanged(CurrentBatch,value);
			} 
		} 
		public LaoHuaTaskOperate Operate { get; private set; } = LaoHuaTaskOperate.Stop;
		bool RefreshTaskDoing = false;

		/// <summary>
		///  暂停
		/// </summary>
		public void PauseTask()
		{
			Operate = LaoHuaTaskOperate.Pause;
		}

		/// <summary>
		///  继续
		/// </summary>
		public void ResumeTask()
		{
			Operate = LaoHuaTaskOperate.Resume;
			ULogger.Info("用户继续：" + this.SpanTotal.TotalSeconds.ToString("f2"));
		}

		public string Message
        {
			set {
				ULogger.Info(value);
			}
		}
		public string MessageLog
		{
			set
			{
				ULogger.Info(value);
			}
		}
		
		/// <summary>
		///  暂停
		/// </summary>
		public void StopTask()
		{
			Operate = LaoHuaTaskOperate.Stop;
		}


		bool RefreshTaskFree = true;
		[LogTagClass(Title = "刷新老化任务")]
		public void RefreshTask()
		{


			RefreshTaskFree = false;
			try
			{

				List<Batch> _Batchs = _BatchServices.QuerySql(string.Format("select * from pd_batch where F_AgingStatus='{0}' ", (int)EnumAgingStatus.InAging)).Result;
				foreach (Batch _Batch in _Batchs)
				{
					ULogger.Log(string.Format("批次需要老化：{0}......", _Batch.F_BatchName));
					StartTask(_Batch).Wait();
				} 

			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				RefreshTaskFree = true;
			}

		}


		public TimeSpan StartTaskInterval { get; set; } = TimeSpan.MinValue;
		[LogTagClass(Title = "开启任务")]
		public Task StartTask(Batch _bat) {

			var newTask = new Task(() =>
			{
				try
				{
					 
					if (RefreshTaskDoing) return;
					RefreshTaskDoing = true;
					Status = LaHuaTaskStatus.Starting;
					Min = int.MaxValue;
					Max = int.MinValue;
					CurrentBatch = _bat;
					BatchSensorDataLastDictionary = new SensorGroupData();
					ElectroChemicalSaver.Batch = _bat;
					ElectroChemicalSaver.Start();
					Operate = LaoHuaTaskOperate.Start;
					if (modbusFactory==null) modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));

					if (!Debug)
					{
						this.Connection.Open();
						if (!this.Connection.IsOpen) return;
					}

					try
					{
						 
						StartBase = DateTime.Now;
						try
						{

							while (true) {


								if (!Debug) if (!this.IsOpen || Master == null) return;

								if (Operate== LaoHuaTaskOperate.Stop)
								{
									ULogger.Info("用户停止，结束！");
									CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
									CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
									CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
									var ress = _BatchServices.Update(CurrentBatch).Result;
									Status = LaHuaTaskStatus.Stoped;
									return;
								}

								if (Operate == LaoHuaTaskOperate.Pause)
								{
									Status = LaHuaTaskStatus.Paused;
									//ULogger.Info("用户暂停！");
									Thread.Sleep(20);
									continue;
								}
								Status = LaHuaTaskStatus.Running;

								//if (SpanTotal.TotalSeconds > CurrentBatch.F_AgingEndTime)
								//{
								//	ULogger.Info("任务到期，结束！");
								//	CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
								//	CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
								//	CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
								//	var ress = _BatchServices.Update(CurrentBatch).Result;

								//	Status = LaHuaTaskStatus.Stoped;
								//	if (OnFinish != null) OnFinish(_bat, SpanTotal);
								//	return;
								//}

								this.CurrentBatch.FullQueryTimes++;
								this.CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
								this._BatchServices.Update(CurrentBatch);

								ReadVoltageRecycleInner(SpanTotal);

								int sleep = 200;
								sleep = ((int)StartTaskInterval.TotalMilliseconds);

								int s_temp = 0;
								while (s_temp < (int)sleep)//避免长时间睡眠
								{
									if (Operate == LaoHuaTaskOperate.Stop|| Operate == LaoHuaTaskOperate.Pause)
									{
										break;//暂停，或退出，直接跳出
									}
									//等待下次数据采集
									s_temp += 50;
									Thread.Sleep(50);
								}

							}

							Status = LaHuaTaskStatus.Stoped;

						}
						catch (Exception exx)
						{
							string msg = "出错结束！错误：" + exx.Message + "";
							ULogger.Info(msg);
							ULogger.Info(exx.StackTrace);
							if (OnError != null) OnError(CurrentBatch,0,null, msg);
							return;
						}


					}
					catch (Exception ex)
					{
						string msg = ex.Message;
						ULogger.Info(msg);
						ULogger.Info(ex.StackTrace);
						if (OnError != null) OnError(CurrentBatch, 0, null, msg);
						return;
					}

				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
					ULogger.Log(ex.Message);
				}
				finally
				{
					RefreshTaskDoing = false;
					Status = LaHuaTaskStatus.Stoped;
					if (OnFinish != null) OnFinish(_bat, SpanTotal);
					try
					{
						if (!Debug) {
							this.Connection.Close();
						}
					}
					catch (Exception) { }

				}
			});

			return newTask;

		}


		[LogTagClass(Title = "读取任务")]
		void ReadVoltageRecycleInner(TimeSpan spanTotal)
		{
			 
			try
			{

				DateTime beforeDT0 = System.DateTime.Now;

				for (int floor = 1; floor <= FloorSize; floor++)
				{
					if (!this.Connection.FloorsVisible[floor]) continue;
					string[] region = Regions;
					TimeSpan F_AddTime = spanTotal;
					SensorGroupData datag = new SensorGroupData()
					{
						F_AddTime = F_AddTime.TotalSeconds,
						F_BatchId = this.CurrentBatch.F_BatchId,
						F_FloorNo = floor,
						F_DataId = string.Format("{0}{1}_{2}", this.CurrentBatch.F_BatchId, floor.ToString("00"), F_AddTime.TotalSeconds.ToString("f2"))
					};
					bool[] read_sucess = new bool[]{ false, false, false, false };
					for (int r = 0; r < region.Length; r++)
					{
						if (!Debug &&(!this.IsOpen || Master == null)) return;
						if (!this.Connection.FloorsVisible[floor]) continue;
						string boardNo = region[r];
						int address = AddrOf(floor, r + 1);
						ushort[] ret = null;
						try
						{
							if (Operate == LaoHuaTaskOperate.Stop || Operate == LaoHuaTaskOperate.Pause) return;

							if (!Debug)
							{
								ret = Master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)RegionSize);
							}
							else
							{
								ret = GetRandNums((int)(3723 * 0.1 / 5.0), (int)(3723 * 5.0 / 5.0), RegionSize).ToArray();
							}
							read_sucess[r] = true;
							if (OnSucess != null) OnSucess(CurrentBatch, floor, region[r]);
						}
						catch (Exception ex)
						{
							string msg = string.Format("读取：{0} 出错：{1}", address, ex.Message);
							ULogger.Info(msg);
							if (OnError != null) OnError(CurrentBatch, floor, region[r], msg);
							continue;
						}
						finally {
							Thread.Sleep(100);
						}
						for (int i = 0; i < 16; i++)
						{
							if (!this.Connection.FloorsVisible[floor]) continue;
							if (Operate == LaoHuaTaskOperate.Stop || Operate == LaoHuaTaskOperate.Pause) return;
							//ushort v = ret[i];
							string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, floor, boardNo, i + 1);
							double v = (double)ret[i];//
														//ushort a = ret[i];
														//double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a)) / 100;
														//的测量值的每一数据为双字节，高字节在前低字节在后。测量值的计算：输出值 DataN/10000*实际 量程 即为实际测量值。
							datag.SetDataValue(SensorGroupData.PosEnum(boardNo + (i + 1)), v);
							if ((double)v > this.Max) this.Max = (double)v;
							if ((double)v < this.Min) this.Max = (double)v;
						}
						ULogger.Info(".", null,false);
					}
					if ( Operate== LaoHuaTaskOperate.Stop || Operate == LaoHuaTaskOperate.Pause) return;



					if (!this.Connection.FloorsVisible[floor]) continue;
					try
					{
						int insert_act = 0;
						if (read_sucess.Contains(true))
						{

							ULogger.Info("K", null, false);
							if (OnNewData != null && !(Operate == LaoHuaTaskOperate.Stop)) OnNewData(CurrentBatch, datag, F_AddTime);

							if (Operate == LaoHuaTaskOperate.Stop || Operate == LaoHuaTaskOperate.Pause) return;
							StringBuilder sb_f = new StringBuilder();
							StringBuilder sb_v = new StringBuilder();
							foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
							{
								double val = datag.F_DataValue(item).F_DataValueSrc;
								sb_f.Append("," + item);
								sb_v.Append("," + val);
							}

							StringBuilder sb = new StringBuilder();
							sb.Append(
									string.Format(
										"insert into pd_sensorgroupdata_{0} (F_DataId, F_BatchId, F_AddTime,F_BoardCaseNo,F_FloorNo {3})VALUES ('{1}', '{0}',{2},1,{5} {4});",
										CurrentBatch.F_BatchId, datag.F_DataId, datag.F_AddTime, sb_f, sb_v, floor
								));

							if (Operate == LaoHuaTaskOperate.Stop || Operate == LaoHuaTaskOperate.Pause) return;

							if (sb.Length > 0)
							{
								ElectroChemicalSaver.EnqueueTask(sb.ToString());
							}
							if (OnSucess != null) OnSucess(CurrentBatch, floor, null);
						}
						else {
 
						}
					}
					catch (Exception ex)
					{ 
						string msg = string.Format("插入存储出错 {0}", ex.Message);
						ULogger.Info(msg);
						ULogger.Info(ex.StackTrace);
						if (OnError != null) OnError(CurrentBatch, floor, null, msg);
					}
				}
				ULogger.Info(System.Environment.NewLine, null, false);
				if (CircleOnce != null && !(Operate == LaoHuaTaskOperate.Stop)) CircleOnce(CurrentBatch, CurrentBatch.FullQueryTimes);




			}
			catch (Exception ex)
			{ 
				string msg = string.Format("未知错误 {0}", ex.Message);
				ULogger.Info(msg);
				ULogger.Info(ex.StackTrace);
				if (OnError != null) OnError(CurrentBatch, 0, null, msg);
			}
			 

		}


		//public bool OpenTcp()
		//{

		//	if (string.IsNullOrEmpty(IP) || PORT <= 0)
		//	{
		//		throw new Exception("error:请输入远程服务器地址");
		//	}
		//	else
		//	{

		//		TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
		//		if (_tcpClient.Connected)
		//		{
		//			ULogger.Info("重用已有连接");
		//		}
		//		else
		//		{
		//			try
		//			{
		//				PooledConnection.Pool.Open(IPAddr);
		//				_tcpClient = PooledConnection.Pool.Of(IPAddr);
		//				ULogger.Info("网口打开" + (_tcpClient.Connected ? "成功" : "失败"));
		//			}
		//			catch (Exception e)
		//			{
		//				ULogger.Info(e.Message);
		//				return false;
		//			}
		//		}


		//		var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
		//		this.Master = modbusFactory.CreateRtuMaster(adapter);
		//		this.Master.Transport.ReadTimeout = 500;
		//		this.Master.Transport.WriteTimeout = 500;
		//		this.Master.Transport.Retries = 0;
		//		this.Master.Transport.WaitToRetryMilliseconds = 500;

		//		return true;
		//	}

		//}



		//public void CloseTCPIP()
		//{
			 
			 
		//	//必须先关闭 TcpClient,Master.Dispose 会连带释放 TcpClient
		//	TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
		//	PooledConnection.Pool.Close(IPAddr);
		//	if (Master != null)
		//	{
		//		Master.Dispose();
		//		Master = null;
		//	}


		//}


		/// <summary>
		///  获取 地址
		/// </summary>
		/// <param name="floor">层编号</param>
		/// <param name="region_no">区域编号</param>
		/// <returns></returns>
		int AddrOf(int floor, int region_no) {
			return (floor - 1) * 4 + region_no -1;
		}


		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public static Batch OfBatch(long F_BatchId)
		{

			try
			{
				return ElectroChemicalSaver.SensorServices.BaseDal.Db.Queryable<Batch>().Where(w=>w.F_BatchId== F_BatchId).First();
			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("找不到 F_BatchId：{1},{0}", ex.Message, F_BatchId));
				ULogger.Log(ex.Message);
			}
			finally
			{

			}
			return null;
		}

		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public static List<SensorGroupData> QueryData(long F_BatchId,double span = -1,int floorNO = -1)
		{

			try
			{
				string s = span <= 0 ? "" : " and F_AddTime=" + span + @"";
				string f = floorNO <= 0 ? "" : " and F_FloorNo=" + floorNO + @"";
				return ElectroChemicalSaver.SensorServices.BaseDal.Db.SqlQueryable<SensorGroupData>(
					@"select * from pd_sensorgroupdata_" + F_BatchId + @" where 1=1 " + s + @" " + f + @" order by F_FloorNo,F_AddTime").ToList();

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
				ULogger.Log(ex.Message);
			}
			finally
			{

			}
			return new List<SensorGroupData>();
		}
		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public static List<SensorGroupData> QueryLastData(long F_BatchId) {

			try
			{
				return ElectroChemicalSaver.SensorServices.BaseDal.Db.SqlQueryable<SensorGroupData>(
					@"select * from pd_sensorgroupdata_" + F_BatchId + @"  where F_DataId in
						(
							select MAX(F_DataId) F_DataId from pd_sensorgroupdata_" + F_BatchId + @"  group by F_FloorNo
						) order by F_FloorNo").ToList();

				 

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
				ULogger.Log(ex.Message);
			}
			finally
			{

			}
			return new List<SensorGroupData>();
		}


		#region 调试数据
		public List<ushort> GetRandNums(int min, int max, int num)

		{
			List<ushort> list = new List<ushort>();
			for (int i = 0; i < num; i++)
			{
				Random rd = new Random();
				ushort temp = (ushort)rd.Next(min, max);
				while (list.Contains(temp))
				{
					temp = (ushort)rd.Next(min, max);
				}
				list.Add(temp);
			}

			return list;

		}
		#endregion

		/// <summary>
		///  委托数据
		/// </summary>
		/// <param name="_Batch">批次</param>
		/// <param name="datas_history">数据历史</param>
		public delegate void NewDataEventHandler(Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime);
		public delegate void FinishEventHandler(Batch _Batch, TimeSpan F_AddTime);
		public delegate void CircleOnceEventHandler(Batch _Batch, int circle);
		public delegate void StatusChangedEventHandler(Batch _Batch, LaHuaTaskStatus circle);
		public delegate void ErrorReadHandler(Batch _Batch,int floor,string region,string message);
		public delegate void SucessReadHandler(Batch _Batch, int floor, string region);


	}



}
 
