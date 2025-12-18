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
using SqlSugar;
using MKSS.Model;

namespace MKSS.APP.WinNBTester
{
	public class NBTesterSaver
	{

		// 任务队列
		Queue<string> _tasks = new Queue<string>();
		// 为保证线程安全，使用一个锁来保护_task的访问
		readonly object _locker = new object();
		// 通过 _wh 给工作线程发信号
		EventWaitHandle _wh = null;
		Thread _worker;
		UIConnection ControlConnection;
		public bool Running { get; private set; }

		SerialPort serialPortMain = null;
		public string IMEI { get; set; }
		public string IMSI { get; set; }
		public string ICCID { get; set; }
		public int Siganel { get; set; }
		public NBTesterSaver(UIConnection form) {
			ControlConnection = form;
		}
		public void Start()
		{
			Cancel = false;
			_wh = new AutoResetEvent(false);
			//设置可以同时处于活动状态的线程池的请求数目。 
			bool pool = ThreadPool.SetMaxThreads(9, 9);//先执行9个，当有空闲线程时再执行下一个
			_worker = new Thread(Work);// 任务开始，启动工作线程
			_worker.Start();
		}

		bool IsFullCommand(StringBuilder s) {
			if (s.ToString().Trim().EndsWith("OK")) return true;
			if (s.ToString().Trim().EndsWith("ERROR")) return true;
			if (s.ToString().Trim().Replace(" ", "").EndsWith("+CFUN:1")) return true;
			if (s.ToString().Trim().Replace(" ", "").EndsWith("+CPIN:READY")) return true;
			if (s.ToString().Trim().Replace(" ", "").EndsWith("+QLWEVTIND:0")) return true;
			if (s.ToString().Trim().Replace(" ", "").EndsWith("+QLWEVTIND:1")) return true;
			if (s.ToString().Trim().Replace(" ", "").EndsWith("+QLWEVTIND:3")) return true;
			return false;
		}

		bool Cancel = false;
		/// <summary>执行工作</summary>
		void Work()
		{
			Running = true;
			StringBuilder works = new StringBuilder();

			while (true)
			{

				if (Cancel) break;
				lock (_locker)
				{
					while (_tasks.Count > 0)
					{
						string work = _tasks.Dequeue(); // 有任务时，出列任务
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.Append(work);
						if (IsFullCommand(works)) break;//是否完整命令
					}
				}

				try
				{

					if (IsFullCommand(works))
					{
						SaveData(works);  // 任务不为null时，处理并保存数据
						works.Clear();
					}

					if(_tasks.Count==0) {
						_wh.WaitOne();   // 没有任务了，等待信号
					}

				}
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Work {0}", ex.Message));
				}

			}
			Running = false;
		}

