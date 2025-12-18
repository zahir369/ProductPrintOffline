using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.SemiTester;
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

namespace MKSS.Service.SemiTester
{


    [LogTagClass(Title = "检测任务池")]
	public class SemiTesterService
	{
		/// <summary>
		///  调试模式，随机数
		/// </summary>
		public static bool Debug { get; set; } = false;
		public static string[] Regions = new string[] { "A", "B" };
		public static int RegionSize = 32;

		int BaudRate = 19200;
		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 1000 ;//200毫秒钟读一次数据

		DateTime StartTime { get; set; }
		public event NewDataEventHandler OnNewData;
		public event FinishEventHandler OnFinish ;
		
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
		public SemiTesterService( ) {
			BatchSensorDataLastDictionary = new SensorGroupData();
		}

		~SemiTesterService() {
			Cancel = true;
			SemiTesterSaver.Dispose();
		}

		public double Min { get; set; } = int.MaxValue;
		public double Max { get; set; } = int.MinValue;
		public DateTime StartBase { get; set; } = DateTime.Now;
		public TimeSpan SpanBase { get; set; } = new TimeSpan();
		public TimeSpan SpanTotal { get; set; }
		public bool Cancel { get; set; }
		public bool Pause { get;private set; }
		public string COM { get; private set; }
		public List<int> ADDR_LIST { get; private set; }

		/// <summary>
		///  暂停
		/// </summary>
		public void PauseTask() {
			SpanBase = SpanBase + (DateTime.Now - StartBase);
			Pause = true;
		}

		/// <summary>
		///  暂停
		/// </summary>
		public void StopTask()
		{
			Cancel = true;

			ULogger.Info("用户停止，结束！");
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
				Thread.Sleep(200);
			}
		}

		/// <summary>
		///  继续
		/// </summary>
		public void ResumeTask()
		{
			StartBase = DateTime.Now;
			Pause = false;
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
			if (Debug)
			{
				bgWorker.DoWork -= new DoWorkEventHandler(bgWorkerDebug_DoWork);
				bgWorker.DoWork += new DoWorkEventHandler(bgWorkerDebug_DoWork);
			}
			else {
				bgWorker.DoWork -= new DoWorkEventHandler(bgWorker_DoWork);
				bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
			}

			bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
			bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);


			bgWorker.RunWorkerAsync();

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
				BatchSensorDictionary = new Dictionary<string, Sensor>();
				BatchSensorDataDictionary = new Dictionary<TimeSpan, MKSS.Model.SensorGroupData>();
				BatchSensorDataLastDictionary = new SensorGroupData();

