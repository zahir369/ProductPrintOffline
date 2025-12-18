using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.IO.Ports;

namespace MKSS.APP.Win4In1Tester
{
	public class Model4In1Tester
	{

		public string[][] PAR_V1 = new string[][] { 
			new string[]{ "6", "CO", "PPM", "" }, 
			new string[]{ "12", "PM2.5", "μg/m³", "" },
		};
		 

		// 任务队列
		Queue<byte[]> _tasks = new Queue<byte[]>();
		// 为保证线程安全，使用一个锁来保护_task的访问
		readonly object _locker = new object();
		// 通过 _wh 给工作线程发信号
		EventWaitHandle _wh = null;
		Thread _worker;
		System.Windows.Forms.Timer _workerTimer;
		UIConnection ControlConnection;
		public bool Running { get; private set; }

		SerialPort serialPortMain = null;
		public PROTOCOL Protocal { get { return FormMain.Instance.Protocal; } } 
		public Model4In1Tester(UIConnection form) {
			ControlConnection = form;
		}

		public void StartThread()
		{

			Cancel = false;
			_wh = new AutoResetEvent(false);

			// 任务开始，启动工作线程
			_worker = new Thread(Work);
			_worker.Start();

            try
            {
				_workerTimer = new System.Windows.Forms.Timer();
				_workerTimer.Tick += _workerTimer_Tick;
				_workerTimer.Interval = 1000;
				_workerTimer.Enabled = true;
				//_worker.Start();
			}
            catch (Exception ex)
            {
				ControlConnection.UpdateTextBox(ex.Message);
				throw;
            }


		}

		List<SenV> LastSenV = null;
		private void _workerTimer_Tick(object sender, EventArgs e)
		{
			//9604 型号 发→◇01 03 10 00 00 0A C1 0D
			//9604 收←◆01 03 14 01 B2 01 74 04 64 00 00 00 27 00 64 00 05 00 00 00 00 00 00 AF AC 16 0B 10 00 00 00 00 01 2C 02 EE 00 00 B2 

			//发→◇10 03 00 10 00 08 46 88 □
			//收←◆10 03 10 1C 01 2F 00 00 15 00 00 00 00 00 00 00 00 00 00 BF 4C 
			if (Protocal == PROTOCOL.V3) SendCommand("10 03 00 10 00 08 46 88");
			//三次清空一次数据
			if (LastSenV != null) {
				RefreshUI(LastSenV);
			}
		}

		void RefreshUI(List<SenV> vs) {
			ControlConnection.Invoke((Action)delegate
			{

				if (vs == null) return;
				for (int i = 0; i < ControlConnection.ICS.Length; i++)
				{
					if (vs == null) return;
					if (i < vs.Count)
					{
						ControlConnection.ICS[i].SenV = vs[i];
						ControlConnection.ToolTip.SetToolTip(ControlConnection.ICS[i], string.Format("{0}", DateTime.Now.ToLongTimeString()));
					}
					else
					{
						//ControlConnection.ICS[i].SenV = null;
					}
				}

				Application.DoEvents();
			});
		}

        void WorkTimer() { 
		
		}

		bool IsFullCommand(List<byte> s)
		{

            switch (Protocal)
            {
                case PROTOCOL.No:
					return false; 
                case PROTOCOL.V1:
					if (s.Count == 14) return true;
					s.Clear();
					return false;
				case PROTOCOL.V2:
					if (s.Count == 18) return true;
					s.Clear();
					return false;
				case PROTOCOL.V3:
					if (s.Count >= 2 && (s[0] != 16 || s[1] != 3))
					{
						ControlConnection.UpdateTextBox("首字节错误清空:" + s[0] + "," + s[1]);
						s.Clear();//首字节错误
					}
					if (s.Count == 21) {
						return true;
					}
					if (s.Count > 21)
					{
						ControlConnection.UpdateTextBox("过长清空:" + s.Count);
						s.Clear();//过长
					}
					return false;
				default:
                    break;
            } 
			return false;
		}

