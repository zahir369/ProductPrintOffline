using NModbus;
using NModbus.Serial;
using MKSS.APP.UIBiaoDing.Signal;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading; 
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using MKSS.Service.LaoHua;

namespace MKSS.APP.UIBiaoDing.Util
{

    public class CommFactory
	{
		public string ID { get; set; }
		public IModbusMaster Master { get; set; }
		public BoardConnection Connection { get;private set; }
		public bool Connected { get { return Connection != null && Connection.IsOpen && Master != null; } }
		public override string ToString()
        {
            return string.Format("{0}_{1}",this.Connection ,this.IsOpen);
        }
        public CommFactory(BoardConnection conn)
		{
			ID = DateTime.Now.ToString();
			Connection = conn;
			Init( );
		}
		public CommFactory(BoardConnection conn, int read_speed)
		{
			ID = DateTime.Now.ToString(); 
			this.ReadSpeed = read_speed;
			Connection = conn;
			Init( );
		}
		public void Init( )
		{ 
		}

		private ReadMode _ReadMode = ReadMode.WX_StartRead;

		private string _message;

		  
		public event CommFactory.DataEventHandler DataEvent;
		
		public event CommFactory.MessageEventHander MessageEvent;

		public bool IsRead
		{
			get;
			set;
		}

		public bool IsWrite
		{
			get;
			set;
		}
		 
		public bool IsOpen
		{
			get {
				return this.Connection.IsOpen;
			}
		}
		 
		public int ReadSpeed
		{
			get;
			set;
		}

		public bool IsLargeVaule
		{
			get;
			set;
		}

		public int Index
		{
			get;
			set;
		}

		string AddrString(List<Address> AddrList)
		{
			string addrs = "";
			foreach (var item in AddrList) addrs += item + ",";
			return addrs;
		}

		public void Open() {
			this.Connection.Open();
		}

		public void Close()
		{
			this.Connection.Close();
		}

		public string Message
		{
			set
			{
				this._message = "【" + this.Connection + "】" + value;
				CommFactory.MessageEventArgs e = new CommFactory.MessageEventArgs(this._message);
				MessageLogCore(this._message);
				bool flag = this.MessageEvent != null;
				if (flag)
				{
					this.MessageEvent(this, e);
				}
			}
		}


		public string MessageLog
		{
			set
			{
				this._message = "【" + this.Connection + "】" + value;
				CommFactory.MessageEventArgs e = new CommFactory.MessageEventArgs(this._message);
				MessageLogCore(this._message); 
			}
		}