		/// <summary>插入任务</summary>
		public void EnqueueTask(string task)
		{
			try
			{
				lock (_locker)
					_tasks.Enqueue(task);  // 向队列中插入任务 

				if (_wh != null) _wh.Set();  // 给工作线程发信号
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("EnqueueTask {0}", ex.Message));
			}

		}

		/// <summary>结束释放</summary>
		public void Dispose()
		{
			try
			{
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
				Console.WriteLine(string.Format("Dispose {0}", ex.Message));
			}
			finally {

			}

		}


		public static string[][] PAR = new string[][] {

			new string[]{ "AT+QSCLK=0", "OK", "","" },//设置睡眠模式 bc265,,new string[]{ "AT+QCFG=\"wakeupRXD\",0", "OK", "","" },//设置睡眠模式  bc260y
            new string[]{ "AT+CGATT?", "+CGATT: 1OK", "","" },//查询附着网络
            new string[]{ "AT+QSCLK=0", "OK", "","" },//禁用睡眠模式
            
            new string[]{ "AT+NNMI=1", "OK", "","" },//设置直吐模式
            new string[]{ "AT+CGPADDR?", "OK", "","" },//查询模块是否成功注网
            new string[]{ "AT+NMGS=7,010548454c4c4f", "OK", "","" },//通讯测试
            new string[]{ "AT+NCFG=0,86400", "OK", "","" },//设置端口
            new string[]{ "AT+NCDPOPEN=\"221.229.214.202\"", "+QLWEVTIND: 3", "","" },//ctwing连接成功 0 3 ，1 是被服务器拒绝没成功或没在ctwing注册。+QLWEVTIND:0 注册LWM2M 成功；+QLWEVTIND:3 平台订阅Object19 成功
            new string[]{ "AT+CGSN=1", "OK", "","" },//读取设备IMEI
            new string[]{ "AT+NRB", "OK", "","" },//复位
            new string[]{ "AT+CSQ", "OK", "","" },//查信号
            new string[]{ "AT+QCCID", "OK", "","" },//读取QCCID卡号 
            new string[]{ "AT+CIMI", "OK", "","" },//读取IMSI卡号 
            
            new string[]{ "RDY", "CFUN: 1", "已上电","上电异常" },//上电
            new string[]{ "+CPIN", "READY", "初始化成功","初始化失败" },//初始化
            

        };


		public enum CommandType
		{
			设置睡眠模式 = 0,
			查询附着网络 = 1,
			禁用串口唤醒睡眠 = 2,//禁用串口唤醒睡眠
			设置直吐模式 = 3,
			查询模块是否成功注网 = 4,
			通讯测试 = 5,
			设置端口 = 6,
			设置Ctwing地址并发起连接 = 7,//设置Ctwing LWM2M 接入地址并发起连接 设置Ctwing地址并发起连接
			读取设备IMEI = 8,
			复位 = 9,
			查信号 = 10,
			读取卡号ICCID = 11,
			读取IMSI = 12,

			上电 = 13,
			初始化 = 14, 

		}
		public enum CommandResult
		{
			成功 = 1,
			等待 = 3,
			失败 = 2
		}

		string[][] StatusTypePAR = new string[][] {

			new string[]{ "已上电", "RDY\n\n+CFUN: 1", "" },
			new string[]{ "已初始化", "+CPIN: READY", "" },

		};


		CommandType[] CommandAll = new CommandType[]{
					CommandType.上电,
					CommandType.初始化,
					CommandType.禁用串口唤醒睡眠,
					CommandType.查询附着网络,
					CommandType.设置直吐模式,
					CommandType.查询模块是否成功注网,
					CommandType.设置端口,
					CommandType.设置Ctwing地址并发起连接,
					CommandType.查信号,
					CommandType.读取设备IMEI,
					CommandType.读取卡号ICCID,
					CommandType.读取IMSI
				};


		/// <summary>处理保存</summary>
		void SaveData(StringBuilder buffer)
		{

			if (FormMain.VersionOnline) {

				if (FormMain.Instance.SelectProduct == null || FormMain.Instance.SelectNBProduct == null)
				{
					ControlConnection.UpdateText = string.Format("必须先选择产品类别");
					return;
				}
			}

			CommandType Main = CommandType.上电;
			CommandResult MainResult = CommandResult.等待;

			try
			{
				string result = buffer.ToString().Trim();
				if (string.IsNullOrEmpty(result)) return;
				//result = result.Replace(System.Environment.NewLine, "");
				bool find = false;
				//判断命令
				for (int i = 0; i < PAR.Length; i++)
				{
					var ling = PAR[i];
					if (result.StartsWith(ling[0]))
					{
						Main = (CommandType)i;
						find = true;
					}
				}


				if (!find)
				{
					//判断命令
					for (int i = 0; i < PAR.Length; i++)
					{
						var ling = PAR[i];
						string lingstr = ling[0];
						lingstr = lingstr.TrimStart("AT+".ToCharArray());
						if (lingstr.IndexOf("=") > 0) {
							lingstr = lingstr.Split("=".ToCharArray())[0].Trim();
						}
						if (result.IndexOf(lingstr)>=0)
						{
							Main = (CommandType)i;
							find = true;
						}

						if (result.IndexOf("+QLWEVTIND") >= 0)
						{
							Main = CommandType.设置Ctwing地址并发起连接;
							find = true;
						}

					}
				}
				 

				if (result.Equals(PAR[(int)CommandType.设置Ctwing地址并发起连接][1]))
				{
					Main = CommandType.设置Ctwing地址并发起连接;
					find = true;
				}


				if (!find)
				{
					ControlConnection.UpdateText = string.Format("非预期结果：" + result);
					SendCommand(Main);//重试
				}
				else {
					//ControlConnection.UpdateText = string.Format("收到指令：" + Main);
				}

				string comm_str = PAR[(int)Main][0];

				if (!result.StartsWith(comm_str) && (Main!= CommandType.设置Ctwing地址并发起连接))
				{
					ControlConnection.UpdateText = string.Format("【" + Main + "】非预期结果：" + result);
					SendCommand(Main);//重试
					return;
				}

				string sucess_str = PAR[(int)Main][1];
				string sucess_desc = PAR[(int)Main][2];
				string fali_desc = PAR[(int)Main][3];



				int CommandAllCurrent = Array.IndexOf(CommandAll, Main);
				Label IC = ControlConnection.ICS[Array.IndexOf(CommandAll, Main)];

				if (result.ToString().EndsWith(sucess_str))
				{
					MainResult = CommandResult.成功;
					if (string.IsNullOrEmpty(sucess_desc)) sucess_desc = "成功！";
					ControlConnection.UpdateText = string.Format("{0}指令：{1}", Main, sucess_desc);
				}
				else
				{
					if (Main == CommandType.设置Ctwing地址并发起连接)
					{

						if (result.ToString().EndsWith("+QLWEVTIND: 0"))
						{
							ControlConnection.UpdateText = string.Format("{0}指令：注册LWM2M 成功，等待平台订阅Object19 成功", Main);
							return;
						}
						return;
					}
					else {

						MainResult = CommandResult.失败;
						if (string.IsNullOrEmpty(fali_desc)) fali_desc = "失败！";
						ControlConnection.UpdateText = string.Format("{0}指令：{1}", Main, fali_desc);

					}

				}

				switch (Main)
				{
					case CommandType.上电:
						ControlConnection.ResetIC();
						break;
					case CommandType.初始化:
						if (MainResult == CommandResult.成功)
						{
							SendCommand(CommandType.禁用串口唤醒睡眠);
						}
						else
						{
							 
						}
						break;
					case CommandType.禁用串口唤醒睡眠:
						if (MainResult == CommandResult.成功)
						{
							SendCommand(CommandType.查询附着网络);
						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.查询附着网络:
						if (MainResult == CommandResult.成功)
						{
							SendCommand(CommandType.设置直吐模式);
						}
						else
						{
							Thread.Sleep(5001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.设置直吐模式:
						if (MainResult == CommandResult.成功)
						{
							SendCommand(CommandType.查询模块是否成功注网);
						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.查询模块是否成功注网:
						if (MainResult == CommandResult.成功)
							SendCommand(CommandType.设置端口);
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.设置端口:
						if (MainResult == CommandResult.成功)
							SendCommand(CommandType.查信号);
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.设置Ctwing地址并发起连接:
						if (MainResult == CommandResult.成功)
						{
							ControlConnection.TestCTWING(true);
						}
						else
						{
							ControlConnection.TestCTWING(false);
							//失败时等待，不能重复发送指令 SendCommand(Main);
						}
						break;
					case CommandType.查信号:
						if (MainResult == CommandResult.成功)
						{
							result = result.Trim();
							result = result.Trim(comm_str.ToCharArray());
							result = result.Trim();
							result = result.Trim(sucess_str.ToCharArray());
							result = result.Trim();
							result = result.Trim("+CSQ:".ToCharArray());
							Siganel = int.Parse(result.Trim().Split(',')[0].Trim());
							SendCommand(CommandType.读取设备IMEI);
						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.读取设备IMEI:
						if (MainResult == CommandResult.成功) {
							result = result.Trim();
							result = result.Trim(comm_str.ToCharArray());
							result = result.Trim();
							result = result.Trim(sucess_str.ToCharArray());
							result = result.Trim();
							result = result.Trim("+CGSN:".ToCharArray());
							IMEI = result.Trim();
							SendCommand(CommandType.读取IMSI);
						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.读取IMSI:
						if (MainResult == CommandResult.成功)
						{
							result = result.Trim();
							result = result.Trim(comm_str.ToCharArray());
							result = result.Trim();
							result = result.Trim(sucess_str.ToCharArray());
							result = result.Trim(); 
							IMSI = result.Trim();
							SendCommand(CommandType.读取卡号ICCID);
						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					case CommandType.读取卡号ICCID:
						if (MainResult == CommandResult.成功) {

							result = result.Trim();
							result = result.Trim(comm_str.ToCharArray());
							result = result.Trim();
							result = result.Trim(sucess_str.ToCharArray());
							result = result.Trim();
							result = result.Trim("+QCCID:".ToCharArray());
							ICCID = result.Trim();

							Device dev = null;
							bool sucess = SaveToDb(ref dev);
							ControlConnection.SaveToDB(sucess);

							if (sucess && FormMain.VersionOnline) {
								//联机版需要保存到 CTWING
								sucess = SaveToCTWING(dev);
								sucess = sucess && !string.IsNullOrEmpty(dev.F_DeviceId);
								ControlConnection.SaveToCTWING(sucess);
								SendCommand(CommandType.设置Ctwing地址并发起连接);
							}

						}
						else
						{
							Thread.Sleep(1001);
							SendCommand(Main);//失败时重试
						}
						break;
					default:
						break;
				}

				ControlConnection.Invoke((Action)delegate
				{
					IC.Text = MainResult == CommandResult.成功 ? "√" : "?";
					IC.ForeColor = MainResult == CommandResult.成功 ? Color.Green : Color.Red;
					ControlConnection.ToolTip.SetToolTip(IC, string.Format("{0}", Main));

					ControlConnection.LabelIMEI.Text = IMEI;
					ControlConnection.LabelIMSI.Text = IMSI;
					ControlConnection.LabelICCID.Text = ICCID;
					Application.DoEvents();
				});

			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(string.Format("插入数据出错 {0}", ex.Message));
			}


			

		}

		bool SaveToDb(ref Device dev) {
			return MysqlDB.CreateDevice(this.IMEI, this.IMSI, this.ICCID,ref dev);
		}
		bool SaveToCTWING(Device dev)
		{
			return FormMain.Instance.IotYun.RegisterNetInner(dev);
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
				string result = System.Text.Encoding.ASCII.GetString(buf);

				result = result.Trim();
				result = result.Replace(System.Environment.NewLine, "");

				this.EnqueueTask(result);
				ControlConnection.UpdateTextBoxCommand("收←◆" + result);

			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(ex.Message);
			}


		}

		public void BtnConnectCommand()
		{

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

				this.Start();
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

		private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{

		}

		private void SerialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			ControlConnection.UpdateTextBox(e.EventType.ToString());
		}

		public void BtnDisConnectCommand()
		{
			try
			{
				serialPortMain.DataReceived -= SerialPort1_DataReceived;
				serialPortMain.Close();
				ControlConnection.UpdateTextBox("已断开连接" + serialPortMain.PortName);
			}
			catch (Exception ex)
			{
				ControlConnection.UpdateTextBox(ex.Message);
			}
		}
		 

		private void SendCommand(CommandType sender)
		{
			if (!this.serialPortMain.IsOpen)
			{
				ControlConnection.UpdateTextBox("请连接串口");
				return;
			}
			try
			{

				Label IC = ControlConnection.ICS[Array.IndexOf(CommandAll, sender)];
				ControlConnection.Invoke((Action)delegate
				{
					IC.Text =  "?" ;
					IC.ForeColor = Color.Red;
					ControlConnection.ToolTip.SetToolTip(IC, string.Format("正在发送{0}......", sender));
					Application.DoEvents();
				});

				string command = PAR[(int)sender][0];
				byte[] bs = System.Text.Encoding.ASCII.GetBytes(command+System.Environment.NewLine); 
				this.serialPortMain.Write(bs, 0, bs.Length);
				ControlConnection.UpdateText = sender + "已发送!";
				this.ControlConnection.UpdateTextBoxCommand(string.Format("发→◇{0}", command));
			}
			catch (Exception ex)
			{
				this.ControlConnection.UpdateTextBox(ex.Message);
			}
		}


		byte[] ToBytes(string str_in)
		{
			byte[] byteArray = System.Text.Encoding.ASCII.GetBytes(str_in);
			return byteArray;
		}


	}



}
 
