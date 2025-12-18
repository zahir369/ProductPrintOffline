using NModbus;
using NModbus.Serial;
using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace DeviceDataMonitorWPF.UIBiaoDing.Util
{

    public class ModBusBoard
	{
		public string ID { get; set; }
		public int TunnelNo { get; set; }
		public int BoardCaseNo { get; set; }
        public override string ToString()
        {
            return string.Format("{0}_{1}_{2}",this.BoardCaseNo,this.TunnelNo,this.IsOpen);
        }
        public ModBusBoard()
		{
			ID = DateTime.Now.ToString();
			Init(0,0);
		}
		public ModBusBoard(int boardcase_no, int tunnel_no,int read_speed)
		{
			ID = DateTime.Now.ToString();
			BoardCaseNo = boardcase_no;
			TunnelNo = tunnel_no;
			this.ReadSpeed = read_speed;
			Init(boardcase_no, tunnel_no);
		}
		public void Init(int boardcase_no,int tunnel_no)
		{
			this.Address = new List<Address>(); 
			this.Qualified = new DataQualified();
			this._serial = new SerialPort();
			this._tcpClient = new TcpClient();
			modbusFactory = new ModbusFactory();
			modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));
		}

		public delegate void DataEventHandler(object sender, ModBusBoard.DataEventArgs e);

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
		 
		public delegate void MessageEventHander(object sender, ModBusBoard.MessageEventArgs e);

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

		private ReadMode _ReadMode = ReadMode.RX01;

		private string _message;

		private SerialPort _serial;

		private TcpClient _tcpClient;
		private string IP { get; set; }
		private int PORT { get; set; }
		private ModbusFactory modbusFactory;
		private IModbusMaster _master;
		  
		public event ModBusBoard.DataEventHandler DataEvent;
		
		public event ModBusBoard.MessageEventHander MessageEvent;

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
		public bool IsSerial
		{
			get;
			set;
		}

		public bool IsOpen
		{
			get {
				if (IsSerial)
				{
					return this._serial != null && this._serial.IsOpen;
				}
				else
				{
					return this._tcpClient != null && this._tcpClient.Connected;
				}
			}
		}

		public List<Address> Address
		{
			get;
			set;
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

		public DataQualified Qualified
		{
			get;
			set;
		}
 

		private string Message
		{
			set
			{
				this._message = value;
				ModBusBoard.MessageEventArgs e = new ModBusBoard.MessageEventArgs(this._message);
				MessageLogCore(this._message);
				bool flag = this.MessageEvent != null;
				if (flag)
				{
					this.MessageEvent(this, e);
				}
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

			if (this.IsOpen)  return true;

			int tryTimes = 0;
			while (!this.IsOpen)
			{
				if (tryTimes >= 3) return false;
				tryTimes++;
				if (IsSerial) {
					Open(this._serial.PortName);
				}
                else
                {
					OpenTcp(IP, PORT);
				}
			}
			return this.IsOpen;

		}


		public void Open(string COM)
		{
			this.IsSerial = true;
			this.Qualified.Clear();
			bool isOpen = this._serial.IsOpen;
			if (isOpen)
			{
				this.Message = "error:串口设备已被其它端口占用";
			}
			else
			{
				bool flag = COM == "";
				if (flag)
				{
					this.Message = "error:请选择你的设备端口！";
				}
				else
				{
					this._serial.PortName = COM;
					this._serial.BaudRate = 19200;
					this._serial.DataBits = 8;
					this._serial.Parity = Parity.None;
					this._serial.StopBits = StopBits.One;
					bool flag2 = !this._serial.IsOpen;
					if (flag2)
					{
						try
						{
							this._serial.Open();
							this.Message = "串口打开成功";
						}
						catch
						{
							this.Message = "error:串口打开失败，请检查系统串口是否可用！";
							return;
						}
					}
					this._master = modbusFactory.CreateRtuMaster(this._serial);
					this._master.Transport.ReadTimeout = 1000;
					this._master.Transport.WriteTimeout = 1000;
					this._master.Transport.Retries = 0;
					this._master.Transport.WaitToRetryMilliseconds = 250;
				}
			}
		}

		public void OpenTcp(string ip, int port)
		{

			this.IsSerial = false;
			this.Qualified.Clear();
			this.IP = ip;
			this.PORT = port;
			if (_tcpClient == null) _tcpClient = new TcpClient();
			bool connected = this._tcpClient.Connected;
			if (connected)
			{
				this.Message = "error:tcp已经连接";
			}
			else
			{
				bool flag = ip == "";
				if (flag)
				{
					this.Message = "error:请输入远程服务器地址";
				}
				else
				{
					try
					{
						this._tcpClient.Connect(ip, port);
						this.Message = "已连接 "+ ip + ":"+ port + " ！";
					}
					catch (Exception e)
					{
						this.Message = e.Message;
						return;
					}
					var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
					this._master = modbusFactory.CreateRtuMaster(adapter);
					this._master.Transport.ReadTimeout = 1000;
					this._master.Transport.WriteTimeout = 1000;
					this._master.Transport.Retries = 0;
					this._master.Transport.WaitToRetryMilliseconds = 250;
				}
			}
		}
		 

		public void Close()
		{
			bool isRead = this.IsRead;
			if (isRead)
			{
				this.Message = "数据正在读取，请先停止读取数据后再关闭串口！";
			}
			else
			{
				bool isOpen = this._serial.IsOpen;
				if (isOpen)
				{
					try
					{
						this._serial.Close();
						bool flag = !this._serial.IsOpen;
						if (flag)
						{
							this.Message = "已关闭串口！";
						}
					}
					catch
					{
						this.Message = "关闭串口失败！";
					}
				}
			}
		}

		public void CloseTCPIP()
		{
			bool isRead = this.IsRead;
			if (isRead)
			{
				this.Message = "数据正在读取，请先停止读取数据后再关闭串口！";
			}
			else
			{
				bool isOpen = this._tcpClient!=null && this._tcpClient.Connected;
				if (isOpen)
				{
					try
					{
						this._tcpClient.Close();
						bool flag = !this._tcpClient.Connected;
						if (flag)
						{
							this.Message = "已关闭网络连接！";
						}
						_tcpClient = null;
					}
					catch
					{
						this.Message = "关闭网络连接失败！";
					}
				}
			}
		}

		private void ReadSignal(DeviceDataMonitorWPF.UIBiaoDing.Signal.ISignal signal)
		{
			try
			{
				WXFF_StopRead StopReadSignal = new WXFF_StopRead();
				this._master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch
			{
			}
			Thread.Sleep(50);
			try
			{
				this._master.WriteMultipleRegisters(0, 0, signal.GetSignalBytes());
			}
			catch
			{
			}
			Thread.Sleep(50);
		}

        public void StopRead()
		{
			this.IsRead = false;
			this.ReadAllContinue = false;
		}

		private bool WriteSignal(DeviceDataMonitorWPF.UIBiaoDing.Signal.ISignal signal,int times =1,byte addr = 0,bool clean_databus = true)
		{

			//终止循环读取的指令
			this.ReadAllContinue = false;
			UISheBeiBiaoDingViewModel.Intance.BtnQueryIsRead = false;
			UISheBeiBiaoDingViewModel.Intance.BtnQueryIsRead = false;

			if (clean_databus) {
				WXFF_StopRead StopReadSignal = new WXFF_StopRead();
				//RX01_Read StartReadSignal = new RX01_Read();
				try
				{
					this._master.WriteMultipleRegisters(addr, 0, StopReadSignal.GetSignalBytes()); 
				}
				catch (Exception ex_25)
				{
					this.Message = ex_25.Message;
				}
			}
			

            for (int i = 0; i < times; i++)
			{
				Thread.Sleep(200);
				try
				{
					this._master.WriteMultipleRegisters(addr, 0, signal.GetSignalBytes());
				}
				catch (Exception ex_4D)
				{
					this.Message = ex_4D.Message;
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
			WXFF_StopRead StopReadSignal = new WXFF_StopRead(); 
			try
			{
				this._master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch (Exception ex_25)
			{
				this.Message = ex_25.Message;
			}
			return true;
		}

		public void SetLiangCheng(int LiangCheng)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX99_WriteLiangCheng
			{
				LiangCheng = LiangCheng
			});
			this.IsWrite = false;
			this.Message = string.Format("已完成发送设置传感器量程（{0}）指令；", LiangCheng);
		}

		public void SetVolageOutPutRange(int MinVolage, int MaxVolage)
		{
			this.IsWrite = true;
			this.WriteSignal(new WXA4_WriteVolageOutPutRange
			{
				LowVoltage = (ushort)MinVolage,
				HightVoltage = (ushort)MaxVolage
			});
			this.IsWrite = false;
			this.Message = string.Format("已完成发送设置传感器模拟输出电压范围({0}-{1})指令；", MinVolage, MaxVolage);
		}

		public void SetAutoAdjust(bool Open)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX79_WriteAutoAdjust
			{
				Open = Open
			});
			this.IsWrite = false;
			if (Open)
			{
				this.Message = "已完成发送开启传感器自动校准功能指令；";
			}
			else
			{
				this.Message = "已完成发送关闭传感器自动校准功能指令；";
			}
		}


		public void SetModeQuery(bool queryMode)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX78_WriteModeQuery
			{
				 IsQueryMode = queryMode
			});
			this.IsWrite = false;//0x78-切换主被动上传模式（通讯模式：0x03-主动上传；0x04-问询模式）
			if (queryMode)
			{
				this.Message = "已完成发送开启被动上传模式功能指令；";
			}
			else
			{
				this.Message = "已完成发送开启主动上传模式功能指令；";
			}
		}

		public void SetZero(int Zero, int TemperatureZero, int DifferenceValue)
		{
			this.IsWrite = true;
			bool sucess = this.WriteSignal(new WX87_WriteAdjustZeroAppend
			{
				Zero = Zero,
				DifferenceValue = DifferenceValue * 2,
				TemperaturePoint = TemperatureZero
			});
			this.IsWrite = false;
			this.Message = (sucess ? string.Format("已完成发送校准传感器零点({0})指令；", Zero) : "标定失败");
		}

		public void SetZero(int Zero)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX87_WriteAdjustZero
			{
				Zero = Zero
			});
			this.IsWrite = false;
			this.Message = string.Format("已完成发送校准传感器零点({0})指令；", Zero);
		}


		public void SetSerialNo(Address address,int pos,ulong serialno,bool clean_databus)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX94_WriteSerialNo
			{
				 Postion = pos, SerialNo= serialno
			},2,address.Vb, clean_databus);
			this.IsWrite = false;
			this.Message = string.Format("已完成发送设置位置{0}串号({1})指令；",pos, serialno);
		}


		public void SetDeviceDateTime(DateTime d, int times = 1)
		{
			this.IsWrite = true;
			this.WriteSignal(new WXAB_WriteDeviceDateTime
			{
				 DateTime = d
			}, times);
			this.IsWrite = false;
			this.Message = string.Format("已完成发送设置设备时间({0})指令；", d);
		}

		public void SetManufacturingDate(DateTime d, int times = 1)
		{
			this.IsWrite = true;
			this.WriteSignal(new WXAC_WriteManufacturingDate
			{
				DateTime = d
			}, times);
			this.IsWrite = false;
			this.Message = string.Format("已完成发送设置出厂日期({0})指令；", d.ToString("yyyy-MM-dd"));
		}


		public void SetSpan(int Span)
		{
			this.IsWrite = true;
			this.WriteSignal(new WX88_WriteSpan
			{
				Span = Span
			});
			this.IsWrite = false;
			this.Message = string.Format("已完成发送校准传感器Span({0})指令；", Span);
		}

		public void SetTemperature(int Temperature)
		{
			this.IsWrite = true;
			this.WriteSignal(new WXA1_WriteTemperature
			{
				Temperature = (ushort)Temperature
			});
			this.IsWrite = false;
			this.Message = string.Format("已完成发送校准传感器温度值({0})指令；", Temperature);
		}

		public void SetLedStatus()
		{
			this.IsWrite = true;
			WXFF_StopRead StopReadSignal = new WXFF_StopRead();
			try
			{
				this._master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch
			{
			}
			Thread.Sleep(500);
			for (int i = 0; i < this.Address.Count; i++)
			{
				if (!CheckConnection()) return;//自动重连不成功，那么直接退出
				WXA8_WriteLedStatus WriteSignalA8 = new WXA8_WriteLedStatus();
				WriteSignalA8.LedStatus = this.Qualified.CurrentProductData[this.Address[i]].GetProductQualified();
				try
				{
					this._master.WriteMultipleRegisters(this.Address[i].Vb, 0, WriteSignalA8.GetSignalBytes());
				}
				catch
				{
				}
				Thread.Sleep(500);
				this.Message = string.Format("已完成发送工装板（{0}）设置指示灯指令；", this.Address[i]);
			}
			this.IsWrite = false;
			this.Message = string.Format("已完成所有工装板设置指示灯指令；", new object[0]);
		}


		public class CRoot
		{
			public ModBusBoard XTHIS;
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
		public async Task ReadLiangCheng()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX9B_ReadLiangCheng;
				this.ReadSignal(new RX9B_ReadLiangCheng());
			});
			this.IsRead = true;

			CRoot root = new CRoot
			{
				XTHIS = this,
				i = 0
			}; 
			while (root.i < this.Address.Count)
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

				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempD = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempD = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 75);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].LiangCheng = new int?((int)line.temp[i * 10 + 1]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].LiangCheng = pm.SingleAddressData[j].LiangCheng;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i],pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取读取传感器量程已完成；";
		}


		public bool ReadVoltageRangeContinue { get; set; } = true;
		public async Task ReadVoltageRange()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RXA5_ReadVoltageRange;
				this.ReadSignal(new RXA5_ReadVoltageRange());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].DianYaRange = string.Format("{0}-{1}", line.temp[i * 10], line.temp[i * 10 + 1]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].DianYaRange = pm.SingleAddressData[j].DianYaRange;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取传感器模拟电压设定范围已完成";
		}

		public bool ReadAutoAdjustStatusContinue { get; set; } = true;
		public async Task ReadAutoAdjustStatus()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX7D_ReadAutoAdjustStatus;
				this.ReadSignal(new RX7D_ReadAutoAdjustStatus());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].AutoAdjustState = new bool?(false);
					if (line.temp[i * 10 + 1] == 160)
					{
						pm.SingleAddressData[i].AutoAdjustState = new bool?(true);
					}
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].AutoAdjustState = pm.SingleAddressData[j].AutoAdjustState;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取传感器自动校准状态已完成！";
		}

		public bool ReadOutPutVolageContinue { get; set; } = true;
		public async Task ReadOutPutVolage()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RXA9_ReadOutPutVolage;
				this.ReadSignal(new RXA9_ReadOutPutVolage());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].OutPutVolage = new int?((int)line.temp[i * 10 + 9]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].OutPutVolage = pm.SingleAddressData[j].OutPutVolage;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取模拟值输出电压已完成！";
		}

		public async Task ReadTemperatureZero(List<TemPointConfig> temperaturePointList)
		{
			bool flag = this.Address.Count == 0;
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
		public async Task ReadSerialNo()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RX90_ReadSerialNo;
				this.ReadSignal(new RX95_ReadSerialNo());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;

			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						Thread.Sleep(2000);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 75);
						Thread.Sleep(2000);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
				for (int i = 0; i < 15; i++)
				{
					ushort[] us = line.temp;
					byte[] a0 = BitConverter.GetBytes(us[i * 10 + 0]);
					byte[] a1 = BitConverter.GetBytes(us[i * 10 + 1]); 
					byte[] a2 = BitConverter.GetBytes(us[i * 10 + 2]);
					byte[] a3 = BitConverter.GetBytes(us[i * 10 + 3]);
					byte[] a4 = BitConverter.GetBytes(us[i * 10 + 4]);
					string hexString = string.Format("{0}{1}{2}{3}{4}{5}"
						,"00"
						, ToDt16(a0[1])
						, ToDt16(a0[0])
						, ToDt16(a1[1])
						, ToDt16(a1[0])
						, ToDt16(a2[1])
						, ToDt16(a2[0]) 
						);
					ulong sno = UInt64.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
					if (sno.ToString() == "2852126720"|| sno.ToString() == "0")//空位置
					{
						pm.SingleAddressData[i].Serial = "";
					}
					else {
						pm.SingleAddressData[i].Serial = string.Format("{0}", sno.ToString());
					}
					//blist = new byte[] { 0, 0, a0[0], a0[1], a1[0], a1[1], a2[0], a2[1] };
					//byte[] blist = new byte[] { a2[0], a2[1], a1[0], a1[1], a0[0], a0[1], 0, 0 };
					//ulong sno = BitConverter.ToUInt64(blist,0);//舍弃前两个字节暂时用不到
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取设备串号已完成！";
		}
		 

		public bool ReadDeviceDateTimeContinue { get; set; } = true;
		public async Task ReadDeviceDateTime()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RXAB_ReadDeviceDateTime;
				this.ReadSignal(new RXB2_ReadDeviceDateTime());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			Thread.Sleep(1000);//等待足够时间，让总线把数据汇总完
			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						Thread.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 75);

						Thread.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified, new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
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
						, u0
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
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取设备当前时间已完成！";
		}

		string ToDt16(byte bt) {
			string s  =  string.Format("{0:x}", (int)bt);
			return s.Length == 1 ? "0"+ s : s;
		}

		public bool ReadManufacturingDateContinue { get; set; } = true;
		public async Task ReadManufacturingDate()
		{
			await Task.Run(delegate ()
			{
				this._ReadMode = ReadMode.RXAC_ReadManufacturingDate;
				this.ReadSignal(new RXAC_ReadManufacturingDate());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
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
				await Task.Run(delegate ()
				{
					try
					{
						lineExt.LineData.Root.XTHIS.Index = lineExt.LineData.Root.i;
						Thread.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 0, 75);
						Thread.Sleep(1000);//等待足够时间，让总线把数据汇总完
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i].Vb, 75, 75);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 75);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified, new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
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
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
				}
				ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
				if (this.DataEvent != null)
				{
					this.DataEvent(this, e);
				}
				await Task.Run(delegate ()
				{
					Thread.Sleep(this.ReadSpeed);
				});
				line = null;
				pm = null;
				e = null;
				root.i++;
			}
			root = null;
			this.IsRead = false;
			this.Message = "读取模拟值输出电压已完成！";
		}


		/// <summary>
		///  读取所有数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadAllNoThread()
		{
			this._ReadMode = ReadMode.RX01;
			this.ReadSignal(new WX01_StartRead()); 
			this.IsRead = true;
			while (this.IsRead)
			{
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < this.Address.Count)
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
						line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 0, 75);
						line.temp.CopyTo(line.DataTemp, 0);
						line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 75, 75);
						line.temp.CopyTo(line.DataTemp, 75);
					}
					catch (Exception ex)
					{
						line.Root.XTHIS.Message = ex.Message;
					}
					SensorGroupData pm = new SensorGroupData(this.Qualified,line.DataTemp, this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
					if (this.Qualified.CurrentProductData.Count > line.Root.i)
					{
						for (int i = 0; i < 15; i++)
						{
							pm.SingleAddressData[i].AutoAdjustState = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].AutoAdjustState;
							pm.SingleAddressData[i].DianYaRange = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].DianYaRange;
							pm.SingleAddressData[i].LiangCheng = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].LiangCheng;
							pm.SingleAddressData[i].OutPutVolage = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].OutPutVolage;
						} 
						this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
					}
					else
					{
						this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
					}
					 
					ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					bool flag = this.ReadSpeed < 200;
					if (flag)
					{
						Thread.Sleep(200);
					}
					else
					{
						Thread.Sleep(this.ReadSpeed);
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
				this.ReadSignal(new WXFF_StopRead());
			});
		}


		public bool ReadAllContinue { get; set; } = true;
		/// <summary>
		///  读取所有数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadAll()
		{
			this._ReadMode = ReadMode.RX01;
			 
			await Task.Run(delegate ()
			{
				this.ReadSignal(new WX01_StartRead());
			});
			this.IsRead = true;
			while (this.IsRead)
			{
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < this.Address.Count)
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

					//await Task.Run(delegate ()
					//{
					//	try
					//	{
					//		line.Root.XTHIS.Index = line.Root.i;
					//		line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 0, 75);
					//		line.temp.CopyTo(line.DataTemp, 0);
					//		Thread.Sleep(1000);
					//		line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 75, 75);
					//		line.temp.CopyTo(line.DataTemp, 75);
					//	}
					//	catch (Exception ex)
					//	{
					//		line.Root.XTHIS.Message = ex.Message;
					//	}
					//});

					Task<bool> sendEmailMessage = Task.Run(() => {
						try
						{
							line.Root.XTHIS.Index = line.Root.i;
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 0, 75);
							line.temp.CopyTo(line.DataTemp, 0);
							Task.Delay(this.ReadSpeed < 10?200: this.ReadSpeed);
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i].Vb, 75, 75);
							line.temp.CopyTo(line.DataTemp, 75);
							return true;
						}
						catch (Exception ex)
						{
							line.Root.XTHIS.Message = ex.Message;
							return false;
						}

					});

					bool result = await sendEmailMessage;

					if (result) {

						SensorGroupData pm = new SensorGroupData(this.Qualified, line.DataTemp, this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now);
						if (this.Qualified.CurrentProductData.Count > line.Root.i)
						{
							for (int i = 0; i < 15; i++)
							{
								pm.SingleAddressData[i].AutoAdjustState = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].AutoAdjustState;
								pm.SingleAddressData[i].DianYaRange = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].DianYaRange;
								pm.SingleAddressData[i].LiangCheng = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].LiangCheng;
								pm.SingleAddressData[i].OutPutVolage = this.Qualified.CurrentProductData[this.Address[line.Root.i]].SingleAddressData[i].OutPutVolage;
							}
							this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
						}
						else
						{
							this.Qualified.CurrentProductData.TryAdd(this.Address[line.Root.i], pm);
						}

						ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
						if (this.DataEvent != null)
						{
							this.DataEvent(this, e);
						}

						pm = null;
						e = null;

					}


					await Task.Run(delegate ()
					{
						bool flag = this.ReadSpeed < 10;
						if (flag)
						{
							Thread.Sleep(200);
							//Task.Delay(200);
						}
						else
						{
							Thread.Sleep(this.ReadSpeed);
						}
					});


					line = null;
					root.i++;

				}

				root = null;
			}

			await Task.Run(delegate ()
			{
				this.ReadSignal(new WXFF_StopRead());
			});

		}
		 
	}
}
