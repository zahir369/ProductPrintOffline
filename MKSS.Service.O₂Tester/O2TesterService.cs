using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.O2Tester;
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
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace MKSS.Service.O2Tester
{


    [LogTagClass(Title = "检测任务池")]
	public class O2TesterService
	{
		/// <summary>
		///  调试模式，随机数
		/// </summary>
		public static bool Debug { get; set; } = false;
		public static string[] Regions = new string[] { "A", "B", "C", "D" };
		public static int RegionSize = 16;

		int BaudRate = 9600;
		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 1000 ;//200毫秒钟读一次数据

		DateTime StartTime { get; set; }
		public event NewDataEventHandler OnNewData;
		public event FinishEventHandler OnStart;
		public event FinishEventHandler OnFinish;
		public event ErrorEventHandler OnError;

		BatchServices _BatchServices = new BatchServices();

		public Dictionary<string, Sensor> BatchSensorDictionary { get; set; }
		public Dictionary<TimeSpan, MKSS.Model.SensorGroupData> BatchSensorDataDictionary { get; set; }
		public SensorGroupData BatchSensorDataLastDictionary { get; set; }

		public MKSS.Model.SensorGroupData this[TimeSpan ts] {
			get {
                try
                {
					var find = BatchSensorDataDictionary.Where(w => Math.Abs(w.Key.TotalSeconds - ts.TotalSeconds) < 1).Select(w => w.Value).FirstOrDefault();
					return find;
				}
                catch (Exception)
                {
					return null;
                }
				
			}
		}

		void OnErrorMessage(string str) {
			if (OnError != null) OnError(str);
		}

		/// <summary>
		///  获取某些事件点的数据
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public Dictionary<int, MKSS.Model.SensorGroupData> GetDataRange(int[] seconds)
		{
			Dictionary<int, MKSS.Model.SensorGroupData> ret = new Dictionary<int, MKSS.Model.SensorGroupData>();
            foreach (var w in seconds)
            {
				ret.Add(w, new SensorGroupData());
				if (BatchSensorDataLastDictionary == null) return ret;
			}
			if (BatchSensorDataDictionary != null) {
				foreach (TimeSpan ts in BatchSensorDataDictionary.Keys.ToArray())
				{
					foreach (int s in seconds)
					{
						if (BatchSensorDataDictionary == null) return ret;
						if (Math.Abs(ts.TotalSeconds - s) < 1)
						{
							ret[s] = BatchSensorDataDictionary[ts];
						}
					}

				}
			}
  
			return ret;
		}

		/// <summary>
		///  获取最新数据
		/// </summary>
		/// <param name="boardNo"></param>
		/// <param name="slotNo"></param>
		/// <returns></returns>
		public SensorDataItem this[PosEnum posenum] {
			get {
				if (CurrentBatch == null) return null;
				return BatchSensorDataLastDictionary.F_DataValue(posenum);
			}
		}

		Batch CurrentBatch { get; set; }
		List<Sensor> CurrentSensors { get; set; }
		ModbusFactory modbusFactory;
		IModbusMaster _master;
		SerialPort serialPort1 = new SerialPort();
		public O2TesterService( ) {
			BatchSensorDataLastDictionary = new SensorGroupData();
		}

		~O2TesterService() {
			Cancel = true;
			O2TesterSaver.Dispose();
		}

		public double Min { get; set; } = int.MaxValue;
		public double Max { get; set; } = int.MinValue;
		public TimeSpan SpanTotal { get; set; } = new TimeSpan(); 
		public bool Cancel { get; set; }
		public bool Pause { get;private set; } 
		public string COM { get; private set; }
		public List<int> ADDR_LIST { get; private set; }

		/// <summary>
		///  暂停
		/// </summary>
		public void PauseTask() {
			Pause = true;
			ULogger.Info("用户暂停："+this.SpanTotal.TotalSeconds.ToString("f2"));
		}

		/// <summary>
		///  暂停
		/// </summary>
		public void StopTask()
		{
			Cancel = true;

			ULogger.Info("用户停止，结束！" + this.SpanTotal.TotalSeconds.ToString("f2"));
			CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
			CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
			CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
			var ress = _BatchServices.Update(CurrentBatch).Result;
			try
			{
				if (serialPort1.IsOpen) serialPort1.Close();
			}
			catch (Exception) { }

			if (bgWorker != null)
			{
				try
				{
					bgWorker.CancelAsync();
					bgWorker.Dispose();
					bgWorker = null;
				}
				catch (Exception)
				{

				} 
			}
		}

		/// <summary>
		///  继续
		/// </summary>
		public void ResumeTask()
		{
			Pause = false;
			ULogger.Info("用户继续：" + this.SpanTotal.TotalSeconds.ToString("f2"));
		}


		public bool RefreshTaskDoing { get { return DoWorking; } }
		public TimeSpan StartTaskInterval { get; set; } = new TimeSpan(0, 0, 0, 0, 100);
		private BackgroundWorker bgWorker = null;

		[LogTagClass(Title = "开启任务")]
		public void StartTask(Batch _bat, List<Sensor> sens, ProductConfig product, string com, List<int> addr_list)
		{

			if (bgWorker!=null)
			{
                try
                {
					bgWorker.CancelAsync();
					bgWorker.Dispose();
					bgWorker = null;
				}
                catch (Exception)
                {

				}
				Thread.Sleep(200);
			}

			this.CurrentBatch = _bat;
			this.CurrentSensors = sens;
			this.COM = com;
			this.ADDR_LIST = addr_list;

			bgWorker = new BackgroundWorker();
			bgWorker.WorkerReportsProgress = true;
			bgWorker.WorkerSupportsCancellation = true;
			bgWorker.ProgressChanged -= new ProgressChangedEventHandler(bgWorker_ProgessChanged);
			bgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
			bgWorker.DoWork -= new DoWorkEventHandler(bgWorker_DoWork);
			bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);

			bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
			bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);

			// 保证 bgWorker_DoWork 启动，对象初始化完成
			BatchSensorDictionary = new Dictionary<string, Sensor>();
			BatchSensorDataDictionary = new Dictionary<TimeSpan, MKSS.Model.SensorGroupData>();
			BatchSensorDataLastDictionary = new SensorGroupData();

			string[] region = O2TesterService.Regions;
			foreach (var b in region)
			{
				for (int s = 1; s <= O2TesterService.RegionSize; s++)
				{
					string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
					Sensor sen = this.CurrentSensors.FirstOrDefault(w => w.F_SensorId == F_SensorId);
					BatchSensorDictionary.Add(sen.F_SensorId, sen);
				}
			}

			O2TesterSaver.Batch = this.CurrentBatch;


			bgWorker.RunWorkerAsync();
			Thread.Sleep(50);

		}


		bool DoWorking = false;
		public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{

				Cancel = false;
				Pause = false;
				DoWorking = true;

				Min = int.MaxValue;
				Max = int.MinValue;

				O2TesterSaver.Start();

				try
				{


					if (Cancel) return;

					if (!Debug) {

						try
						{

							if (modbusFactory == null) modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));

							this.serialPort1.PortName = COM;
							this.serialPort1.BaudRate = BaudRate;
							this.serialPort1.DataBits = 8;
							this.serialPort1.Parity = Parity.None;
							this.serialPort1.StopBits = StopBits.One;

							this.serialPort1.Open();

						}
						catch (Exception ex)
						{
							OnErrorMessage(string.Format(COM + "打开失败：" + ex.Message));
							return;
						}

						ULogger.Info("串口打开成功");
						if (this._master == null) this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
						this._master.Transport.ReadTimeout = 400;//减少超时时间，界面不出现长时间卡顿
						this._master.Transport.WriteTimeout = 400;
						this._master.Transport.Retries = 1;
						this._master.Transport.WaitToRetryMilliseconds = 250;

					}
                    


					SpanTotal = new TimeSpan();
					if (this.CurrentBatch.F_AgingLastUpdateTime.Year > 2020)
					{


					}

					//第一帧
					Stopwatch watcher = null;
					if (OnStart != null) OnStart(CurrentBatch, BatchSensorDictionary.Values.ToList(), SpanTotal);
					
					while (true)
					{
						try
						{


							if (Cancel) break;

							if (Pause)
							{
								watcher = null;
								//ULogger.Info("用户暂停！");
								Thread.Sleep(20);
								continue;
							}

							if (watcher == null) {
								watcher = new Stopwatch();
								watcher.Start();
							}


							if (SpanTotal.TotalSeconds > CurrentBatch.F_AgingEndTime + 1)
							{
								ULogger.Info("任务到期，结束！");
								CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
								CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
								CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
								var ress = _BatchServices.Update(CurrentBatch).Result;
								if (OnFinish != null) OnFinish(CurrentBatch, BatchSensorDictionary.Values.ToList(), SpanTotal);
								break;
							}

							bool read_sucess = false;
							if (!Cancel && !Pause) ReadVoltageRecycleInner(watcher,this.ADDR_LIST,ref read_sucess);

							//开始下一帧,放到这里的目的是为了将 下面的帧间隔也计算进去
							watcher.Stop();
							watcher = null;
							watcher = new Stopwatch();
							watcher.Start();

							int s_temp = 0;
							int sleep = ((int)StartTaskInterval.TotalMilliseconds);
							while (s_temp < (int)sleep)//避免长时间睡眠
							{

								if (Cancel || Pause)
								{
									watcher.Stop();//暂停计时
									watcher = null;
									break;//暂停，或退出，直接跳出

								}
								//等待下次数据采集
								s_temp += 50;
								Thread.Sleep(50);
							}

						}
						catch (Exception exx)
						{
							ULogger.Info("出错结束！错误：" + exx.Message + "");
							break;
						}
						finally {
							
						}

					}

					ULogger.Info("正常退出X ！");

				}
				catch (Exception ex)
				{
					ULogger.Info(ex.Message);
					ULogger.Info("串口打开失败，请检查系统串口是否可用！"); 
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

				try
				{
					if (serialPort1.IsOpen) serialPort1.Close();
				}
				catch (Exception)
				{

				}
				DoWorking = false;
			}
		}

		public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
		{
			 
		}

		public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				ULogger.Info(e.Error.ToString());
				return;
			}
			if (!e.Cancelled)
				ULogger.Info("处理完毕!");
			else
				ULogger.Info("处理终止!");

		}

		bool ReadVoltageRecycleInnerDoing = false;
		[LogTagClass(Title = "读取任务")]
		void ReadVoltageRecycleInner(Stopwatch watcher,List<int> addr_list,ref bool any_read_sucess)
		{

			if (!Debug) {
				if (!this.serialPort1.IsOpen)
				{
					ULogger.Info("请打开串口");
					return;
				}
			}


			any_read_sucess = false;
			//记录当前时间，如有必要回滚时间
			TimeSpan oldSpanTotal = TimeSpan.FromSeconds(SpanTotal.TotalSeconds);
			try
			{

				ReadVoltageRecycleInnerDoing = true;

				if (Cancel || Pause) return;

				try
				{

					if (Cancel || Pause) return;
					ULogger.Info("开始读取 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
					DateTime beforeDT0 = System.DateTime.Now;
					string[] region = O2TesterService.Regions;
					SensorGroupData datag = new SensorGroupData()
					{ 
						F_BatchId = this.CurrentBatch.F_BatchId  
					};

					for (int r = 0; r < region.Length; r++)
					{

						if (Cancel || Pause) return;
						string boardNo = region[r];
						int address = addr_list[r] - 1;
						ushort[] ret = null;
						try
						{
							if (Cancel || Pause) return;

							if (!Debug)
							{
								ret = _master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)O2TesterService.RegionSize);
							}
							else {
								//ret = GetRandNums((int)(3723 * 0.1 / 5.0), (int)(3723 * 5.0 / 5.0), RegionSize).ToArray();
								double mid = 0;//y = -40 * t*t + 800*t -1300
								double buff = 1;// SensorStaData.ADTolerance;//y = -2.667 * t*t + 106.67*t + 1633.33
								double t = this.SpanTotal.TotalSeconds;
								if (t < 5 || t > 35)
								{
									mid = 2100;
									ret = GetRandNums((int)(mid - buff), (int)(mid + buff), O2TesterService.RegionSize).ToArray();
								}
								else {
									mid = -2.667 * t * t + 106.67 * t + 1633.33;
									ret = GetRandNums((int)(mid - buff), (int)(mid + buff), O2TesterService.RegionSize).ToArray();
								}
								//if (this.SpanTotal.TotalSeconds<4) ret = GetRandNums((int)(mid- buff), (int)(mid + buff), O2TesterService.RegionSize).ToArray();
								//else ret = GetRandNums((int)(3723 * 2.0 / 5.0), (int)(3723 * 3.3 / 5.0), O2TesterService.RegionSize).ToArray();
							}
							any_read_sucess = true;
							Thread.Sleep(5);
							if (Cancel || Pause) return;
						}
						catch (TimeoutException ex)
						{
							ULogger.Info(string.Format("读取：{0} 出错", address));
							ULogger.Info(ex.Message);
							ULogger.Info(ex.StackTrace); 
							continue;
						}
						catch (Exception ex)
						{
							ULogger.Info(string.Format("读取：{0} 出错", address));
							ULogger.Info(ex.Message);
							ULogger.Info(ex.StackTrace); 
							continue;
						}
						for (int i = 0; i < O2TesterService.RegionSize; i++)
						{
							if (Cancel || Pause) return;
							double v = (double)ret[i];
							//ushort a = ret[i];
							//double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a)) / 100;
							//的测量值的每一数据为双字节，高字节在前低字节在后。测量值的计算：输出值 DataN/10000*实际 量程 即为实际测量值。
							datag.SetDataValue(SensorGroupData.PosEnum(boardNo + (i + 1)), v);
							if ((double)v > this.Max) this.Max = (double)v;
							if ((double)v < this.Min) this.Max = (double)v;
						} 
					}
							

					try
					{
						int insert_act = 0;
						if (any_read_sucess)
						{

							ULogger.Info("读取完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
							if (Cancel || Pause) return;


							//***********************************************
							//  叠加经过的时间，包含
							//***********************************************
							SpanTotal = SpanTotal.Add(watcher.Elapsed).Add(StartTaskInterval);
									
							TimeSpan F_AddTime = SpanTotal;
							datag.F_AddTime = Math.Round(F_AddTime.TotalSeconds, 2);
							datag.F_DataId = string.Format("{0}_{1}", this.CurrentBatch.F_BatchId, F_AddTime.TotalSeconds.ToString("f3"));
							if (OnNewData != null && !(Cancel || Pause)) OnNewData(CurrentBatch, datag, F_AddTime);
							if (Cancel || Pause) return;
							BatchSensorDataDictionary.Add(F_AddTime, datag);
							ULogger.Info("刷新界面完成["+ this.SpanTotal.TotalSeconds.ToString("f2") + "]：" + this.SpanTotal.TotalSeconds.ToString("f2"));

							if (Cancel || Pause) return;
							StringBuilder sb_f = new StringBuilder();
							StringBuilder sb_v = new StringBuilder();
							foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
							{
								double val = datag.F_DataValue(item).F_DataValue;
								sb_f.Append("," + item);
								sb_v.Append("," + val);
							}

							StringBuilder sb = new StringBuilder();
							sb.Append(
									string.Format(
										"insert into pd_sensorgroupdata_{0} (F_DataId, F_BatchId, F_AddTime {3})VALUES ('{1}', '{0}',{2} {4});",
										CurrentBatch.F_BatchId, datag.F_DataId, datag.F_AddTime, sb_f, sb_v
								));

							if (Cancel || Pause) return;
							O2TesterSaver.EnqueueTask(sb.ToString());

						}
					}
					catch (Exception ex)
					{
						ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
					}
					ULogger.Info("存储完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				}
				catch (Exception ex)
				{
					ULogger.Info(ex.Message);
					ULogger.Info(ex.StackTrace);
				}
				 

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取出错 {0}", ex.Message));
				ULogger.Log(string.Format("读取出错 {0}", ex.StackTrace));
			}
			finally {

				if (Cancel || Pause) {
					SpanTotal = oldSpanTotal;//恢复原有时间，避免时间跳，停不住
				} 
				ULogger.Info("结束 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
				ReadVoltageRecycleInnerDoing = false;
			}
			

		}



		public bool TestConn(string COM, List<int> addr_list)
		{

			try
			{

				modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace)); 
				this.serialPort1.PortName = COM;
				this.serialPort1.BaudRate = BaudRate;
				this.serialPort1.DataBits = 8;
				this.serialPort1.Parity = Parity.None;
				this.serialPort1.StopBits = StopBits.One;

			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("刷新数据时出错 {0}", ex.Message));
			}

			try
			{

				this.serialPort1.Open();
				this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
				this._master.Transport.ReadTimeout = 1000;
				this._master.Transport.WriteTimeout = 1000;
				this._master.Transport.Retries = 0;
				this._master.Transport.WaitToRetryMilliseconds = 250;
			}
			catch (Exception ex)
			{
				throw new Exception("串口打开失败，请检查系统串口是否可用：" + ex.Message);
			}

			try
			{

				for (int r = 0; r < O2TesterService.Regions.Length; r++)
				{
					string boardNo = O2TesterService.Regions[r];
					int address = addr_list[r];
					ushort[] ret = null;
					try
					{
						ret = _master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)O2TesterService.RegionSize);
						Thread.Sleep(50);
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("读取：{0} 出错", address));
					}
				}
				if (serialPort1.IsOpen) serialPort1.Close();

				return true;


			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				try
				{
					if (serialPort1.IsOpen) serialPort1.Close();
				}
				catch (Exception)
				{

				}
			}

			return false;

		}

		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public void InitDataByDb(Batch _bat) {
			List<Sensor> sens = O2TesterSaver.SensorServices.BaseDal.Db.Queryable<Sensor>().Where(w=>w.F_BatchId== _bat.F_BatchId).ToList();
			try
			{

				Min = int.MaxValue;
				Max = int.MinValue;
				CurrentBatch = _bat;
				CurrentSensors = sens;
				BatchSensorDictionary = new Dictionary<string, Sensor>();
				BatchSensorDataDictionary = new Dictionary<TimeSpan
					, SensorGroupData>();
				BatchSensorDataLastDictionary = new SensorGroupData();
				GC.Collect();

				string[] region = O2TesterService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= O2TesterService.RegionSize; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = sens.FirstOrDefault(w=>w.F_BoardId== b && w.F_SlotNO==s);
						if (sen == null) continue;
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
					}
				}

				List<SensorGroupData> sendatas = new List<SensorGroupData>();
				DataTable table = O2TesterSaver.SensorDataServices.QueryTable("select * from pd_sensorgroupdata_" + _bat.F_BatchId + " order by F_AddTime").Result;
				foreach (DataRow item in table.Rows)
                {
					SensorGroupData sen = new SensorGroupData()
					{
						F_AddTime = Convert.ToDouble(item["F_AddTime"]),
						F_DataId = item["F_DataId"] + "",
						F_BatchId = long.Parse(item["F_BatchId"] + "")
					};
					foreach (PosEnum pos in Enum.GetValues(typeof(PosEnum)))
					{
						double val = Convert.ToDouble(item[pos.ToString()]);
						sen.SetDataValue(pos, val);
					}
					sendatas.Add(sen);
				}
				var sendatas_dic = sendatas.OrderBy(w => w.F_AddTime).ToDictionary(w => TimeSpan.FromSeconds(w.F_AddTime), w => w);

				foreach (TimeSpan F_AddTime in sendatas_dic.Keys)
				{
					SensorGroupData datas = sendatas_dic[F_AddTime]; 
					BatchSensorDataDictionary.Add(F_AddTime, datas);
				}
				foreach (TimeSpan F_AddTime in sendatas_dic.Keys)
				{
					SensorGroupData datas = sendatas_dic[F_AddTime];
					if (OnNewData != null) OnNewData(CurrentBatch, datas, F_AddTime); 
				}

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
				ULogger.Log(ex.Message);
			}
			finally
			{

				try
				{
					if (serialPort1.IsOpen) serialPort1.Close();
				}
				catch (Exception ex)
				{

				}

			}

		}

		/// <summary>
		///  委托数据
		/// </summary>
		/// <param name="_Batch">批次</param>
		/// <param name="datas_history">数据历史</param>
		public delegate void NewDataEventHandler(Batch _Batch, SensorGroupData datas_history,TimeSpan F_AddTime);
		public delegate void FinishEventHandler(Batch _Batch ,List<Sensor> sens, TimeSpan F_AddTime);
		public delegate void ErrorEventHandler(string F_AddTime);




		/// <summary>
		///  通过数据查询获取历史数据
		/// </summary>
		public static Batch OfBatch(long F_BatchId)
		{

			try
			{
				return O2TesterSaver.SensorServices.BaseDal.Db.Queryable<Batch>().Where(w => w.F_BatchId == F_BatchId).First();
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
		public static List<SensorGroupData> QueryData(long F_BatchId, double span = -1, int floorNO = -1)
		{

			try
			{
				string s = span <= 0 ? "" : " and F_AddTime=" + span + @"";
				return O2TesterSaver.SensorServices.BaseDal.Db.SqlQueryable<SensorGroupData>(
					@"select * from pd_sensorgroupdata_" + F_BatchId + @" where 1=1 " + s + @" order by F_AddTime").ToList();

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
		public static List<SensorGroupData> QueryLastData(long F_BatchId)
		{

			try
			{
				return O2TesterSaver.SensorServices.BaseDal.Db.SqlQueryable<SensorGroupData>(
					@"select * from pd_sensorgroupdata_" + F_BatchId + @"  where F_DataId in
						(
							select MAX(F_DataId) F_DataId from pd_sensorgroupdata_" + F_BatchId + @" 
						) ").ToList();



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
		public List<ushort> GetRandNums(int min, int max,int num)

		{
			List<ushort> list = new List<ushort>();
			for (int i = 0; i < num; i++)
			{
				Random rd = new Random();
				ushort temp = (ushort)rd.Next(min, max);
				//while (list.Contains(temp))
				//{
				//	temp = (ushort)rd.Next(min, max);
				//}
				list.Add(temp);
			}

			return list;

		} 
		#endregion

	}

}
 
