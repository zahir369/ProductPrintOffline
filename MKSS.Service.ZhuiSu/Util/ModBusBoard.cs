using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MKSS.Service.ZhuiSu.Util;
using MKSS.Service.ZhuiSu;
using MKSS.Util.Log;
using System.Diagnostics;

namespace MKSS.Service.ZhuiSu.Util
{

    public class ModBusBoard
	{
		public ModBusBoardConnection Connection { get; set; }
		int BaudRate
        {
			get {
				if (Connection != null && Connection.DataBus != null && Connection.DataBus.F_ComBaudRate > 0)
					return Connection.DataBus.F_ComBaudRate;
				return  19200;//67135953, 86559079 ,
			}
		}
		public override string ToString()
        {
            return string.Format("{0}_{1}_{2}",this.Connection==null?this.ToString(): Connection);
        }
        public ModBusBoard()
		{
			this.ReadSpeed = 500;
			Init(null);
		}
		public ModBusBoard(ModBusBoardConnection busConfig,int read_speed)
		{
			Connection = busConfig;
			this.ReadSpeed = read_speed;
			Init(busConfig);
		}
		public void Init(ModBusBoardConnection busConfig)
		{
			this.Qualified = new DataQualified();
			this.DataAging = new DataAging(busConfig);
			this._serial = new SerialPort();
			this._tcpClient = new TcpClient();
			modbusFactory = new ModbusFactory();
			modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace));
		}

		public delegate void DataEventHandler(object sender, ModBusBoard.DataEventArgs e);
		public delegate void DataEventAgingHandler(object sender, ModBusBoard.DataEventAgingArgs e);

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

			public DataEventArgs(byte Address, int Index, SensorGroupData ProductData )
			{
				this.Address = Address;
				this.ProductData = ProductData;
				this.Index = Index; 
			}
		}

		public class DataEventAgingArgs : EventArgs
		{
			public DataAgingSensorGroupData ProductData
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
			 
			public DataEventAgingArgs(byte Address, int Index, DataAgingSensorGroupData ProductData )
			{
				this.Address = Address;
				this.ProductData = ProductData;
				this.Index = Index; 
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
		 
		private string _message;

		private SerialPort _serial;

		private TcpClient _tcpClient;

		private ModbusFactory modbusFactory;
		private IModbusMaster _master;
		  
		public event ModBusBoard.DataEventHandler DataEvent;
		public event ModBusBoard.DataEventAgingHandler DataEventAging;
		

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
			get{ return this.Connection.Address; }
		}

		public int ReadSpeed
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
		public DataAging DataAging
		{
			get;
			set;
		}

		private string Message
		{
			set
			{
				this._message = value;
				ULogger.Info(this._message);
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
					this._serial.BaudRate = BaudRate;
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

		string IP { get; set; }
		int PORT { get; set; }
		public void OpenTcp(string ip, int port)
		{
			IP = ip;
			PORT = port;
			if (_tcpClient != null) {
                try
                {
					_tcpClient.Close();
				}
                catch (Exception)
                {
					 
                }
				try
				{
					_tcpClient.Dispose();
				}
				catch (Exception)
				{

				} 
				_tcpClient = null;
			}
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
							//this.Message = "已关闭网络连接！";
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
		 
		 

        public void StopRead()
		{
			this.IsRead = false;
		}


		public class CRoot
		{
			public ModBusBoard XTHIS;
			public int i;
		}
		public class CLineData
		{
			public CRoot Root; 
			public ushort[] DataTemp = new ushort[32];
		} 
		 

		int ReadVoltageErrorCount = 0;
		int ReadVoltageSucessTotal = 0;
		int ReadVoltageErrorTotal = 0;
		/// <summary>
		///  读取电压数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadVoltageRecycle()
		{

			//ULogger.Log(string.Format("开始读取检测数据 {0}:{1} ...", this.Connection.IP, this.Connection.PORT));

			this.DataAging.Reset(this.Address);
			this.IsRead = true;
			while (this.IsRead)
			{

                try {
					if (!this._tcpClient.Connected)
					{
						this.OpenTcp(this.IP, this.PORT);
					}
				}
				catch (Exception ex)
				{
					ULogger.Log(
					string.Format("读取出错 {0}:{1} {2} ",
					this.Connection.IP,
					this.Connection.PORT, ex.Message));
				}

				
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < this.Address.Count)
				{


                    try
                    {
						if (!this._tcpClient.Connected) break;

						DateTime start = DateTime.Now;
						DateTime end001 = DateTime.Now;
						DateTime end002 = DateTime.Now;

						CLineData line = new CLineData();
						line.Root = root;
						line.DataTemp = new ushort[32];
						if (!this.IsRead)
						{
							break;
						}

						bool error_data = true;
						await Task.Run(delegate ()
						{
							try
							{
								line.Root.XTHIS.Index = line.Root.i;
								line.DataTemp = line.Root.XTHIS._master.ReadHoldingRegisters(line.Root.XTHIS.Address[line.Root.i], 3, 32);
								error_data = false;
								ReadVoltageSucessTotal++;
							}
							catch (Exception ex)
							{
								ReadVoltageErrorCount++;
								ReadVoltageErrorTotal++;
								line.Root.XTHIS.Message = string.Format("读取出错【{0}：{1}】：{2}", ReadVoltageSucessTotal, ReadVoltageErrorTotal, ex.Message);
							}
						});


						end001 = DateTime.Now;

						DataAgingSensorGroupData pm = null;
						ModBusBoard.DataEventAgingArgs e = null;
						if (!error_data)
						{
							try
							{
								pm = new DataAgingSensorGroupData(this.DataAging, this.Address[line.Root.i], this.Connection);
								pm.PushData(line.DataTemp, DateTime.Now);
								e = new ModBusBoard.DataEventAgingArgs(this.Address[line.Root.i], this.Index, pm);
								if (this.DataEventAging != null)
								{
									try
									{
										this.DataEventAging(this, e);
									}
									catch (Exception xx)
									{
										line.Root.XTHIS.Message = xx.Message;
									}
								}
							}
							catch (Exception ex)
							{
								line.Root.XTHIS.Message = ex.Message;
							}
						}
						end002 = DateTime.Now;



						pm = null;
						e = null;
						line = null;
						root.i++;

						ULogger.Log(
							string.Format("结束读取检测数据 {0}:{1} {2}S {3}S...",
							this.Connection.IP,
							this.Connection.PORT,
							(end002 - start).TotalSeconds.ToString("f3"),
							(end001 - start).TotalSeconds.ToString("f3")));
					}
					catch (Exception ex)
					{
						ULogger.Log(
						string.Format("读取出错 {0}:{1} {2} ",
						this.Connection.IP,
						this.Connection.PORT, ex.Message));
					}
					

				}
				root = null;

				await Task.Run(delegate ()
				{

                    try
                    {
						if (ReadVoltageErrorCount == 0)
						{
							bool flag = this.ReadSpeed < 1;
							if (flag)
							{
								Thread.Sleep(500);
							}
							else
							{
								Thread.Sleep(this.ReadSpeed);
							}
						}
						else
						{
							ReadVoltageErrorCount = 0;
							Thread.Sleep(this.ReadSpeed <= 0 ? 1000 : this.ReadSpeed);
						}
					}
                    catch (Exception ex)
                    {
						ULogger.Log(
						string.Format("读取出错 {0}:{1} {2} ",
						this.Connection.IP,
						this.Connection.PORT, ex.Message));
					}
					

				});

			}


		}

		/// <summary>
		///  读取电压数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadVoltageOnce()
		{
			//ULogger.Log(string.Format("开始读取检测数据 {0}:{1} X...", this.Connection.IP, this.Connection.PORT));

			DateTime start  =DateTime.Now; 
			this.DataAging.Reset(this.Address);
			//ULogger.Log(string.Format("Reset{0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));


			this.IsRead = true;
			CRoot root = new CRoot();
			root.XTHIS = this;
			root.i = 0;
			while (root.i < this.Address.Count)
			{

				CLineData line = new CLineData();
				line.Root = root; 
				line.DataTemp = new ushort[32];
				if (!this.IsRead)
				{
					break;
				}

				bool error_data = true;
				await Task.Run(delegate ()
				{
					//ULogger.Log(string.Format("Run 1 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

					byte addr = line.Root.XTHIS.Address[line.Root.i];
					try
					{
						line.Root.XTHIS.Index = line.Root.i;
						line.DataTemp = line.Root.XTHIS._master.ReadHoldingRegisters(addr, 4003, 32);
						error_data = false;
					}
					catch (Exception ex)
					{
						line.Root.XTHIS.Message = string.Format("{0}_{1}出错:{2}",this.Connection, addr,ex.Message);
						ReadVoltageErrorCount++;
					}
					//ULogger.Log(string.Format("Run 2 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

				});

				DataAgingSensorGroupData pm = null;
				ModBusBoard.DataEventAgingArgs e = null;
				if (!error_data)
				{
					try
					{
						pm = new DataAgingSensorGroupData(this.DataAging, this.Address[line.Root.i], this.Connection);
						pm.PushData(line.DataTemp, DateTime.Now);
						e = new ModBusBoard.DataEventAgingArgs(this.Address[line.Root.i], this.Index, pm);
						if (this.DataEventAging != null)
						{

							this.DataEventAging(this, e);
						}
					}
					catch (Exception ex)
					{
						line.Root.XTHIS.Message = ex.Message;
					}
				}
				//ULogger.Log(string.Format("Run 3 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

				await Task.Run(delegate ()
				{
					if (ReadVoltageErrorCount == 0)
					{
						bool flag = this.ReadSpeed < 5;
						if (flag)
						{
							Thread.Sleep(100);
						}
						else
						{
							Thread.Sleep(this.ReadSpeed);
						}
					}
					else
					{
						ReadVoltageErrorCount = 0;
						Thread.Sleep(30 * (this.ReadSpeed <= 0 ? 1000 : this.ReadSpeed));
					}

				});
				//ULogger.Log(string.Format("Run 4 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

				pm = null;
				e = null;
				line = null;
				root.i++;

			}
			root = null;
			this.IsRead = false;
			//ULogger.Log(string.Format("已读取 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now- start).TotalSeconds.ToString("f2")));

		}

		 
		/// <summary>
		///  读取电压数据
		/// </summary>
		/// <returns></returns>
		public async Task<float[]> ReadEnv()
		{
			float[] ff = null;
			await Task.Run(delegate ()
			{
				DateTime start = DateTime.Now;
				try
				{
					ushort[] us = _master.ReadHoldingRegisters(1, 99 , 4);
					ff = new float[2];
					byte[] bdata = new byte[4];
					bdata[0] = BitConverter.GetBytes(us[1])[0];
					bdata[1] = BitConverter.GetBytes(us[1])[1];
					bdata[2] = BitConverter.GetBytes(us[0])[0];
					bdata[3] = BitConverter.GetBytes(us[0])[1];
					ff[0] = BitConverter.ToSingle(bdata, 0);
					 
					bdata = new byte[4];
					bdata[0] = BitConverter.GetBytes(us[3])[0];
					bdata[1] = BitConverter.GetBytes(us[3])[1];
					bdata[2] = BitConverter.GetBytes(us[2])[0];
					bdata[3] = BitConverter.GetBytes(us[2])[1];
					ff[1] = BitConverter.ToSingle(bdata, 0);

					ULogger.Log(string.Format("读取环境温湿度 {0}:{1} {2}℃ {3}% ......", this.Connection.IP, this.Connection.PORT,ff[0],ff[1]));
				}
				catch (Exception ex)
				{
					Message = string.Format("读取环境温湿度 {0}_{1}出错:{2}", this.Connection, "", ex.Message);
				}
				//ULogger.Log(string.Format("Run 2 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

			});

			//ULogger.Log(string.Format("已读取 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now- start).TotalSeconds.ToString("f2")));
			return ff;
		}

	}
}
