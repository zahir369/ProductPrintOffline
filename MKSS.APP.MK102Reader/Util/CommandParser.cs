using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel; 
using System.Text;
using MKSS.APP.MK102Reader;

namespace MKSS.APP.MK102Reader
{
	public class CommandParser
	{

		// 任务队列
		static Queue<List<byte>> _tasks = new Queue<List<byte>>();
		// 为保证线程安全，使用一个锁来保护_task的访问
		readonly static object _locker = new object();

		// 通过 _wh 给工作线程发信号
		static EventWaitHandle _wh = null;
		static Thread _worker;

		public static Dictionary<int, EggFloor> Floors = new Dictionary<int, EggFloor>();
		public static bool Running { get; private set; }
		public static FormSensor FormSensor { get; private set; }
		public static void Start(FormSensor form)
		{
			FormSensor = form;
			EggLayed.Start = DateTime.Now;
            for (int i = 1; i <= 8; i++)
            {
				if (!Floors.ContainsKey(i))
				{
					Floors.Add(i, new EggFloor() { FloorNo = i });
				}
				Floors[i].Clear();

			}
			Cancel = false;
			_wh = new AutoResetEvent(false);

			// 任务开始，启动工作线程
			_worker = new Thread(Work);
			_worker.Start();
		}


		static bool Cancel = false;
		/// <summary>执行工作</summary>
		static void Work()
		{
			Running = true;
	 
			
			List<byte> works = new List<byte>();

			while (true)
			{
				int resLen = works.Count >2 ? (int)works[2] + 5 : 3;
				bool match = works.Count > 2 ? (works.Count >= (int)works[2] + 5) :false;
				if (Cancel) break;
				lock (_locker)
				{
					while (works.Count < resLen && _tasks.Count > 0 && !match)
					{
						List<byte> work = _tasks.Dequeue(); // 有任务时，出列任务
						
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.AddRange(work);
						if (works.Count >= 6) { 
							match = works.Count >= (int)works[2] + 5;
						}

					}
				}

				try
				{
					if (works.Count>2 && works.Count >= (int)works[2] + 5)
					{
						SaveData(works);  // 任务不为null时，处理并保存数据
						works.Clear();
					}
					else {
						_wh.WaitOne();   // 没有任务了，等待信号
					}

				}
				catch (Exception ex)
				{
					FormSensor.Log(string.Format("Work {0}", ex.Message));
					FormSensor.Log(string.Format("{0}", ex.StackTrace));
				}

			}
			Running = false;
		}

		/// <summary>插入任务</summary>
		public static void EnqueueTask(List<byte> task)
		{
			try
			{
				lock (_locker)
					_tasks.Enqueue(task);  // 向队列中插入任务 

				if (task != null) {
					StringBuilder strBuider = new StringBuilder();
					for (int index = 0; index < task.Count; index++)
					{
						strBuider.Append(((int)task[index]).ToString("X2") + " ");
					}
					FormSensor.Log(string.Format("APP: {0}", strBuider));
				}



				if (_wh != null) _wh.Set();  // 给工作线程发信号
			}
			catch (Exception ex)
			{
				FormSensor.Log(string.Format("EnqueueTask {0}", ex.Message));
				FormSensor.Log(string.Format("{0}", ex.StackTrace));
			}

		}

		public static void ClearQueQue() {
			_tasks.Clear();
		}

