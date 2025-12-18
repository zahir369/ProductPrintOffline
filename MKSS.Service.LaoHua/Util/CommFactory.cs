using NModbus;
using NModbus.Serial;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MKSS.Service.LaoHua.Util;
using MKSS.Service.LaoHua;
using MKSS.Util.Log;
using System.Diagnostics;
using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.Service.LaoHua.Util
{


	[LogTagClass(Title = "数据查询")]
	public class CommFactory
	{
		public ModBusBoardConnection Connection { get; set; }
		public IPAddr IPAddr { get { return new IPAddr(Connection.IP, Connection.PORT); } } 

		public override string ToString()
        {
            return string.Format("{0}_{1}_{2}",this.Connection==null?this.ToString(): Connection);
        }
        public CommFactory()
		{
			this.ReadSpeed = 200;
			Init(null);
		}
		public CommFactory(ModBusBoardConnection busConfig,int read_speed)
		{
			Connection = busConfig;
			this.ReadSpeed = read_speed;
			Init(busConfig);
		}
		public void Init(ModBusBoardConnection busConfig)
		{
			this.Qualified = new DataQualified();
			this.DataAging = new DataAging(busConfig);  
			modbusFactory = new ModbusFactory();
			modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger( busConfig,LoggingLevel.Trace));
		}

		public delegate void DataEventHandler(object sender, CommFactory.DataEventArgs e);
		public delegate void DataEventAgingHandler(object sender, CommFactory.DataEventAgingArgs e);

		public class DataEventArgs : EventArgs
		{
			public SensorGroupData ProductData
			{
				get;
				set;
			}

			public TaskAddress Address
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

			public DataEventArgs(TaskAddress Address, int Index, SensorGroupData ProductData, ReadMode r)
			{
				this.Address = Address;
				this.ProductData = ProductData;
				this.Index = Index;
				this.ReadMode = r;
			}
		}

		public class DataEventAgingArgs : EventArgs
		{
			public DataAgingSensorGroupData ProductData
			{
				get;
				set;
			}

			 
			public TaskAddress Address
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

			public DataEventAgingArgs(TaskAddress Address, int Index, DataAgingSensorGroupData ProductData, ReadMode read)
			{
				this.Address = Address;
				this.ProductData = ProductData;
				this.Index = Index; ReadMode = read;
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

		private ReadMode _ReadMode = ReadMode.WX_StartRead;

		private string _message;
		 
		//private TcpClient _tcpClient;

		private ModbusFactory modbusFactory;
		private IModbusMaster _master;
		  
		public event CommFactory.DataEventHandler DataEvent;
		public event CommFactory.DataEventAgingHandler DataEventAging;
		

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
			get { return PooledConnection.Pool.Of(IPAddr).Connected; }
		}

		/// <summary>
		///  方法已作废
		/// </summary>
		public List<TaskAddress> AddressTemp
		{
			get{ return this.Connection.Address; }
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
		 

		public void OpenTcp( )
		{

			if (string.IsNullOrEmpty(Connection.IP) || Connection.PORT <= 0)
			{
				throw new Exception("error:请输入远程服务器地址");
			}
			else
			{

				TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
				if (_tcpClient.Connected)
				{
					this.Message = "重用已有连接"; 
				}
				else {
					try
					{
						PooledConnection.Pool.Open(IPAddr); 
						_tcpClient = PooledConnection.Pool.Of(IPAddr);
					}
					catch (Exception e)
					{
						this.Message = e.Message;
						return;
					}
				}

				
				var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
				this._master = modbusFactory.CreateRtuMaster(adapter);
				this._master.Transport.ReadTimeout = 150;
				this._master.Transport.WriteTimeout = 150;
				this._master.Transport.Retries = 0;
				this._master.Transport.WaitToRetryMilliseconds = 250;
			}

		}
		 
		 

		public void CloseTCPIP()
		{

			bool isRead = this.IsRead;
			if (isRead)
			{
				this.Message = "数据正在读取，请先停止读取数据后再关闭串口！";//_master  modbusFactory
			}


			//必须先关闭 TcpClient,Master.Dispose 会连带释放 TcpClient
			TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
			PooledConnection.Pool.Close(IPAddr);
			if (_master != null) {
				_master.Dispose();
				_master = null;
			}
			 

		}


		public void ReadSignalStartReadData( )
		{
			WX_StartRead ToggleSignal = new WX_StartRead();
			try
			{
				this._master.WriteMultipleRegisters(0, 0, ToggleSignal.GetSignalBytes());
			}
			catch (Exception ex_75)
			{
			}
			Thread.Sleep(200);
		}

		public void StopReadData()
		{
			WX_StopRead ToggleSignal = new WX_StopRead();
			try
			{
				this._master.WriteMultipleRegisters(0, 0, ToggleSignal.GetSignalBytes());
			}
			catch (Exception ex_75)
			{
			}
			Thread.Sleep(200);
		}


		private void ReadSignal(ISignal signal)
		{
			WX_StartRead ToggleSignal = new WX_StartRead();
			try
			{
				this._master.WriteMultipleRegisters(0, 0, ToggleSignal.GetSignalBytes());
			}
			catch (Exception ex_75)
			{
			}
			Thread.Sleep(1000);

			try
			{
				WX_StopRead StopReadSignal = new WX_StopRead();
				this._master.WriteMultipleRegisters(0, 0, StopReadSignal.GetSignalBytes());
			}
			catch
			{
			}
			Thread.Sleep(5);
			try
			{
				this._master.WriteMultipleRegisters(0, 0, signal.GetSignalBytes());
			}
			catch
			{
			}
			Thread.Sleep(5);
		}
		 

        public void StopRead()
		{
			this.IsRead = false;
		}

		private bool WriteSignal(ISignal signal)
		{
			WX_StopRead StopReadSignal = new WX_StopRead();
			
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
			
			return true;
		}


		private bool WriteSignal(ISignal signal, int times = 1, byte addr = 0, bool clean_databus = true)
		{

			if (clean_databus)
			{
				WX_StopRead StopReadSignal = new WX_StopRead();
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
			public ushort[] tempDbl = new ushort[150];
			public ushort[] DataTempDbl = new ushort[150];
		}
		public class CLineDataExt
		{
			public CLineData LineData;
			public ushort[] tempD = new ushort[75];
			public ushort[]  tempH = new ushort[80];
			public ushort[]  tempL = new ushort[70];
		}
		    
		string ToDt16(byte bt)
		{
			string s = string.Format("{0:x}", (int)bt);
			return s.Length == 1 ? "0" + s : s;
		}
 
		 
		 
		int ReadVoltageErrorCount = 0;
 
		public int Debugger
		{
			set
			{
				//ULogger.Log(string.Format("Debug _____________------> {0} ", value));
			}
		}


		/// <summary>
		///  读取电压数据
		/// </summary>
		/// <returns></returns>
		public async Task ReadVoltageOnce(List<TaskAddress> Address)
		{

			try
			{
				//ULogger.Log(string.Format("开始读取老化数据 {0}:{1} X...", this.Connection.IP, this.Connection.PORT));

				Debugger = 3001;
				DateTime start = DateTime.Now;
				this._ReadMode = ReadMode.WX_StartRead;
				//await Task.Run(delegate ()
				//{
				//	this.ReadSignal(new WX_StartRead());
				//});
				 
				//ULogger.Log(string.Format("ReadSignal RX01{0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

				this.DataAging.Reset(Address);
				//ULogger.Log(string.Format("Reset{0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));


				Debugger = 3002;
				this.IsRead = true;
				CRoot root = new CRoot();
				root.XTHIS = this;
				root.i = 0;
				while (root.i < Address.Count)
				{

					Debugger = 3003;
					CLineData line = new CLineData();
					line.Root = root;
					line.temp = new ushort[75];
					line.DataTemp = new ushort[150];
					if (!this.IsRead)
					{
						break;
					}

					Debugger = 3004;
					bool error_data = true;
					bool sucess = await Task<bool>.Run(() =>
					{
						Debugger = 3005;
						//ULogger.Log(string.Format("Run 1 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));
						byte addr = 0;
						try
						{
							int iii = line.Root.i;
							addr = Address[line.Root.i].Address;
							line.Root.XTHIS.Index = line.Root.i;
							
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(addr, 0, 75);
							line.temp.CopyTo(line.DataTemp, 0);
							Task.Delay(this.ReadSpeed < 10 ? 200 : this.ReadSpeed);
							line.temp = line.Root.XTHIS._master.ReadHoldingRegisters(addr, 75, 75);
							line.temp.CopyTo(line.DataTemp, 75);
							error_data = false;
							Debugger = 3006;

							//2 秒钟后再次数据， 滤波算法验证数据有效性 
							Task.Delay(2000);
							line.tempDbl = line.Root.XTHIS._master.ReadHoldingRegisters(addr, 0, 75);
							line.tempDbl.CopyTo(line.DataTempDbl, 0);
							Task.Delay(this.ReadSpeed < 10 ? 200 : this.ReadSpeed);
							line.tempDbl = line.Root.XTHIS._master.ReadHoldingRegisters(addr, 75, 75);
							line.tempDbl.CopyTo(line.DataTempDbl, 75);
							error_data = false;
							Debugger = 3006;


							return true;
						}
						catch (Exception ex)
						{
							//ULogger.Log(string.Format("{0}-----> {1} ", line.Root.i,string.Join(',', Address.Select(w=>w.Address)) ));
							//ULogger.Log(string.Format("{0}", ex.Message));
							//ULogger.Log(ex.StackTrace);
							line.Root.XTHIS.Message = string.Format("{0}@{1}出错:{2}", this.Connection, addr, ex.Message);
							ReadVoltageErrorCount++;
							return false;
						}
						//ULogger.Log(string.Format("Run 2 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

					});

					Debugger = 3007;
					DataAgingSensorGroupData pm = null;
					CommFactory.DataEventAgingArgs e = null;
					if (!error_data)
					{
						try
						{
							pm = new DataAgingSensorGroupData(this.DataAging, Address[line.Root.i], this.Connection);
							pm.PushData(line.DataTemp, line.DataTempDbl, DateTime.Now );
							e = new CommFactory.DataEventAgingArgs(Address[line.Root.i], this.Index, pm,_ReadMode);
							if (this.DataEventAging != null)
							{

								this.DataEventAging(this, e);
							}
							Debugger = 3008;
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
					Debugger = 3009;
					//ULogger.Log(string.Format("Run 4 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now - start).TotalSeconds.ToString("f2")));

					pm = null;
					e = null;
					line = null;
					root.i++;

					Debugger = 3010;
				}
				root = null;
				this.IsRead = false;
				//ULogger.Log(string.Format("已读取 {0}:{1} {2}秒...", this.Connection.IP, this.Connection.PORT, (DateTime.Now- start).TotalSeconds.ToString("f2")));

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("{0}", ex.Message));
				ULogger.Log(ex.StackTrace);
			}
			finally {
				 
				this.IsRead = false;

			}
			Debugger = 3011;

		}

	}
}
