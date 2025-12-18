using System.Collections.Generic;
using System;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel;
using EggCounter;
using System.Text;

namespace MKSS.Service.UIBiaoDing
{
	public class EggSaver
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
		public static void Start( )
		{
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
			//0103200000000F000000000000000700000000000000130000000000000000000000009F95
			//01032000000000000000000000000000000000000000000000000000000000000000ABD3C5
			//0103200000010700000000000000030000000000000002000000000000000000000000E75E
			int resLen = "0103200000000F000000000000000700000000000000130000000000000000000000009F95".Length / 2;
			List<byte> works = new List<byte>();

			while (true)
			{

				if (Cancel) break;
				lock (_locker)
				{
					while (works.Count < resLen && _tasks.Count > 0)
					{
						List<byte> work = _tasks.Dequeue(); // 有任务时，出列任务
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.AddRange(work);
					}
				}

				try
				{
					if (works.Count >= resLen)
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
					FormMain.Instance.Log(string.Format("Work {0}", ex.Message));
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

				if (_wh != null) _wh.Set();  // 给工作线程发信号
			}
			catch (Exception ex)
			{
				FormMain.Instance.Log(string.Format("EnqueueTask {0}", ex.Message));
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
				FormMain.Instance.Log(string.Format("Dispose {0}", ex.Message));
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
					strBuider.Append(((int)buffer[index]).ToString("X2"));
				}

				if (buffer.Count >= 4 && buffer[buffer.Count - 1] > 0 && buffer[buffer.Count - 1] > 0)
				{
					
					FormMain.Instance.Log("##:" + strBuider);

                    //0103200000010700000000000000030000000000000002000000000000000000000000E75E
                    for (int f = 1; f <= 8; f++)
                    {
						int i = f * 4 - 1;
						EggFloor floor = Floors[f];
				 
						if (i + 3 > buffer.Count - 1) break;
						ushort _EggCount = BitConverter.ToUInt16(new byte[4] { buffer[i + 3], buffer[i + 2], buffer[i + 1], buffer[i] }, 0);
						if (floor.eggs_start == int.MinValue)
						{
							floor.eggs_start = _EggCount;
						}

						floor.eggs_current = _EggCount;
						if (!floor.Layers.ContainsKey(_EggCount) && _EggCount != floor.eggs_start)
						{
							EggLayed e = new EggLayed(floor)
							{
								Dt = DateTime.Now,
								Xh = _EggCount, 
								Count = floor.eggs_current - floor.eggs_start
							};
							floor.Layers.Add(_EggCount, e);
						}

					}
					//byte[] xx = new byte[2];
					//Array.Copy(buffer.ToArray(), buffer.Count - 4, xx, 0, 2);
					//byte[] xx1 = new byte[] { xx[1], xx[0] };
					//ushort _EggCount = BitConverter.ToUInt16(xx1, 0);
					


                }
                else {
					FormMain.Instance.Log("EE:" + strBuider);
				}

			}
			catch (Exception ex)
			{
				FormMain.Instance.Log(string.Format("插入数据出错 {0}", ex.Message));
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
 
