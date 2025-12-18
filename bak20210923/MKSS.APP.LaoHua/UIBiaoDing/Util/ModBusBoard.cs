using NModbus;
using NModbus.Serial;
using MKSS.APP.UIBiaoDing.Signal;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MKSS.APP.UIBiaoDing.Util
{

    public class ModBusBoard
	{
		public ModBusBoardConnection Connection { get; set; }
		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.Connection == null ? this.ToString() : Connection);
		}
		public ModBusBoard()
		{
			this.ReadSpeed = 200;
			Init(null);
		}
		public ModBusBoard(ModBusBoardConnection busConfig, int read_speed)
		{
			Connection = busConfig;
			this.ReadSpeed = read_speed;
			Init(busConfig);
		}
		public void Init(ModBusBoardConnection busConfig)
		{
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

			public byte Address
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

			public DataEventArgs(byte Address, int Index, SensorGroupData ProductData, ReadMode r)
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

		private ModbusFactory modbusFactory;
		private IModbusMaster _master;
		  
		public event ModBusBoard.DataEventHandler DataEvent;
		
		public event ModBusBoard.MessageEventHander MessageEvent;

		public bool CanRealSave
		{
			get;
			set;
		}

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
			get;
			set;
		}


		public List<byte> Address
		{
			get { return this.Connection.Address; }
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
				bool flag = this.MessageEvent != null;
				if (flag)
				{
					this.MessageEvent(this, e);
				}
			}
		}

		public void Open(string COM)
		{
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
							this.IsOpen = this._serial.IsOpen;
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
						this.IsOpen = this._tcpClient.Connected;
					}
					catch (Exception e)
					{
						this.Message = e.Message;
						return;
					}
					var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
					this._master = modbusFactory.CreateRtuMaster(adapter);
					this._master.Transport.ReadTimeout = 150;
					this._master.Transport.WriteTimeout = 150;
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
							this.IsOpen = this._serial.IsOpen;
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
							this.IsOpen = this._serial.IsOpen;
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

		private void ReadSignal(MKSS.APP.UIBiaoDing.Signal.ISignal signal)
		{
			try
			{
				XFF_StopRead StopReadSignal = new XFF_StopRead();
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
		}

		private bool WriteSignal(MKSS.APP.UIBiaoDing.Signal.ISignal signal)
		{
			XFF_StopRead StopReadSignal = new XFF_StopRead();
			RX01 ToggleSignal = new RX01();
			try
			{
				this._master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch (Exception ex_25)
			{
			}
			Thread.Sleep(1000);
			try
			{
				this._master.WriteMultipleRegisters(0, 0, signal.GetSignalBytes());
			}
			catch (Exception ex_4D)
			{
			}
			Thread.Sleep(1000);
			try
			{
				this._master.WriteMultipleRegisters(0, 0, ToggleSignal.GetSignalBytes());
			}
			catch (Exception ex_75)
			{
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
			XFF_StopRead StopReadSignal = new XFF_StopRead();
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
				WXA8_WriteLedStatus WriteSignalA8 = new WXA8_WriteLedStatus();
				WriteSignalA8.LedStatus = this.Qualified.CurrentProductData[i].GetProductQualified();
				try
				{
					this._master.WriteMultipleRegisters(this.Address[i], 0, WriteSignalA8.GetSignalBytes());
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
						lineExt.tempD = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 0, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempD = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 75, 75);
						lineExt.tempD.CopyTo(lineExt.LineData.temp, 75);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].LiangCheng = new int?((int)line.temp[i * 10 + 1]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[j].LiangCheng = pm.SingleAddressData[j].LiangCheng;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.Add(pm);
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
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].DianYaRange = string.Format("{0}-{1}", line.temp[i * 10], line.temp[i * 10 + 1]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[j].DianYaRange = pm.SingleAddressData[j].DianYaRange;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.Add(pm);
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
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch
					{
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
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
						this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[j].AutoAdjustState = pm.SingleAddressData[j].AutoAdjustState;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.Add(pm);
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
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
				for (int i = 0; i < 15; i++)
				{
					pm.SingleAddressData[i].OutPutVolage = new int?((int)line.temp[i * 10 + 9]);
				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[j].OutPutVolage = pm.SingleAddressData[j].OutPutVolage;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.Add(pm);
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
				this.ReadSignal(new RX90_ReadSerialNo());
			});
			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
			{
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
						lineExt.tempH = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 0, 80);
						lineExt.tempL = lineExt.LineData.Root.XTHIS._master.ReadHoldingRegisters(lineExt.LineData.Root.XTHIS.Address[lineExt.LineData.Root.i], 80, 70);
						lineExt.tempH.CopyTo(lineExt.LineData.temp, 0);
						lineExt.tempL.CopyTo(lineExt.LineData.temp, 80);
					}
					catch (Exception ex)
					{
						lineExt.LineData.Root.XTHIS.Message = ex.ToString();
					}
				});
				lineExt = null;
				SensorGroupData pm = new SensorGroupData(this.Qualified,new ushort[150], this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
				for (int i = 0; i < 15; i++)
				{
					ushort[] us = line.temp;
					pm.SingleAddressData[i].Serial = string.Format(
						"{0}{1}{2}{3}{4}",
						us[i * 10 + 0], 
						us[i * 10 + 1], 
						us[i * 10 + 2], 
						us[i * 10 + 3], 
						us[i * 10 + 4] );

				}
				if (this.Qualified.CurrentProductData.Count > line.Root.i)
				{
					for (int j = 0; j < 15; j++)
					{
						this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[j].Serial = pm.SingleAddressData[j].Serial;
					}
				}
				else
				{
					this.Qualified.CurrentProductData.Add(pm);
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
			this.ReadSignal(new RX01());
			this.Qualified.Clear();
			this.IsRead = true;
			while (this.IsRead)
			{
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < this.Address.Count)
				{
					CLineData line = new CLineData();
					line.Root = root;
					line.temp = new ushort[75];
					line.DataTemp = new ushort[150];
					if (!this.IsRead)
					{
						break;
					}
					try
					{
						line.Root.XTHIS.Index = line.Root.i;
						line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i], 0, 75);
						line.temp.CopyTo(line.DataTemp, 0);
						line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i], 75, 75);
						line.temp.CopyTo(line.DataTemp, 75);
					}
					catch (Exception ex)
					{
						line.Root.XTHIS.Message = ex.Message;
					}
					SensorGroupData pm = new SensorGroupData(this.Qualified,line.DataTemp, this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
					if (this.Qualified.CurrentProductData.Count > line.Root.i)
					{
						for (int i = 0; i < 15; i++)
						{
							pm.SingleAddressData[i].AutoAdjustState = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].AutoAdjustState;
							pm.SingleAddressData[i].DianYaRange = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].DianYaRange;
							pm.SingleAddressData[i].LiangCheng = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].LiangCheng;
							pm.SingleAddressData[i].OutPutVolage = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].OutPutVolage;
						}
						this.Qualified.CurrentProductData.RemoveAt(line.Root.i);
						this.Qualified.CurrentProductData.Insert(line.Root.i, pm);
					}
					else
					{
						this.Qualified.CurrentProductData.Add(pm);
					}
					if (this.CanRealSave)
					{
						if (this.Qualified.HistoryProductData.Count > line.Root.i)
						{
							this.Qualified.HistoryProductData[line.Root.i].Add(pm);
						}
						else
						{
							List<SensorGroupData> tempList = new List<SensorGroupData>();
							tempList.Add(pm);
							this.Qualified.HistoryProductData.Add(tempList);
							tempList = null;
						}
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
				this.ReadSignal(new RX01());
			});
			this.Qualified.Clear();
			this.IsRead = true;
			while (this.IsRead)
			{
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < this.Address.Count)
				{
					CLineData line = new CLineData();
					line.Root = root;
					line.temp = new ushort[75];
					line.DataTemp = new ushort[150];
					if (!this.ReadAllContinue)
					{
						this.IsRead = false;
						break;
					}
					await Task.Run(delegate ()
					{
						try
						{
							line.Root.XTHIS.Index = line.Root.i;
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i], 0, 75);
							line.temp.CopyTo(line.DataTemp, 0);
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i], 75, 75);
							line.temp.CopyTo(line.DataTemp, 75);
						}
						catch (Exception ex)
						{
							line.Root.XTHIS.Message = ex.Message;
						}
					});
					SensorGroupData pm = new SensorGroupData(this.Qualified, line.DataTemp, this.IsLargeVaule, this.Address[line.Root.i], DateTime.Now.ToString("yyyy/MM/dd HH/mm/ss"));
					if (this.Qualified.CurrentProductData.Count > line.Root.i)
					{
						for (int i = 0; i < 15; i++)
						{
							pm.SingleAddressData[i].AutoAdjustState = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].AutoAdjustState;
							pm.SingleAddressData[i].DianYaRange = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].DianYaRange;
							pm.SingleAddressData[i].LiangCheng = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].LiangCheng;
							pm.SingleAddressData[i].OutPutVolage = this.Qualified.CurrentProductData[line.Root.i].SingleAddressData[i].OutPutVolage;
						}
						this.Qualified.CurrentProductData.RemoveAt(line.Root.i);
						this.Qualified.CurrentProductData.Insert(line.Root.i, pm);
					}
					else
					{
						this.Qualified.CurrentProductData.Add(pm);
					}
					if (this.CanRealSave)
					{
						if (this.Qualified.HistoryProductData.Count > line.Root.i)
						{
							this.Qualified.HistoryProductData[line.Root.i].Add(pm);
						}
						else
						{
							List<SensorGroupData> tempList = new List<SensorGroupData>();
							tempList.Add(pm);
							this.Qualified.HistoryProductData.Add(tempList);
							tempList = null;
						}
					}
					ModBusBoard.DataEventArgs e = new ModBusBoard.DataEventArgs(this.Address[line.Root.i], this.Index, pm, this._ReadMode);
					if (this.DataEvent != null)
					{
						this.DataEvent(this, e);
					}
					await Task.Run(delegate ()
					{
						bool flag = this.ReadSpeed < 10;
						if (flag)
						{
							Thread.Sleep(200);
						}
						else
						{
							Thread.Sleep(this.ReadSpeed);
						}
					});
					line = null;
					pm = null;
					e = null;
					root.i++;
				}
				root = null;
			}
		}
		 
	}
}