		bool Cancel = false;
		/// <summary>执行工作</summary>
		void Work()
		{
			Running = true;

			List<byte> works = new List<byte>();
			while (true)
			{

				if (Cancel) break;

				lock (_locker)
				{
					while (_tasks.Count > 0)
					{
						byte[] work = _tasks.Dequeue(); // 有任务时，出列任务
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.AddRange(work.ToList()); 
						if (IsFullCommand(works)) {
							byte[] bf = works.ToArray();
							//ControlConnection.UpdateTextBox("SUCESS:" + bf.Length);
							works.Clear();
							SaveData(bf);  // 处理并保存数据
							break;//是否完整命令
						} 
					}
				}

				try
				{

					if(_tasks.Count==0) {
						_wh.WaitOne();   // 没有任务了，等待信号
					}

				}
				catch (Exception ex)
				{
					ControlConnection.UpdateTextBox(string.Format("Work {0}", ex.Message));
				}

			}
			Running = false;
		}

		/// <summary>插入任务</summary>
		public void EnqueueTask(byte[] task)
		{
			try
			{
				lock (_locker)
					_tasks.Enqueue(task);  // 向队列中插入任务 

				if (_wh != null) _wh.Set();  // 给工作线程发信号
			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(string.Format("EnqueueTask {0}", ex.Message));
			}

		}

		/// <summary>结束释放</summary>
		public void Dispose()
		{
			DisConnect();
		}


		/// <summary>处理保存</summary>
		void SaveData(byte[] buffer)
		{ 

			try
			{
				PROTOCOL p1 = Protocal;
				string[][] PAR_V = null;
                switch (p1)
                {
                    case PROTOCOL.V1:
						PAR_V = PAR_V1;
						break; 
                    default:
                        break;
                }

				List<SenV> vs = new List<SenV>();
				byte[] bs = buffer;
				for (int i = 0; i < PAR_V.Length; i++)
				{
					var ling = PAR_V[i];
					int index = Convert.ToInt32(ling[0]);
					byte b1 = bs[index-1];
					byte b2 = bs[index];
					byte[] bsv = new byte[] { b2, b1 };
					ushort value = (ushort)BitConverter.ToInt16(new byte[] { b2, b1 }, 0);
					vs.Add(new SenV() { PAR = ling, Value = value, Bytes = bsv });
				}
				LastSenV = vs;
				this.ControlConnection.SaveToDb(LastSenV);
				RefreshUI(LastSenV);

			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(string.Format("插入数据出错 {0}", ex.Message));
			}


			

		}




		private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{

			try
			{
				int len = serialPortMain.BytesToRead;
				Byte[] buf = new byte[len];
				int length = serialPortMain.Read(buf, 0, len);
				StringBuilder sx = new StringBuilder();
				foreach (var i in buf)
				{
					string str = i.ToString("X");
					while (str.Length < 2) str = "0" + str;
					sx.Append(string.Format("{0} ", str));
				}

				this.EnqueueTask(buf);
				ControlConnection.UpdateTextBoxCommand("收←◆" + sx);

			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(ex.Message);
			}


		}