				string[] region = SemiTesterService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= SemiTesterService.RegionSize; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = this.CurrentSensors.FirstOrDefault(w => w.F_SensorId == F_SensorId);
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
					}
				}

				SemiTesterSaver.Batch = this.CurrentBatch;
				SemiTesterSaver.Start();
				if (modbusFactory == null) modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));


				this.serialPort1.PortName = COM;
				this.serialPort1.BaudRate = BaudRate;
				this.serialPort1.DataBits = 8;
				this.serialPort1.Parity = Parity.None;
				this.serialPort1.StopBits = StopBits.One;

				try
				{

					if (Cancel) return;
					this.serialPort1.Open();
					ULogger.Info("串口打开成功");
					if (this._master == null) this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
					this._master.Transport.ReadTimeout = 1000;
					this._master.Transport.WriteTimeout = 1000;
					this._master.Transport.Retries = 0;
					this._master.Transport.WaitToRetryMilliseconds = 250;
					StartBase = DateTime.Now;
					SpanBase = new TimeSpan();
					if (this.CurrentBatch.F_AgingLastUpdateTime.Year > 2020)
					{


					}

					while (true)
					{

						try
						{


							if (Cancel) break;

							if (Pause)
							{
								//ULogger.Info("用户暂停！");
								Thread.Sleep(20);
								continue;
							}

							DateTime Now = DateTime.Now;
							SpanTotal = SpanBase + (DateTime.Now - StartBase);
							if (SpanTotal.TotalSeconds > CurrentBatch.F_AgingEndTime)
							{
								ULogger.Info("任务到期，结束！");
								CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
								CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
								CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
								var ress = _BatchServices.Update(CurrentBatch).Result;
								if (OnFinish != null) OnFinish(CurrentBatch, BatchSensorDictionary.Values.ToList(), SpanTotal);
								break;
							}

							if (!Cancel) ReadVoltageRecycleInner(SpanTotal, this.ADDR_LIST);

							int sleep = 200;
							if (StartTaskInterval == TimeSpan.MinValue)
							{
								//动态提供数据刷新频率，采集速度逐渐变慢
								int[][] arr = new int[][] {
										new int[] { 3 ,200 },
										new int[] { 5 ,500 },
										new int[] { 10 ,1000 },
										new int[] { 20 ,2000 },
										new int[] { 30 ,3000 },
										new int[] { 40 ,4000 },
										new int[] { 50 ,5000 },//50分钟
										new int[] { 60 ,6000 },//1小时
										new int[] { 2 * 60 ,7000 },//2小时
										new int[] { 5 * 60 ,8000 },//5小时
										new int[] { 24 * 60 ,10000 },//一天
									};

								sleep = arr[arr.Length - 1][1];
								foreach (var item in arr)
								{
									if (SpanTotal.TotalMinutes <= item[0])
									{
										sleep = item[1];
										break;
									}
								}

							}
							else
							{
								sleep = ((int)StartTaskInterval.TotalMilliseconds);
							}

							int s_temp = 0;
							while (s_temp < (int)sleep)//避免长时间睡眠
							{

								if (Cancel || Pause)
								{
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
		void ReadVoltageRecycleInner(TimeSpan spanTotal,List<int> addr_list)
		{

			Stopwatch watcher = new Stopwatch();
			watcher.Start();
			try
			{
				ReadVoltageRecycleInnerDoing = true;

				if (this.serialPort1.IsOpen)
				{

					if (Cancel) return;

					if (_master != null)
					{
						try
						{

							if (Cancel) return;
							ULogger.Info("开始读取 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
							DateTime beforeDT0 = System.DateTime.Now;
							string[] region = SemiTesterService.Regions;
							TimeSpan F_AddTime = spanTotal;
							SensorGroupData datag = new SensorGroupData()
							{
								F_AddTime = F_AddTime.TotalSeconds,
								F_BatchId = this.CurrentBatch.F_BatchId,
								F_DataId = string.Format("{0}_{1}", this.CurrentBatch.F_BatchId, F_AddTime.TotalSeconds.ToString("f2"))
							};


							bool read_sucess = false;
							for (int r = 0; r < region.Length; r++)
							{

								if (Cancel) return;
								string boardNo = region[r];
								int address = addr_list[r];
								ushort[] ret = null;
								try
								{
									if (Cancel) return;
									ret = _master.ReadHoldingRegisters((byte)address, (ushort)1 + 2, (ushort)SemiTesterService.RegionSize);
									read_sucess = true;
									Thread.Sleep(20);
								}
								catch (Exception ex)
								{
									ULogger.Info(string.Format("读取：{0} 出错", address));
									ULogger.Info(ex.Message);
									ULogger.Info(ex.StackTrace);
									continue;
								}
								for (int i = 0; i < SemiTesterService.RegionSize; i++)
								{
									if (Cancel) return;
									double v = (double)ret[i] * 5 / 10000;
									//ushort a = ret[i];
									//double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a)) / 100;
									//的测量值的每一数据为双字节，高字节在前低字节在后。测量值的计算：输出值 DataN/10000*实际 量程 即为实际测量值。
									datag.SetDataValue(SensorGroupData.PosEnum(boardNo + (i + 1)), v);
									if ((double)v > this.Max) this.Max = (double)v;
									if ((double)v < this.Min) this.Max = (double)v;
								} 
							}
							ULogger.Info("读取完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));

							if (Cancel) return;
							if (OnNewData != null && !Cancel) OnNewData(CurrentBatch, datag, F_AddTime);
							BatchSensorDataDictionary.Add(F_AddTime, datag); 
							ULogger.Info("刷新界面完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));

							try
							{
								int insert_act = 0;
								if (read_sucess)
								{

									if (Cancel) return;
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

									if (Cancel) return;
									SemiTesterSaver.EnqueueTask(sb.ToString());

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
				}
				else
				{
					ULogger.Info("请打开串口");
				}

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取出错 {0}", ex.Message));
				ULogger.Log(string.Format("读取出错 {0}", ex.StackTrace));
			}
			finally {
				watcher.Stop();
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

				for (int r = 0; r < SemiTesterService.Regions.Length; r++)
				{
					string boardNo = SemiTesterService.Regions[r];
					int address = addr_list[r];
					ushort[] ret = null;
					try
					{
						ret = _master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)SemiTesterService.RegionSize);
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
			List<Sensor> sens = SemiTesterSaver.SensorServices.BaseDal.Db.Queryable<Sensor>().Where(w=>w.F_BatchId== _bat.F_BatchId).ToList();
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

				string[] region = SemiTesterService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= SemiTesterService.RegionSize; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = sens.FirstOrDefault(w=>w.F_BoardId== b && w.F_SlotNO==s);
						if (sen == null) continue;
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
					}
				}

				List<SensorGroupData> sendatas = new List<SensorGroupData>();
				DataTable table = SemiTesterSaver.SensorDataServices.QueryTable("select * from pd_sensorgroupdata_" + _bat.F_BatchId + " order by F_AddTime").Result;
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
					if (OnNewData != null) OnNewData(CurrentBatch, datas, F_AddTime);
					BatchSensorDataDictionary.Add(F_AddTime, datas); 
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




		#region 调试数据

		bool DoWorkingDebug = false;
		[LogTagClass(Title = "读取任务Debug")]
		public void bgWorkerDebug_DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{

				Cancel = false;
				Pause = false;
				DoWorkingDebug = true;

				Min = int.MaxValue;
				Max = int.MinValue;
				BatchSensorDictionary = new Dictionary<string, Sensor>();
				BatchSensorDataDictionary = new Dictionary<TimeSpan, MKSS.Model.SensorGroupData>();
				BatchSensorDataLastDictionary = new SensorGroupData();

				string[] region = SemiTesterService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= SemiTesterService.RegionSize; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = this.CurrentSensors.FirstOrDefault(w => w.F_SensorId == F_SensorId);
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
					}
				}

				SemiTesterSaver.Batch = this.CurrentBatch;
				SemiTesterSaver.Start();



				try
				{

					if (Cancel) return;

					ULogger.Info("模拟数据打开成功");

					StartBase = DateTime.Now;
					SpanBase = new TimeSpan();
					if (this.CurrentBatch.F_AgingLastUpdateTime.Year > 2020)
					{


					}

					while (true)
					{

						try
						{


							if (Cancel) break;

							if (Pause)
							{
								//ULogger.Info("用户暂停！");
								Thread.Sleep(20);
								continue;
							}

							DateTime Now = DateTime.Now;
							SpanTotal = SpanBase + (DateTime.Now - StartBase);
							if (SpanTotal.TotalSeconds > CurrentBatch.F_AgingEndTime)
							{
								ULogger.Info("任务到期，结束！");
								CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
								CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
								CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
								var ress = _BatchServices.Update(CurrentBatch).Result;
								if (OnFinish != null) OnFinish(CurrentBatch, BatchSensorDictionary.Values.ToList(), SpanTotal);
								break;
							}

							if (!Cancel) ReadVoltageRecycleDebugInner(SpanTotal, this.ADDR_LIST);

							int sleep = 200;
							if (StartTaskInterval == TimeSpan.MinValue)
							{
								//动态提供数据刷新频率，采集速度逐渐变慢
								int[][] arr = new int[][] {
										new int[] { 3 ,200 },
										new int[] { 5 ,500 },
										new int[] { 10 ,1000 },
										new int[] { 20 ,2000 },
										new int[] { 30 ,3000 },
										new int[] { 40 ,4000 },
										new int[] { 50 ,5000 },//50分钟
										new int[] { 60 ,6000 },//1小时
										new int[] { 2 * 60 ,7000 },//2小时
										new int[] { 5 * 60 ,8000 },//5小时
										new int[] { 24 * 60 ,10000 },//一天
									};

								sleep = arr[arr.Length - 1][1];
								foreach (var item in arr)
								{
									if (SpanTotal.TotalMinutes <= item[0])
									{
										sleep = item[1];
										break;
									}
								}

							}
							else
							{
								sleep = ((int)StartTaskInterval.TotalMilliseconds);
							}

							int s_temp = 0;
							while (s_temp < (int)sleep)//避免长时间睡眠
							{

								if (Cancel || Pause)
								{
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

					}

					ULogger.Info("正常退出X ！");

				}
				catch (Exception ex)
				{
					ULogger.Info(ex.Message);
					ULogger.Info("测试数据打开失败，请检查 是否可用！");

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


				DoWorkingDebug = false;
			}
		}



		bool ReadVoltageRecycleDebugInnerDoing = false;
		[LogTagClass(Title = "读取任务Debug")]
		void ReadVoltageRecycleDebugInner(TimeSpan spanTotal, List<int> addr_list)
		{

			Stopwatch watcher = new Stopwatch();
			watcher.Start();
			try
			{
				ReadVoltageRecycleDebugInnerDoing = true;

				{

					if (Cancel) return;

					{
						try
						{

							if (Cancel) return;
							ULogger.Info("开始读取 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
							DateTime beforeDT0 = System.DateTime.Now;
							string[] region = SemiTesterService.Regions;
							TimeSpan F_AddTime = spanTotal;
							SensorGroupData datag = new SensorGroupData()
							{
								F_AddTime = F_AddTime.TotalSeconds,
								F_BatchId = this.CurrentBatch.F_BatchId,
								F_DataId = string.Format("{0}_{1}", this.CurrentBatch.F_BatchId, F_AddTime.TotalSeconds.ToString("f2"))
							};


							bool read_sucess = false;
							for (int r = 0; r < region.Length; r++)
							{

								if (Cancel) return;
								string boardNo = region[r];
								int address = addr_list[r];
								ushort[] ret = null;
								try
								{
									if (Cancel) return;
									ret = GetRandNums(1000, 2000, SemiTesterService.RegionSize).ToArray();
									read_sucess = true;
									Thread.Sleep(20);
								}
								catch (Exception ex)
								{
									ULogger.Info(string.Format("读取：{0} 出错", address));
									ULogger.Info(ex.Message);
									ULogger.Info(ex.StackTrace);
									continue;
								}
								for (int i = 0; i < SemiTesterService.RegionSize; i++)
								{
									if (Cancel) return;
									double v = (double)ret[i] * 5 / 10000;
									//ushort a = ret[i];
									//double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a)) / 100;
									//的测量值的每一数据为双字节，高字节在前低字节在后。测量值的计算：输出值 DataN/10000*实际 量程 即为实际测量值。
									datag.SetDataValue(SensorGroupData.PosEnum(boardNo + (i + 1)), v);
									if ((double)v > this.Max) this.Max = (double)v;
									if ((double)v < this.Min) this.Max = (double)v;
								}
							}
							ULogger.Info("读取完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));

							if (Cancel) return;
							if (OnNewData != null && !Cancel) OnNewData(CurrentBatch, datag, F_AddTime);
							BatchSensorDataDictionary.Add(F_AddTime, datag);
							ULogger.Info("刷新界面完成 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));

							try
							{
								int insert_act = 0;
								if (read_sucess)
								{

									if (Cancel) return;
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

									if (Cancel) return;
									SemiTesterSaver.EnqueueTask(sb.ToString());

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
				} 

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("读取出错 {0}", ex.Message));
				ULogger.Log(string.Format("读取出错 {0}", ex.StackTrace));
			}
			finally
			{
				watcher.Stop();
				ULogger.Info("结束 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
				ReadVoltageRecycleDebugInnerDoing = false;
			}


		}

		public List<ushort> GetRandNums(int min, int max,int num)

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

	}

}
 
