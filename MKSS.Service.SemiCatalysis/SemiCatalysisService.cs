using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.SemiCatalysis;
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

namespace MKSS.Service.SemiCatalysis
{


    [LogTagClass(Title = "检测任务池")]
	public class SemiCatalysisService
	{

		/// <summary>
		///  调试模式，随机数
		/// </summary>
		public static bool Debug { get; set; } = false;

		int BaudRate = 19200;
		public static string[] Regions = new string[] { "A" }; 
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
		public Dictionary<TimeSpan, Dictionary<string, MKSS.Model.SensorData>> BatchSensorDataDictionary { get; set; }
		public Dictionary<string, MKSS.Model.SensorData> BatchSensorDataLastDictionary { get; set; }

		public Dictionary<string, MKSS.Model.SensorData> this[TimeSpan ts] {
			get {
				var find = BatchSensorDataDictionary.Where(w => Math.Abs(w.Key.TotalSeconds - ts.TotalSeconds) < 1).Select(w=>w.Value).FirstOrDefault();
				return find;
			}
		}

		/// <summary>
		///  获取某些事件点的数据
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns></returns>
		public Dictionary<int, Dictionary<string, MKSS.Model.SensorData>> GetDataRange(int[] seconds)
		{
			Dictionary<int, Dictionary<string, MKSS.Model.SensorData>> ret = new Dictionary<int, Dictionary<string, SensorData>>();
            foreach (var w in seconds)
            {
				ret.Add(w, new Dictionary<string, SensorData>());
                foreach (var s in BatchSensorDataLastDictionary.Keys.ToArray())
                {
					ret[w].Add(s, null);

				}
			}
            foreach (TimeSpan ts in BatchSensorDataDictionary.Keys.ToArray())
            {
                foreach (int s in seconds)
                {
					if (Math.Abs(ts.TotalSeconds - s) < 1) {
						foreach (string sensor_id in BatchSensorDataDictionary[ts].Keys)
						{
							ret[s][sensor_id] = BatchSensorDataDictionary[ts][sensor_id];
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
		public MKSS.Model.SensorData this[string boardNo, int slotNo] {
			get {
				if (CurrentBatch == null) return null;
				string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, boardNo, slotNo);
				return BatchSensorDataLastDictionary[F_SensorId];
			}
		}

		Batch CurrentBatch { get; set; }
		List<Sensor> CurrentSensors { get; set; }
		public bool RefreshTaskDoing { get { return DoWorking; } }
		ModbusFactory modbusFactory;
		IModbusMaster _master;
		SerialPort serialPort1 = new SerialPort();
		public SemiCatalysisService( ) {

		}

		~SemiCatalysisService() {
			Cancel = true;
			SemiCatalysisSaver.Dispose();
		}
		 
		public DateTime StartBase { get; set; } = DateTime.Now;
		public TimeSpan SpanBase { get; set; } = new TimeSpan();
		public TimeSpan SpanTotal { get; set; }
		public bool Cancel { get; set; }
		public bool Pause { get;private set; }

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
		}

		/// <summary>
		///  继续
		/// </summary>
		public void ResumeTask()
		{
			StartBase = DateTime.Now;
			Pause = false;
		}

		public bool DoWorking = false;
		public TimeSpan StartTaskInterval { get; set; } = TimeSpan.MinValue;
		[LogTagClass(Title = "开启任务")]
		public Task StartTask(Batch _bat, List<Sensor> sens, ProductConfig product,string COM, List<int> addr_list) {

			var newTask = new Task(() =>
			{
				try
				{

					if (RefreshTaskDoing) return;
					DoWorking = true;
					Cancel = false;
					Pause = false;
					 
					CurrentBatch = _bat;
					CurrentSensors = sens;
					BatchSensorDictionary = new Dictionary<string, Sensor>();
					BatchSensorDataDictionary = new Dictionary<TimeSpan
						, Dictionary<string, MKSS.Model.SensorData>>();
					BatchSensorDataLastDictionary = new Dictionary<string, SensorData>();

					foreach (var b in Regions)
					{
						for (int s = 1; s <= 16; s++)
						{
							string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
							Sensor sen = sens.FirstOrDefault(w=>w.F_SensorId== F_SensorId);
							BatchSensorDictionary.Add(sen.F_SensorId, sen);
							BatchSensorDataLastDictionary.Add(sen.F_SensorId, new SensorData() { F_SensorId = sen.F_SensorId,Sensor=sen });
						}
					}

					SemiCatalysisSaver.Batch = _bat;
					SemiCatalysisSaver.Start();

					

					try
					{

						if (!Debug)
						{
							if (modbusFactory == null) modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));

							this.serialPort1.PortName = COM;
							this.serialPort1.BaudRate = BaudRate;
							this.serialPort1.DataBits = 8;
							this.serialPort1.Parity = Parity.None;
							this.serialPort1.StopBits = StopBits.One;

							this.serialPort1.Open();
							ULogger.Info("串口打开成功");
							if (this._master == null) this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
							this._master.Transport.ReadTimeout = 1000;
							this._master.Transport.WriteTimeout = 1000;
							this._master.Transport.Retries = 0;
							this._master.Transport.WaitToRetryMilliseconds = 250;

						}
						
						StartBase = DateTime.Now;
						if (_bat.F_AgingLastUpdateTime.Year > 2020)
                        {


                        }

						while (true)
						{

							try
							{
								 

								if (Cancel)
								{
									ULogger.Info("用户停止，结束！");
									CurrentBatch.F_AgingEndTimeActual = DateTime.Now;
									CurrentBatch.F_AgingLastUpdateTime = DateTime.Now;
									CurrentBatch.EnumAgingStatus = EnumAgingStatus.Finished;
									var ress = _BatchServices.Update(CurrentBatch).Result;
									SpanBase = new TimeSpan();
									try
									{
										if (!Debug) {
											if (serialPort1.IsOpen) serialPort1.Close();
										}
											
									}
									catch (Exception) { }
									break;
								}

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
									if (OnFinish != null) OnFinish(_bat, BatchSensorDictionary.Values.ToList(), SpanTotal);
									break;
								}

								ReadVoltageRecycleInner(SpanTotal, addr_list);

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


					}
					catch (Exception ex)
					{
						ULogger.Info(ex.Message);
						ULogger.Info("串口打开失败，请检查系统串口是否可用！");
						
						return;
					}
					finally
					{
						DoWorking = false;
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
						if (!Debug)
						{
							if (serialPort1.IsOpen) serialPort1.Close();
						}
					}
					catch (Exception)
					{

					}

					DoWorking = false;
				}
			});

			return newTask;

		}


		[LogTagClass(Title = "读取任务")]
		void ReadVoltageRecycleInner(TimeSpan spanTotal,List<int> addr_list)
		{
			if (!Debug)
			{
				if ((this.serialPort1.IsOpen) && (_master != null))
				{

				}
				else
				{
					ULogger.Info("serialPort1 not Open or _master is null");
					return;
				}
			}
			try
			{

				DateTime beforeDT0 = System.DateTime.Now;
				TimeSpan F_AddTime = spanTotal;
				Dictionary<string, MKSS.Model.SensorData> datas = new Dictionary<string, SensorData>();
				for (int r = 0; r < Regions.Length; r++)
				{
					if (r > addr_list.Count - 1) continue;
					string boardNo = Regions[r];
					int address = addr_list[r];
					ushort[] ret = null;
					try
					{
						if (!Debug)
						{
							ret = _master.ReadHoldingRegisters((byte)address, (ushort)2, (ushort)16);
							Thread.Sleep(20);
						}
						else
						{
							//if (this.SpanTotal.TotalSeconds < 4) ret = GetRandNums((int)(3000 * 0.5 / 5.0), (int)(3000 * 0.8 / 5.0), 16).ToArray();
							//else ret = GetRandNums((int)(3000 * 2.0 / 5.0), (int)(3000 * 3.3 / 5.0), 16).ToArray();
							ret = new ushort[16];
							for (int i = 0; i < 16; i++)
                            {
								ret[i] = (ushort)(1000 + 2000/16*i);
							}
							Thread.Sleep(20);
						}
					}
					catch (Exception ex)
					{
						ULogger.Info(string.Format("读取：{0} 出错", address));
						ULogger.Info(ex.Message);
						ULogger.Info(ex.StackTrace);
						continue;
					} 
					for (int i = 0; i < 16; i++)
					{
						ushort a = ret[i];
						double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a))/100;
						//v = (ushort)((double)v / (double)100);
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, boardNo, i + 1);
						datas.Add(F_SensorId, new SensorData() { 
							F_AddTime = F_AddTime.TotalSeconds, F_DataId = Guid.NewGuid().ToString(),
							F_DataValue = v, F_SensorId = F_SensorId,
							Sensor = BatchSensorDictionary[F_SensorId]
						}); 
						BatchSensorDataLastDictionary[F_SensorId].F_AddTime = datas[F_SensorId].F_AddTime;
						BatchSensorDataLastDictionary[F_SensorId].F_DataValue = datas[F_SensorId].F_DataValue;
						BatchSensorDataLastDictionary[F_SensorId].F_DataId = datas[F_SensorId].F_DataId;
						BatchSensorDataLastDictionary[F_SensorId].Sensor = BatchSensorDictionary[F_SensorId];
					}
					ULogger.Info("读取耗时 :'" + (System.DateTime.Now - beforeDT0) + "'秒");
				}
				if (OnNewData != null) OnNewData(CurrentBatch, datas,F_AddTime);
				BatchSensorDataDictionary.Add(F_AddTime, datas);
				ULogger.Info("刷新界面耗时 :'" + (System.DateTime.Now - beforeDT0) + "'秒");

				try
				{
					int insert_act = 0;
					if (datas.Count > 0)
					{

						StringBuilder sb = new StringBuilder();
						foreach (var item in datas.Values.ToList())
						{
							if (item.F_DataValue == 0 || item.F_DataValue == 170) continue;//170 无用数据不再存储
							insert_act++;
							sb.Append(
								string.Format(
									"insert into pd_sensordata_{0} (F_DataId, F_SensorId, F_DataValue,F_AddTime)VALUES ('{1}', '{2}', {3},{4});",
									CurrentBatch.F_BatchId, item.F_DataId, item.F_SensorId, item.F_DataValue, item.F_AddTime
							));
						}

						if (sb.Length > 0)
						{
							SemiCatalysisSaver.EnqueueTask(sb.ToString());
						}

					}
				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
				}

			}
			catch (Exception ex)
			{
				ULogger.Info(ex.Message);
				ULogger.Info(ex.StackTrace);
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

				for (int r = 0; r < Regions.Length; r++)
				{
					if (r > addr_list.Count - 1) continue;
					string boardNo = Regions[r];
					int address = addr_list[r];
					ushort[] ret = null;
					try
					{
						ret = _master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)16);
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
			List<Sensor> sens = SemiCatalysisSaver.SensorServices.BaseDal.Db.Queryable<Sensor>().Where(w=>w.F_BatchId== _bat.F_BatchId).ToList();
			try
			{
				 
				CurrentBatch = _bat;
				CurrentSensors = sens;
				BatchSensorDictionary = new Dictionary<string, Sensor>();
				BatchSensorDataDictionary = new Dictionary<TimeSpan
					, Dictionary<string, MKSS.Model.SensorData>>();
				BatchSensorDataLastDictionary = new Dictionary<string, SensorData>();
				GC.Collect();

				string[] region = new string[] { "A" };
				foreach (var b in region)
				{
					for (int s = 1; s <= 16; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = sens.FirstOrDefault(w=>w.F_BoardId== b && w.F_SlotNO==s);
						if (sen == null) continue;
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
						BatchSensorDataLastDictionary.Add(sen.F_SensorId, new SensorData() { F_SensorId = sen.F_SensorId, Sensor = sen });
					}
				}

				List<SensorData> sendatas = new List<SensorData>();
				DataTable table = SemiCatalysisSaver.SensorDataServices.QueryTable("select * from pd_sensordata_"+ _bat.F_BatchId + " order by F_AddTime,F_SensorId").Result;
				foreach (DataRow item in table.Rows)
                {
					sendatas.Add(new SensorData()
					{
						F_AddTime = Convert.ToDouble(item["F_AddTime"]),
						F_DataId = item["F_DataId"] + "",
						F_DataValue = Convert.ToDouble(item["F_DataValue"]),
						F_SensorId = item["F_SensorId"] + ""
					});
				}
				var sendatas_dic = sendatas.GroupBy(w => w.F_AddTime).OrderBy(w=>w.Key).ToDictionary(w => TimeSpan.FromSeconds(w.Key), w => w.ToDictionary(w => w.F_SensorId, w => w));

				foreach (TimeSpan F_AddTime in sendatas_dic.Keys)
                {
					Dictionary<string,SensorData> datas = sendatas_dic[F_AddTime];
                    foreach (SensorData sd in datas.Values)
                    {
						sd.Sensor = BatchSensorDictionary[sd.F_SensorId];
					}
					if (OnNewData != null) OnNewData(CurrentBatch, datas, F_AddTime);
					BatchSensorDataDictionary.Add(F_AddTime, datas);
					Thread.Sleep(50);
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
				DoWorking = false;
			}

		}

		/// <summary>
		///  委托数据
		/// </summary>
		/// <param name="_Batch">批次</param>
		/// <param name="datas_history">数据历史</param>
		public delegate void NewDataEventHandler(Batch _Batch, Dictionary<string, MKSS.Model.SensorData> datas_history,TimeSpan F_AddTime);
		public delegate void FinishEventHandler(Batch _Batch ,List<Sensor> sens, TimeSpan F_AddTime);


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

	}

}
 