		/// <summary>结束释放</summary>
		public static void Dispose()
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
				FormSensor.Log(string.Format("Dispose {0}", ex.Message));
			}
			finally {

			}

		}


		/// <summary>处理保存</summary>
		static void SaveData(List<byte> buffer)
		{
			try
			{
				StringBuilder strBuider = new StringBuilder();
				for (int index = 0; index < buffer.Count; index++)
				{
					strBuider.Append(((int)buffer[index]).ToString("X2")+" ");
				}

				if (buffer.Count >= 6 && buffer[buffer.Count - 1] > 0 && buffer[buffer.Count - 2] > 0)
				{
					 
					if (buffer[0] != 0x01)
					{
						FormSensor.Log("ERRRRRR:" + strBuider);
						return;
					}

					CommandType type = (CommandType)(int)buffer[1];
					CommandAbs data = CommandAbs.Of(type);
					if (data == null)
					{
						FormSensor.Log("指令类别不支持:" + strBuider);
						return;
					}
					int dataLength = (int)buffer[2];
					if (buffer.Count < dataLength+5)
					{
						FormSensor.Log("数据长度不足:" + strBuider);
						return;
					}

					FormSensor.Log("RX "+ data.Command + ":" + strBuider);
					data.Response = buffer.ToArray();

					if (data.Command == CommandType.Status)
					{
						/**
							1.读取1号控制器的1号探测器状态：
							发→◇01 03 00 01 00 08 15 CC 
							收←◆01 03 10 00 0A 00 01 00 01 00 01 00 01 00 01 00 01 00 01 D8 B3
						 ***/
						SensorStatus[] ss = data.ToSensorStatus();
						StringBuilder strBuiderxx = new StringBuilder();
						for (int index = 0; index < ss.Length; index++)
						{
							strBuiderxx.Append(string.Format("{0}号探测器：{1}；", index+1, ss[index]));
							CommandAbs.Sensors.First(w => (int)w.Enum == index + 1).Status = ss[index];
							CommandAbs.Sensors.First(w => (int)w.Enum == index + 1).DateTime = DateTime.Now;
						}
						FormSensor.Log(strBuiderxx.ToString());
						FormSensor.BeginInvoke((EventHandler)(delegate {
							FormSensor.RefreshBtnStatus();
						}));
						return;
					}

					if (data.Command == CommandType.NongDu)
					{
						/**
							2.读取1号控制器的1号探测器数值：
							发→◇01 04 00 01 00 08 A0 0C 
							收←◆01 04 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 2C
							注：bit15表示指数的符号位；bit14~13表示指数位数；bit12~0表示数值。
							如：0xA019，表示25 * 10^（-1）= 2.5
						 ***/
						double[] ss = data.ToSensorValue();
						StringBuilder strBuiderxx = new StringBuilder();
						for (int index = 0; index < ss.Length; index++)
						{
							strBuiderxx.Append(string.Format("{0}号探测器：{1}；", index + 1, ss[index]));
							CommandAbs.Sensors.First(w => (int)w.Enum == index + 1).NongDu = ss[index].ToString();
							CommandAbs.Sensors.First(w => (int)w.Enum == index + 1).DateTime = DateTime.Now;
						}
						FormSensor.Log(strBuiderxx.ToString());
						FormSensor.BeginInvoke((EventHandler)(delegate {
							FormSensor.RefreshBtnStatus();
						}));
						return;
					}


					if (data.Command == CommandType.ControllerStatus)
					{
						/**
							3.读取1号控制器的状态
							发→◇01 02 00 01 00 04 28 09 □
							收←◆01 02 01 00 A1 88
							bit0就是代表备电，bit1代表主电
						 ***/
						List<ControllerStatus> ss = data.ToControllerStatus();
						StringBuilder strBuiderxx = new StringBuilder();
						for (int index = 0; index < ss.Count; index++)
						{
							strBuiderxx.Append(string.Format("{0}", ss[index]));
						}
						FormSensor.Log(strBuiderxx.ToString()); 
						FormSensor.BeginInvoke((EventHandler)(delegate {
							FormSensor.RefreshBtnStatus();
						}));
						return;
					}

					//FormSensor.BeginInvoke((EventHandler)(delegate {
					//	FormSensor.ConfigStateLable.ForeColor = Color.Green;
					//	FormSensor.ConfigStateLableText = "成功";
					//	FormSensor.RecordTimeShowBoxText = "当前记录数读取成功";
					//	FormSensor.RecordTimeShowBoxText = "报警器状态：寿命失效";
					//}));

					//FormSensor.BeginInvoke((EventHandler)(delegate {
					//	if (FormSensor.Commands.ReponsedCount <= FormSensor.toolStripProgressBar1.Maximum) FormSensor.toolStripProgressBar1.Value = FormSensor.Commands.ReponsedCount;
					//	if (FormSensor.toolStripProgressBar1.Value % 10 == 0) FormSensor.dataGridView1.Refresh();
					//	RedarNode(FormSensor.treeView1.Nodes[0]);
					//	foreach (var item in FormSensor.treeView1.Nodes[0].Nodes)
					//	{
					//		RedarNode(item as TreeNode);
					//	}
					//}));
					FormSensor.BeginInvoke((EventHandler)(delegate {
						FormSensor.RefreshBtnStatus();
					}));

				}
				else {
					FormSensor.Log("EE:" + strBuider);
				}

			}
			catch (Exception ex)
			{
				FormSensor.Log(string.Format("插入数据出错 {0}", ex.Message));
				FormSensor.Log(string.Format("{0}", ex.StackTrace));
			}
		}


		 

	}

	public class EggFloor {
		public int eggs_start = int.MinValue;
		public int eggs_current = 0;
		public int EggCountCurrent { get { return (eggs_current - eggs_start); } }
		public Dictionary<int, EggLayed> Layers = new Dictionary<int, EggLayed>();
		public int FloorNo { get; set; }
		public void Clear() {
			eggs_start = int.MinValue;
			eggs_current = 0;
			Layers.Clear();
		}
	}
	public class EggLayed
	{
		public EggLayed(EggFloor g) {
			Parent= g;
		}
		public EggFloor Parent { get; set; }
		public int Count { get; set; }
		public int Xh { get; set; }
		public int Floor { get { return Parent.FloorNo; } }
		public DateTime Dt { get; set; } 
		public int CaseNo
        {
			get {
				return Floor * 100 + (int)((Dt - Start - IntervalStart).TotalSeconds / Interval.TotalSeconds)+1;
			}
		}
		public static DateTime Start { get; set; }
		public static TimeSpan IntervalStart { get; set; } = TimeSpan.FromSeconds(67);
		public static TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(13.45);
	}

	public class CaseLayed
	{
		public int Xh { get; set; }
		public int Count { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
	}

}
 