		static object locked = new object();
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);
		protected void MessageLogCore(  string message)
		{
			message = message?.Replace(Environment.NewLine, BlankHeader);
			try
			{
				lock (locked)
				{ 
					string n = System.Threading.Thread.CurrentThread.Name;
					string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
					if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
					string filename = dir + DateTime.Now.ToString("HH") + "_" + n + "_INFO.log";
					if (!System.IO.File.Exists(filename)) System.IO.File.Create(filename).Close();
					System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
					System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, Encoding.Default);
					sw.Write(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + message + System.Environment.NewLine);
					sw.Close();
					sw.Dispose();
					fileStream.Close();
					fileStream.Dispose();
				}
			}
			catch (Exception)
			{

			}
		}

		/// <summary>
		///  检查连接 
		/// </summary>
		/// <returns></returns>
		bool CheckConnection() {
			 return Connection.CheckConnection();
		}


		private void ReadSignal(MKSS.APP.UIBiaoDing.Signal.ISignal signal)
		{
			if (!Connected) return;
			if (!Connected) return;
			if (Master==null) return;
			try
			{
				WX_StopRead StopReadSignal = new WX_StopRead();
				if (Master != null) this.Master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch
			{
			}
			MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(50);
			try
			{
				if (Master != null) this.Master.WriteMultipleRegisters(0, 0, signal.GetSignalBytes());
			}
			catch
			{
			}
			MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(50);
		}

        public void StopRead()
		{
			this.IsRead = false;
			this.ReadAllContinue = false;
		}

		private bool WriteSignal(MKSS.APP.UIBiaoDing.Signal.ISignal signal,int times =1,byte addr = 0,bool clean_databus = true)
		{
			//终止循环读取的指令
			this.ReadAllContinue = false;
			UISheBeiBiaoDingViewModel.Intance.BtnQueryIsRead = false;
			UISheBeiBiaoDingViewModel.Intance.BtnQueryIsRead = false;


			if (!Connected) return false;
			if (Master == null) return false;
			if (clean_databus) {
				WX_StopRead StopReadSignal = new WX_StopRead();
				//RX01_Read StartReadSignal = new RX01_Read();
				try
				{
					if (Master != null) this.Master.WriteMultipleRegisters(addr, 0, StopReadSignal.GetSignalBytes()); 
				}
				catch (Exception ex_25)
				{
					//this.Message = ex_25.Message;
				}
			}
			

            for (int i = 0; i < times; i++)
			{
				MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(200);
				try
				{
					if (Master != null) this.Master.WriteMultipleRegisters(addr, 0, signal.GetSignalBytes());
				}
				catch (Exception ex_4D)
				{
					this.MessageLog = ex_4D.Message;
				}
			}
			
			//try
			//{
			//	this._master.WriteMultipleRegisters(0, 0, StartReadSignal.GetSignalBytes());
			//}
			//catch (Exception ex_75)
			//{
			//	this.Message = ex_75.Message;
			//}

			return true;
		}
		public bool WriteSignalClearNoSleep()
		{
			if (!Connected) return false;
			if (Master == null) return false;
			WX_StopRead StopReadSignal = new WX_StopRead(); 
			try
			{
				if (Master != null) this.Master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch (Exception ex_25)
			{
				this.MessageLog = ex_25.Message;
			}
			return true;
		}

		public void SetLiangCheng(int LiangCheng)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteLiangCheng
			{
				LiangCheng = LiangCheng
			});
			this.IsWrite = false;
			this.Message = string.Format("已发送设置传感器量程（{0}）指令；", LiangCheng);
		}

		public void SetVolageOutPutRange(int MinVolage, int MaxVolage)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteVolageOutPutRange
			{
				LowVoltage = (ushort)MinVolage,
				HightVoltage = (ushort)MaxVolage
			});
			this.IsWrite = false;
			this.Message = string.Format("已发送设置传感器模拟输出电压范围({0}-{1})指令；", MinVolage, MaxVolage);
		}

		public void SetAutoAdjust(bool Open)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteAutoAdjust
			{
				Open = Open
			});
			this.IsWrite = false;
			if (Open)
			{
				this.Message = "已发送开启传感器自动校准功能指令；";
			}
			else
			{
				this.Message = "已发送关闭传感器自动校准功能指令；";
			}
		}


		public void SetModeQuery(bool queryMode)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteModeQuery
			{
				 IsQueryMode = queryMode
			});
			this.IsWrite = false;//0x78-切换主被动上传模式（通讯模式：0x03-主动上传；0x04-问询模式）
			if (queryMode)
			{
				this.Message = "已发送开启被动上传模式功能指令；";
			}
			else
			{
				this.Message = "已发送开启主动上传模式功能指令；";
			}
		}

		public void SetZero(int Zero, int TemperatureZero, int DifferenceValue, byte addr)
		{
			if (!Connected) return;
			this.IsWrite = true;
			bool sucess = this.WriteSignal(new WX_WriteAdjustZeroAppend
			{
				Zero = Zero,
				DifferenceValue = DifferenceValue * 2,
				TemperaturePoint = TemperatureZero
			},1, addr);
			this.IsWrite = false;
			this.Message = (sucess ? string.Format("已发送" + this.Connection + ",【" + addr + "】校准传感器零点({0})指令；", Zero) : "" + this.Connection + ",【" + addr + "】标定失败");
		}

		public void SetZero(int Zero, byte addr)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteAdjustZero
			{
				Zero = Zero
			}, 1, addr);
			this.IsWrite = false;
			this.Message = string.Format("已" + this.Connection + ",【" + addr + "】发送校准传感器零点({0})指令；", Zero);
		}


		public void SetSerialNo(Address address,int pos,ulong serialno,bool clean_databus)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteSerialNo
			{
				 Postion = pos, SerialNo= serialno
			},2,address.Vb, clean_databus);
			this.IsWrite = false;
			this.Message = string.Format("已发送" + this.Connection + "位置{0}串号({1})指令；", pos, serialno);
		}


		public void SetDeviceDateTime(DateTime d, int times = 1)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteDeviceDateTime
			{
				 DateTime = d
			}, times);
			this.IsWrite = false;
			this.Message = string.Format("已发送" + this.Connection + "设备时间({0})指令；", d);
		}

		public void SetManufacturingDate(DateTime d, int times = 1)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteManufacturingDate
			{
				DateTime = d
			}, times);
			this.IsWrite = false;
			this.Message = string.Format("已发送" + this.Connection + "出厂日期({0})指令；", d.ToString("yyyy-MM-dd"));
		}


		public void SetSpan(int Span, byte addr)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteSpan
			{
				Span = Span
			},1, addr);
			this.IsWrite = false;
			this.Message = string.Format("已发送" + this.Connection + ",【"  + addr + "】校准传感器Span({0})指令；", Span);
		}

		public void SetTemperature(int Temperature)
		{
			if (!Connected) return;
			this.IsWrite = true;
			this.WriteSignal(new WX_WriteTemperature
			{
				Temperature = (ushort)Temperature
			});
			this.IsWrite = false;
			this.Message = string.Format("已发送" + this.Connection + "校准传感器温度值({0})指令；", Temperature);
		}

		public void SetLedStatus(List<Address> AddrList)
		{
			if (!Connected) return;
			this.IsWrite = true;
			WX_StopRead StopReadSignal = new WX_StopRead();
			try
			{
				this.Master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch
			{
			}
			MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(500);
			for (int i = 0; i < AddrList.Count; i++)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				WX_WriteLedStatus WriteSignalA8 = new WX_WriteLedStatus();
				WriteSignalA8.LedStatus = this.Connection.Pool.Qualified.CurrentProductData[AddrList[i]].GetProductQualified();
				try
				{
					this.Master.WriteMultipleRegisters(AddrList[i].Vb, 0, WriteSignalA8.GetSignalBytes());
				}
				catch
				{
				}
				MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(500);
				this.Message = string.Format("已发送工装板（{0}）设置指示灯指令；", AddrList[i]);
			}
			this.IsWrite = false;
			this.Message = string.Format("已完成" + this.Connection + "所有工装板设置指示灯指令；", new object[0]);
		}


		public class CRoot
		{
			public CommFactory XTHIS;
			public int i;
		}
		public class CLineData
		{
			public CRoot Root;
			public ushort[] temp = new ushort[150];
			public ushort[] DataTemp = new ushort[150];
		}
		public class CLineDataExt
		{
			public CLineData LineData;
			public ushort[] tempD = new ushort[75];
			public ushort[]  tempH = new ushort[80];
			public ushort[]  tempL = new ushort[70];
		}
		public bool ReadLiangChengContinue { get; set; } = true;
		public async Task ReadLiangCheng(List<Address> AddrList)
		{
			if (!Connected) return  ;
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadLiangCheng;
				this.ReadSignal(new RX_ReadLiangCheng());
			});
			this.IsRead = true;

			CRoot root = new CRoot
			{
				XTHIS = this,
				i = 0
			}; 
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData
				{
					Root = root,
					temp = new ushort[150]
				};
				 
				if (!this.ReadLiangChengContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt
				{
					LineData = line,
					tempD = new ushort[75],
				};

				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempD = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempD = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 75);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】量程出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						pm.SingleAddressData[i].LiangCheng = new int?((int)line.temp[i * 10 + 1]);
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].LiangCheng = pm.SingleAddressData[j].LiangCheng;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "传感器量程已完成；";
		}


		public bool ReadVoltageRangeContinue { get; set; } = true;
		public async Task ReadVoltageRange(List<Address> AddrList)
		{
			if (!Connected) return;
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadVoltageRange;
				this.ReadSignal(new RX_ReadVoltageRange());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadVoltageRangeContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】设备电压范围出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						pm.SingleAddressData[i].DianYaRange = string.Format("{0}-{1}", line.temp[i * 10], line.temp[i * 10 + 1]);
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].DianYaRange = pm.SingleAddressData[j].DianYaRange;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "传感器模拟电压设定范围已完成";
		}

		public bool ReadAutoAdjustStatusContinue { get; set; } = true;
		public async Task ReadAutoAdjustStatus(List<Address> AddrList)
		{
			if (!Connected) return;
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadAutoAdjustStatus;
				this.ReadSignal(new RX_ReadAutoAdjustStatus());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadAutoAdjustStatusContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "【" + AddrList[line.Root.i] + "】读取传感器自动校准状态出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;
				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						pm.SingleAddressData[i].AutoAdjustState = new bool?(false);
						if (line.temp[i * 10 + 1] == 160)
						{
							pm.SingleAddressData[i].AutoAdjustState = new bool?(true);
						}
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].AutoAdjustState = pm.SingleAddressData[j].AutoAdjustState;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else { 
					
				}
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "传感器自动校准状态已完成！";
		}

		public bool ReadOutPutVolageContinue { get; set; } = true;
		public async Task ReadOutPutVolage(List<Address> AddrList)
		{
			if (!Connected) return;
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadOutPutVolage;
				this.ReadSignal(new RX_ReadOutPutVolage());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadOutPutVolageContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】输出电压出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						pm.SingleAddressData[i].OutPutVolage = new int?((int)line.temp[i * 10 + 9]);
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].OutPutVolage = pm.SingleAddressData[j].OutPutVolage;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "模拟值输出电压已完成！";
		}

		public async Task ReadTemperatureZero(List<Address> AddrList,List<TemPointConfig> temperaturePointList)
		{
			if (!Connected) return;
			bool flag = AddrList.Count == 0;
			if (flag)
			{
				this.Message = "请输入要读取的地址";
			}
			else
			{
				foreach (TemPointConfig temperaturePoint in temperaturePointList)
				{
				}
				List<TemPointConfig>.Enumerator enumerator = default(List<TemPointConfig>.Enumerator);
				this.IsRead = false;
			}
		}

		public bool ReadSerialNoContinue { get; set; } = true;
		public async Task ReadSerialNo(List<Address> AddrList)
		{
			if (!Connected) return;

			MessageLog = "开始读取设备串号";
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadSerialNo;
				this.ReadSignal(new RX_ReadSerialNo());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;

			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadSerialNoContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(Config.BiaoDingConfgig.Instance.ReadSerialNoWait1);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 75);
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(Config.BiaoDingConfgig.Instance.ReadSerialNoWait2);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】设备串号出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						ushort[] us = line.temp;
						byte[] a0 = BitConverter.GetBytes(us[i * 10 + 0]);
						byte[] a1 = BitConverter.GetBytes(us[i * 10 + 1]);
						byte[] a2 = BitConverter.GetBytes(us[i * 10 + 2]);
						byte[] a3 = BitConverter.GetBytes(us[i * 10 + 3]);
						byte[] a4 = BitConverter.GetBytes(us[i * 10 + 4]);
						string hexString = string.Format("{0}{1}{2}{3}{4}{5}"
							, "00"
							, ToDt16(a0[1])
							, ToDt16(a0[0])
							, ToDt16(a1[1])
							, ToDt16(a1[0])
							, ToDt16(a2[1])
							, ToDt16(a2[0])
							);
						ulong sno = UInt64.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
						if (sno.ToString() == "2852126720" || sno.ToString() == "0")//空位置
						{
							pm.SingleAddressData[i].Serial = "";
						}
						else
						{
							pm.SingleAddressData[i].Serial = string.Format("{0}", sno.ToString());
						}
						//blist = new byte[] { 0, 0, a0[0], a0[1], a1[0], a1[1], a2[0], a2[1] };
						//byte[] blist = new byte[] { a2[0], a2[1], a1[0], a1[1], a0[0], a0[1], 0, 0 };
						//ulong sno = BitConverter.ToUInt64(blist,0);//舍弃前两个字节暂时用不到
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "设备串号已完成！";
		}
		 

		public bool ReadDeviceDateTimeContinue { get; set; } = true;
		public async Task ReadDeviceDateTime(List<Address> AddrList)
		{
			if (!Connected) return;
			Message = "开始读取设备当前时间"; 
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadDeviceDateTime;
				this.ReadSignal(new RX_ReadDeviceDateTime());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadDeviceDateTimeContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 75);
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);//等待足够时间，让总线把数据汇总完
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】设备时间出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						ushort[] us = line.temp;
						ushort u0 = (us[i * 10 + 0]);
						byte[] a0 = BitConverter.GetBytes(us[i * 10 + 0]);
						byte[] a1 = BitConverter.GetBytes(us[i * 10 + 1]);
						byte[] a2 = BitConverter.GetBytes(us[i * 10 + 2]);
						pm.SingleAddressData[i].DeviceDateTime =
							string.Format("{0}",
							string.Format("{0}-{1}-{2} {3}:{4}"
							, u0.ToString("0000")
							, a1[1].ToString("00")
							, a1[0].ToString("00")
							, a2[1].ToString("00")
							, a2[0].ToString("00")
							));

						//byte[] a0 = BitConverter.GetBytes(us[i * 10 + 0]);
						//byte[] a1 = BitConverter.GetBytes(us[i * 10 + 1]);
						//byte[] a2 = BitConverter.GetBytes(us[i * 10 + 2]); 
						//byte[] blist = new byte[] { a0[0], a0[1], a1[0], a1[1], a2[0], a2[1], a3[0], a3[1], a4[0], a4[1] };
						//ulong sno = BitConverter.ToUInt64(blist, 2);//舍弃前两个字节暂时用不到
						//pm.SingleAddressData[i].DeviceDateTime = 
						//	string.Format("{0}", string.Format("{0}{1}-{2}-{3} {4}:{5}"
						//	, ToDt16(a0[1])
						//	, ToDt16(a0[0])
						//	, ToDt16(a1[1])
						//	, ToDt16(a1[0])
						//	, ToDt16(a2[1])
						//	, ToDt16(a2[0])
						//	));
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) + "设备当前时间已完成！";
		}

		string ToDt16(byte bt) {
			string s  =  string.Format("{0:x}", (int)bt);
			return s.Length == 1 ? "0"+ s : s;
		}

		public bool ReadManufacturingDateContinue { get; set; } = true;
		public async Task ReadManufacturingDate(List<Address> AddrList)
		{
			if (!Connected) return;
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX_ReadManufacturingDate;
				this.ReadSignal(new RX_ReadManufacturingDate());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < AddrList.Count)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				CLineData line = new CLineData();
				line.Root = root;
				line.temp = new ushort[150];
				if (!this.ReadManufacturingDateContinue)
				{
					break;
				}
				CLineDataExt lineExt = new CLineDataExt();
				lineExt.LineData = line;
				lineExt.tempH = new ushort[80];
				lineExt.tempL = new ushort[70];
				bool sucess = await Task<bool>.Run(() =>
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 0, 75);
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
						return true;
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.MessageLog = "读取【" + AddrList[line.Root.i] + "】出厂日期出错" + ex.Message;
						return false;
					}
				});
				lineExt = null;

				if (sucess)
				{
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, new ushort[150], this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					for (int i = 0; i < 15; i++)
					{
						ushort[] us = line.temp;
						byte[] a0 = BitConverter.GetBytes(us[i * 10 + 0]);
						byte[] a1 = BitConverter.GetBytes(us[i * 10 + 1]);
						byte[] a2 = BitConverter.GetBytes(us[i * 10 + 2]);
						byte[] a3 = BitConverter.GetBytes(us[i * 10 + 3]);
						byte[] a4 = BitConverter.GetBytes(us[i * 10 + 4]);
						byte[] blist = new byte[] { a0[0], a0[1], a1[0], a1[1], a2[0], a2[1], a3[0], a3[1], a4[0], a4[1] };
						ulong sno = BitConverter.ToUInt64(blist, 2);//舍弃前两个字节暂时用不到
						pm.SingleAddressData[i].ManufacturingDate = string.Format("{0}", sno.ToString());
					}
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int j = 0; j < 15; j++)
						{
							this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
						}
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					});
					pm = null;
					e = null;
				}
				else
				{

				}
				
				line = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取" + this.Connection + "" + this.AddrString(AddrList) +"设备出厂日期已完成！";
		}


		/// <summary>
		///  读取所有数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadAllNoThread(List<Address> AddrList)
		{
			if (!Connected) return;
			this._ReadMode = ReadMode.WX_StartRead;
			this.ReadSignal(new WX_StartRead()); 
			this.IsRead = true;
			while (this.IsRead)
			{
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				if (AddrList.Count == 0) break;
				while (root.i < AddrList.Count)
				{
					if (!CheckConnection()) return;//自动重连不成功，那么直接退出
					CLineData line = new CLineData();
					line.Root = root;
					line.temp = new ushort[75];
					line.DataTemp = new ushort[150];
 
					if (!this.ReadAllContinue)
					{
						this.IsRead = false;
						break;
					}

					if (!this.IsRead)
					{
						break;
					}
					try
					{
						line.Root.XTHIS.Index = line.Root.i;
						line.temp = line.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 0, 75);
						line.temp.CopyTo(line.DataTemp, 0);
						line.temp = line.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 75, 75);
						line.temp.CopyTo(line.DataTemp, 75);
					}
					catch (Exception ex)
					{
						line.Root.XTHIS.MessageLog = "【" + AddrList[line.Root.i] + "】读取出错" + ex.Message;
					}
					SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified,line.DataTemp, this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
					if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
						for (int i = 0; i < 15; i++)
						{
							pm.SingleAddressData[i].AutoAdjustState = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].AutoAdjustState;
							pm.SingleAddressData[i].DianYaRange = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].DianYaRange;
							pm.SingleAddressData[i].LiangCheng = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].LiangCheng;
							pm.SingleAddressData[i].OutPutVolage = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].OutPutVolage;
						} 
					}
					else
					{
						this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
					}
					 
					CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					bool flag = this.ReadSpeed < 200;
					if (flag)
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(200);
					}
					else
					{
						MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
					}
					line = null;
					pm = null;
					e = null;
					root.i++;
				}
				root = null;
			}
			await Task.Run(delegate ()
			{
				this.ReadSignal(new WX_StopRead());
			});
		}


		public bool ReadAllContinue { get; set; } = true;
		/// <summary>
		///  读取所有数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadAll(List<Address> AddrList)
		{
			if (!Connected) return;
			this._ReadMode = ReadMode.WX_StartRead;
			 
			await Task.Run(delegate ()
			{
				this.ReadSignal(new WX_StartRead());
			});
			this.IsRead = true;
			string addrs = "";
			foreach (var item in AddrList) addrs += item + ",";
			 Message = string.Format("开始查询数据：{0},{1}", this.Connection.ToString(), addrs);

			while (this.IsRead)
			{

				//Message = string.Format("开始查询1：{0},{1}", this.Connection.ToString(), addrs);
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				if (AddrList.Count == 0) break;
				while (root.i < AddrList.Count)
				{

					//Message = string.Format("开始查询1：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[root.i].Vb);
					if (!CheckConnection()) return;//自动重连不成功，那么直接退出

					CLineData line = new CLineData();
					line.Root = root;
					line.temp = new ushort[75];
					line.DataTemp = new ushort[150];

					//Message = string.Format("开始查询2：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb);
					if (!this.ReadAllContinue)
					{
						this.IsRead = false;
						break;
					}

					//await Task.Run(delegate ()
					//{
					//	try
					//	{
					//		line.Root.XTHIS.Index = line.Root.i;
					//		line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 0, 75);
					//		line.temp.CopyTo(line.DataTemp, 0);
					//		MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(1000);
					//		line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 75, 75);
					//		line.temp.CopyTo(line.DataTemp, 75);
					//	}
					//	catch (Exception ex)
					//	{
					//		line.Root.XTHIS.Message = ex.Message;
					//	}
					//});


					//Message = string.Format("开始查询3：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb);
					Task<bool> sendEmailMessage = Task.Run(() => {
						try
						{
							line.Root.XTHIS.Index = line.Root.i;
							line.temp = line.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 0, 75);
							line.temp.CopyTo(line.DataTemp, 0);
							Task.Delay(this.ReadSpeed < 10?200: this.ReadSpeed);
							line.temp = line.Root.XTHIS.Master.ReadHoldingRegisters(AddrList[line.Root.i].Vb, 75, 75);
							line.temp.CopyTo(line.DataTemp, 75);
							return true;
						}
						catch (Exception ex)
						{
							line.Root.XTHIS.MessageLog = "【" + AddrList[line.Root.i] + "】" + ex.Message;
							return false;
						}

					});

					bool result = await sendEmailMessage;

					//Message = string.Format("开始查询4：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb);
					if (result) {

                        try
                        {
							//line.Root.XTHIS.Message = string.Format("收到数据：{0},{1},{2}", this.Connection.ToString(), AddrList[line.Root.i].APP, AddrList[line.Root.i].V);
							SensorGroupData pm = new SensorGroupData(this.Connection.Pool.Qualified, line.DataTemp, this.IsLargeVaule, AddrList[line.Root.i], DateTime.Now);
							if (this.Connection.Pool.Qualified.CurrentProductData.Count > line.Root.i)
							{
                                try
                                {
									this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
									for (int i = 0; i < 15; i++)
									{
										pm.SingleAddressData[i].AutoAdjustState = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].AutoAdjustState;
										pm.SingleAddressData[i].DianYaRange = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].DianYaRange;
										pm.SingleAddressData[i].LiangCheng = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].LiangCheng;
										pm.SingleAddressData[i].OutPutVolage = this.Connection.Pool.Qualified.CurrentProductData[AddrList[line.Root.i]].SingleAddressData[i].OutPutVolage;
									}
								}
                                catch (Exception)
                                {
									 
                                }
								
							}
							else
							{
								this.Connection.Pool.Qualified.CurrentProductData.TryAdd(AddrList[line.Root.i], pm);
							}

							CommFactory.DataEventArgs e = new CommFactory.DataEventArgs(AddrList[line.Root.i], this.Index, pm, this._ReadMode);
							if (this.DataEvent != null)
							{
								this.DataEvent(this, e);
							}

							pm = null;
							e = null;
						}
                        catch (Exception exx)
                        {
							MessageLog = string.Format("EX：{0},{1}===》{2},{3}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb, exx.Message);
						}
						

					}


					//Message = string.Format("开始查询5：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb);
					await Task.Run(delegate ()
					{
						bool flag = this.ReadSpeed < 10;
						if (flag)
						{
							MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(200);
							//Task.Delay(200);
						}
						else
						{
							MKSS.APP.UIBiaoDing.Util.ViewUtil.Sleep(this.ReadSpeed);
						}
					});


					//Message = string.Format("开始查询6：{0},{1}===》{2}", this.Connection.ToString(), addrs, AddrList[line.Root.i].Vb);
					line = null;
					root.i++;

				}

				root = null;
			}

			Message = string.Format("结束" + this.Connection + "" + this.AddrString(AddrList) + "查询：{0},{1}", this.Connection.ToString(), addrs);

			await Task.Run(delegate ()
			{
				this.ReadSignal(new WX_StopRead());
			});

		}



		public delegate void DataEventHandler(object sender, CommFactory.DataEventArgs e);

		public class DataEventArgs : EventArgs
		{
			public SensorGroupData ProductData
			{
				get;
				set;
			}

			public Address Address
			{
				get;
				set;
			}

			public int Index
			{
				get;
				set;
			}

			public ReadMode ReadMode
			{
				get;
				set;
			}

			public DataEventArgs(Address Address, int Index, SensorGroupData ProductData, ReadMode r)
			{
				this.Address = Address;
				this.ProductData = ProductData;
				this.Index = Index;
				this.ReadMode = r;
			}
		}

		public delegate void MessageEventHander(object sender, CommFactory.MessageEventArgs e);

		public class MessageEventArgs : EventArgs
		{
			public string Message
			{
				get;
				set;
			}

			public MessageEventArgs(string Message)
			{
				this.Message = Message;
			}
		}


	}
}
