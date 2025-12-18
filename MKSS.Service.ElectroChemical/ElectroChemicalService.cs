using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.Service.ElectroChemical;
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

namespace MKSS.Service.ElectroChemical
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

		DateTime StartTime { get; set; }
		public event NewDataEventHandler OnNewData;
		public event FinishEventHandler OnFinish ;
		
		BatchServices _BatchServices = new BatchServices();

		public Dictionary<string, Sensor> BatchSensorDictionary { get; set; }
		public Dictionary<TimeSpan, MKSS.Model.SensorGroupData> BatchSensorDataDictionary { get; set; }
		public SensorGroupData BatchSensorDataLastDictionary { get; set; }

		public MKSS.Model.SensorGroupData this[TimeSpan ts]
		{
			get
			{
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
			if (BatchSensorDataDictionary != null)
			{
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
		public SensorDataItem this[PosEnum posenum]
		{
			get
			{
				if (CurrentBatch == null) return null;
				return BatchSensorDataLastDictionary.F_DataValue(posenum);
			}
		}


		Batch CurrentBatch { get; set; }
		List<Sensor> CurrentSensors { get; set; }
		ModbusFactory modbusFactory;
		IModbusMaster _master;
		SerialPort serialPort1 = new SerialPort();
		public ElectroChemicalService( ) {

		}

		~ElectroChemicalService() {
			Cancel = true;
			ElectroChemicalSaver.Dispose();
		}

		public double Min { get; set; } = double.MaxValue;
		public double Max { get; set; } = double.MinValue;
		public DateTime StartBase { get; set; } = DateTime.Now;
		public TimeSpan SpanBase { get; set; } = new TimeSpan();
		public TimeSpan SpanTotal { get; set; }
		public bool Cancel { get; set; }
		public bool Pause { get;private set; }
		bool RefreshTaskDoing = false;

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

		public TimeSpan StartTaskInterval { get; set; } = TimeSpan.MinValue;
		[LogTagClass(Title = "开启任务")]
		public Task StartTask(Batch _bat, List<Sensor> sens, ProductConfig product,string COM, List<int> addr_list) {

			var newTask = new Task(() =>
			{
				try
				{

					if (RefreshTaskDoing) return;
					RefreshTaskDoing = true;
					Cancel = false;
					Pause = false;

					Min = int.MaxValue;
					Max = int.MinValue;
					CurrentBatch = _bat;
					CurrentSensors = sens;
					BatchSensorDictionary = new Dictionary<string, Sensor>();
					BatchSensorDataDictionary = new Dictionary<TimeSpan, MKSS.Model.SensorGroupData>();
					BatchSensorDataLastDictionary = new SensorGroupData();

					string[] region = new string[] { "A", "B", "C", "D" };
					foreach (var b in region)
					{
						for (int s = 1; s <= 16; s++)
						{
							string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
							Sensor sen = this.CurrentSensors.FirstOrDefault(w => w.F_SensorId == F_SensorId);
							BatchSensorDictionary.Add(sen.F_SensorId, sen);
						}
					}

					ElectroChemicalSaver.Batch = _bat;
					ElectroChemicalSaver.Start();
					if (modbusFactory==null) modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));


					int BaudRate = 9600;
					this.serialPort1.PortName = COM;
					this.serialPort1.BaudRate = BaudRate;
					this.serialPort1.DataBits = 8;
					this.serialPort1.Parity = Parity.None;
					this.serialPort1.StopBits = StopBits.One;

					try
					{

						this.serialPort1.Open();
						ULogger.Info("串口打开成功");
						if(this._master==null) this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
						this._master.Transport.ReadTimeout = 1000;
						this._master.Transport.WriteTimeout = 1000;
						this._master.Transport.Retries = 0;
						this._master.Transport.WaitToRetryMilliseconds = 250;
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
									try
									{
										if (serialPort1.IsOpen) serialPort1.Close();
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

									//arr = new int[][] {
									//	new int[] { 1 ,50 },
									//	new int[] { 2 ,100 },
									//	new int[] { 3 ,200 },
									//	new int[] { 4 ,500 },
									//	new int[] { 5 ,1000 },
									//	new int[] { 6 ,2 * 1000 },
									//	new int[] { 7 ,3 * 1000 },//50分钟
									//	new int[] { 8 ,5 * 1000 },//1小时
									//	new int[] { 9 ,10 * 1000 },//2小时
									//	new int[] { 10 ,60 * 1000 },//5小时
									//	new int[] { 24 * 60 ,2 * 60 * 1000 },//一天
									//	new int[] { 3 * 24 * 60 ,3 * 60 * 1000 },//三天
									//};

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

				}
				catch (Exception ex)
				{
					ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
					ULogger.Log(ex.Message);
				}
				finally
				{
					RefreshTaskDoing = false;

					try
					{
						if (serialPort1.IsOpen) serialPort1.Close();
					}
					catch (Exception)
					{

					}

				}
			});

			return newTask;

		}


		[LogTagClass(Title = "读取任务")]
		void ReadVoltageRecycleInner(TimeSpan spanTotal,List<int> addr_list)
		{
			if (this.serialPort1.IsOpen)
			{
				if (_master != null)
				{
					try
					{

						DateTime beforeDT0 = System.DateTime.Now;
						string[] region = new string[] { "A", "B", "C", "D" };
						TimeSpan F_AddTime = spanTotal;
						SensorGroupData datag = new SensorGroupData()
						{
							F_AddTime = F_AddTime.TotalSeconds,
							F_BatchId = this.CurrentBatch.F_BatchId,
							F_DataId = string.Format("{0}_{1}", this.CurrentBatch.F_BatchId, F_AddTime.TotalSeconds.ToString("f2"))
						};
						bool any_read_sucess = false;
						for (int r = 0; r < region.Length; r++)
						{

							string boardNo = region[r];
							int address = addr_list[r];
							ushort[] ret = null;
							try
							{
								ret = _master.ReadHoldingRegisters((byte)address, (ushort)1, (ushort)16);
								any_read_sucess = true;
								Thread.Sleep(20);
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
								//ushort v = ret[i];
								string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, boardNo, i + 1);
								double v = (double)ret[i];//
								//ushort a = ret[i];
								//double v = (double)BitConverter.ToInt16(BitConverter.GetBytes(a)) / 100;
								//的测量值的每一数据为双字节，高字节在前低字节在后。测量值的计算：输出值 DataN/10000*实际 量程 即为实际测量值。
								datag.SetDataValue(SensorGroupData.PosEnum(boardNo + (i + 1)), v);
								if ((double)v > this.Max) this.Max = (double)v;
								if ((double)v < this.Min) this.Max = (double)v;
							}
							ULogger.Info("读取耗时 :'" + (System.DateTime.Now - beforeDT0) + "'秒");
						}
						if (Cancel) return;
						if (OnNewData != null && !Cancel) OnNewData(CurrentBatch, datag, F_AddTime);
						BatchSensorDataDictionary.Add(F_AddTime, datag);
						ULogger.Info("刷新界面耗时 :'" + (System.DateTime.Now - beforeDT0) + "'秒");

						try
						{
							int insert_act = 0;
							if (any_read_sucess)
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

								if (sb.Length > 0)
								{
									ElectroChemicalSaver.EnqueueTask(sb.ToString());
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
			}
			else
			{
				ULogger.Info("请打开串口");
			}

		}



		public bool TestConn(string COM, List<int> addr_list)
		{

			try
			{

				modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));
				int BaudRate = 9600;
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

				string[] region = new string[] { "A", "B", "C", "D" };
				for (int r = 0; r < region.Length; r++)
				{
					string boardNo = region[r];
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
			List<Sensor> sens = ElectroChemicalSaver.SensorServices.BaseDal.Db.Queryable<Sensor>().Where(w=>w.F_BatchId== _bat.F_BatchId).ToList();
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

				string[] region = new string[] { "A", "B", "C", "D" };
				foreach (var b in region)
				{
					for (int s = 1; s <= 16; s++)
					{
						string F_SensorId = Sensor.CreateCensorId(CurrentBatch.F_BatchId, b, s);
						Sensor sen = sens.FirstOrDefault(w => w.F_BoardId == b && w.F_SlotNO == s);
						if (sen == null) continue;
						BatchSensorDictionary.Add(sen.F_SensorId, sen);
					}
				}

				List<SensorGroupData> sendatas = new List<SensorGroupData>();
				DataTable table = ElectroChemicalSaver.SensorServices.QueryTable("select * from pd_sensorgroupdata_" + _bat.F_BatchId + " order by F_AddTime").Result;
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
				RefreshTaskDoing = false;

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
		public delegate void NewDataEventHandler(Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime);
		public delegate void FinishEventHandler(Batch _Batch, List<Sensor> sens, TimeSpan F_AddTime);


	}

}
 
