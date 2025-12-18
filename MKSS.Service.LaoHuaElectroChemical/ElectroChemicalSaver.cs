using System.Collections.Generic;
using System;
using MKSS.Services;
using MKSS.Model;
using MKSS.Util.Log;
using System.Threading;
using SqlSugar;

namespace MKSS.Service.LaoHuaElectroChemical
{
    public class ElectroChemicalSaver
	{
		// 任务队列
		static Queue<string> _tasks = new Queue<string>();

		// 为保证线程安全，使用一个锁来保护_task的访问 
		readonly static object _locker = new object();

		// 通过 _wh 给工作线程发信号
		static EventWaitHandle _wh = new AutoResetEvent(false);
		static SensorServices _SensorServices = new SensorServices();
		static Thread _worker;

		public static SensorServices SensorServices { get { return _SensorServices; } }

		public static void Start( )
		{
			// 任务开始，启动工作线程
			_worker = new Thread(Work);
			_worker.Start();
		}
		 
		static Batch _Batch = null;
		/// <summary>
		///  当前数据库名称
		/// </summary>
		public static Batch Batch
		{
			get {
				return _Batch;
			}
			set
			{
				if (value != null) {
					 
				}
				_Batch = value;
			}
		}

		static bool Cancel = false;
		/// <summary>执行工作</summary>
		static void Work()
		{
			while (true)
			{

				if (Cancel) break;
				List<string> works = new List<string>(); 
				lock (_locker)
				{
					while (works.Count <= 6 && _tasks.Count > 0)
					{
						string work = _tasks.Dequeue(); // 有任务时，出列任务
						if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
							break;
						works.Add(work);
					}
				}

                try
                {
					if (works.Count > 0)
						SaveData(works);  // 任务不为null时，处理并保存数据
					else
						_wh.WaitOne();   // 没有任务了，等待信号
				}
                catch (Exception)
                {
                    break;
                }
				
			}
		}

		/// <summary>插入任务</summary>
		public static void EnqueueTask(string task)
		{
			lock (_locker)
				_tasks.Enqueue(task);  // 向队列中插入任务 

			_wh.Set();  // 给工作线程发信号
		}

		/// <summary>结束释放</summary>
		public static void Dispose()
		{
            try
            {
				Cancel = true;
				EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
				_wh.Set();  // 给工作线程发信号
				EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
				_wh.Set();  // 给工作线程发信号
				if(_worker!=null) _worker.Interrupt();        
				_wh.Close();            // 释放资源
				_wh.Dispose();
			}
            catch (Exception)
            {
				 
            }
			
		}


		/// <summary>处理保存</summary>
		static void SaveData(List<string> data)
		{
            try
            {
				string s = "";
                foreach (var item in data)
                {
					s = s + item;
				} 
				var xxx = _SensorServices.ExecuteCommand(s).Result;
				ULogger.Info("D", null, false);
			}
            catch (Exception ex)
            {
				ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
			}
		}


	}
	 
}
 