		public void Connect()
		{
			//通讯接口：串口（TTL，3.3V电平），波特率：9600，数据位: 8，停止位: 1，校验位: 无。
			//传输模式：模组主动上传数据帧，间隔1秒。
			serialPortMain = new SerialPort();
			serialPortMain.BaudRate = 9600;//波特率
			serialPortMain.PortName = ControlConnection.Port;
			serialPortMain.Parity = Parity.None;//校验法：无
			serialPortMain.DataBits = 8;//数据位：8
			serialPortMain.StopBits = StopBits.One;//停止位：1
			try
			{
				serialPortMain.DataReceived += SerialPort1_DataReceived;
				serialPortMain.ErrorReceived += SerialPort1_ErrorReceived;
				serialPortMain.DtrEnable = true;//设置DTR为高电平
				serialPortMain.RtsEnable = false;

				serialPortMain.Encoding = Encoding.ASCII;
				serialPortMain.DiscardNull = false;
				serialPortMain.ReceivedBytesThreshold = 1;
				serialPortMain.ReadTimeout = 0x7fff_ffff;
				serialPortMain.ReadBufferSize = 0x1000;
				serialPortMain.WriteBufferSize = 0x800;
				serialPortMain.WriteTimeout = 0x7fff_ffff;

				this.StartThread();
				serialPortMain.Open();//打开串口


				serialPortMain.DiscardInBuffer();
				serialPortMain.DiscardOutBuffer();

				string str = (string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12} ",
						serialPortMain.PortName, serialPortMain.BaudRate,
						serialPortMain.DataBits, serialPortMain.Parity,
						serialPortMain.StopBits, serialPortMain.RtsEnable,
						serialPortMain.Encoding, serialPortMain.DiscardNull,
						serialPortMain.ReceivedBytesThreshold, serialPortMain.ReadTimeout,
						serialPortMain.ReadBufferSize, serialPortMain.WriteBufferSize,
						serialPortMain.WriteTimeout));
			}
			catch (Exception ex)
			{
				//打开串口出错，显示错误信息
				MessageBox.Show(ex.Message);
			}

		}
		 
		private void SerialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			ControlConnection.UpdateTextBox(e.EventType.ToString());
		}

		public void DisConnect()
		{
			try
			{
				if (serialPortMain != null) {
					serialPortMain.DataReceived -= SerialPort1_DataReceived;
					serialPortMain.Close();
					ControlConnection.UpdateTextBox("已断开连接" + serialPortMain.PortName);
				}
			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(ex.Message);
			}

			try
			{
				if (_workerTimer != null)
				{
					_workerTimer.Enabled = false;
					_workerTimer.Tick -= _workerTimer_Tick;
				}
				Cancel = true;
				EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
				if (_wh != null) _wh.Set();  // 给工作线程发信号
				EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
				if (_wh != null) _wh.Set();  // 给工作线程发信号
				try
				{
					if (_worker != null) _worker.Interrupt();
				}
				catch (Exception)
				{

				}
				_worker = null;
				if (_wh != null) _wh.Close();            // 释放资源
				if (_wh != null) _wh.Dispose();
				_wh = null;
			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(string.Format("Dispose {0}", ex.Message));
			}
			finally
			{

			}

		}
		 

		private void SendCommand(string hexString )
		{
			if (!this.serialPortMain.IsOpen)
			{
				ControlConnection.UpdateTextBox("请连接串口");
				return;
			}
			try
			{ 
				byte[] bs = ConvertHexStringToBytes(hexString); 
				this.serialPortMain.Write(bs, 0, bs.Length);
				//ControlConnection.UpdateText = "查询指令已发送!";
				this.ControlConnection.UpdateTextBoxCommand(string.Format("发→◇{0}", hexString));
			}
			catch (Exception ex)
			{
				this.ControlConnection.UpdateTextBox(ex.Message);
			}
		}

		/// <summary>
		/// 16进制原码字符串转字节数组
		/// </summary>
		/// <param name="hexString">"AABBCC"或"AA BB CC"格式的字符串</param>
		/// <returns></returns>
		public static byte[] ConvertHexStringToBytes(string hexString)
		{
			hexString = hexString.Replace(" ", "");
			if (hexString.Length % 2 != 0)
			{
				throw new ArgumentException("参数长度不正确,必须是偶数位。");
			}
			byte[] returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
			{
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}

			return returnBytes;
		} 


		byte[] ToBytes(string str_in)
		{
			byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(str_in);
			return byteArray;
		}


	}

	public class SenV {
		public SenV() {
			DateTime = DateTime.Now;
		}
		public DateTime DateTime { get; set; }
		public bool Overdued { get { return (DateTime.Now - DateTime) > new TimeSpan(0,0,2); } }
		public string[] PAR { get; set; }
		public int Value { get; set; }
		public byte[] Bytes { get; set; }
		public string ValueActual
		{
			get
			{
				if (FormMain.Instance.Protocal != PROTOCOL.V3)
				{
					//温度和湿度的结果扩大了10倍，且温度数据在实际测量结果上增加了500，如上面温度数据0x02FA，十进制数据为762，则实际温度为(762-500)/10=26.2℃
					if (Name == "温度")
					{
						return ((double)(Value - 500) / 10).ToString("f1");
					}
					if (Name == "湿度")
					{
						return ((double)Value / 10).ToString("f1");
					}
				}
				else {
					//温度和湿度的数据，高字节为整数部分，低字节为1位小数部分（湿度小数位忽略，始终为0）
					if (Name == "温度")
					{
						return (((double)(int)Bytes[1])+ (double)(int)Bytes[0]/100).ToString("f1");
					}
					if (Name == "湿度")
					{
						return (((double)(int)Bytes[1])).ToString("f0");
					}
				}
				return Value.ToString();
			}
		}
		public string Name { get { return PAR != null && PAR.Length > 1 ? PAR[1] : "---"; } }
		public string Unit { get { return PAR != null && PAR.Length > 2 ? PAR[2] : "---"; } }
	}

	public enum PROTOCOL
	{
		No = -1, V1 = 0, V2 = 1, V3 = 2
	}

}
 
