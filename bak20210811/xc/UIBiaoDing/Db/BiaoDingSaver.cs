using System.Collections.Generic;
using System;
using MKSS.Services;
using MKSS.Model;
using System.Threading;
using System.IO;

namespace MKSS.Service.UIBiaoDing
{
	public class BiaoDingSaver
	{
		// 任务队列
		static Queue<string> _tasks = new Queue<string>();

		// 为保证线程安全，使用一个锁来保护_task的访问
		readonly static object _locker = new object();

		// 通过 _wh 给工作线程发信号
		static EventWaitHandle _wh = null;
		static SensorServices _SensorServices = null;
		static SensorDataServices _SensorDataServices = null;
		static Thread _worker;

		public static SensorServices SensorServices { get { return _SensorServices; } }
		public static SensorDataServices SensorDataServices { get { return _SensorDataServices; } }

		public static bool Running{ get; private set; }
		public static void Start(Batch _Batch)
		{
			Batch = _Batch;
			Cancel = false;
			_wh = new AutoResetEvent(false);
			_SensorServices = new SensorServices();
			_SensorDataServices = new SensorDataServices();
			//DataSource=D:\WORK\Factory\MKSS.APP.UIBiaoDing\bin\Debug\net5.0-windows\MKSS.APP.UIBiaoDing.sqlite
			_SensorServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", DbNameof(_Batch));
			_SensorDataServices.BaseDal.Db.CurrentConnectionConfig.ConnectionString = string.Format("DataSource={0}", DbNameof(_Batch));
			// 任务开始，启动工作线程
			_worker = new Thread(Work);
			_worker.Start();
		}

		public static string DbNameof(Batch _bat)
		{
			string dir = (new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory)).Parent.FullName + "\\美克盛世气体传感器标定数据";
			if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
			if (!System.IO.File.Exists(dir + @"\MKSS.MAIN.db")) System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + @"MKSS.APP.BiaodingCommon.sqlite", dir + @"\MKSS.MAIN.db");
			if (_bat == null) return dir + @"\MKSS.MAIN.db"; 
			return string.Format(string.Format(dir + @"\MKSS.{0}.db", _bat.F_BatchId));
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
				 
				_Batch = value;
			}
		}

		static bool Cancel = false;
		/// <summary>执行工作</summary>
		static void Work()
		{
			Running = true;
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
				catch (Exception ex)
				{
					Console.WriteLine(string.Format("Work {0}", ex.Message));
				}

			}
			Running = false;
		}

		/// <summary>插入任务</summary>
		public static void EnqueueTask(string task)
		{
            try
            {
				lock (_locker)
					_tasks.Enqueue(task);  // 向队列中插入任务 

				if(_wh!=null) _wh.Set();  // 给工作线程发信号
			}
            catch (Exception ex)
            {
				Console.WriteLine(string.Format("EnqueueTask {0}", ex.Message));
			}
			
		}

		/// <summary>结束释放</summary>
		public static void Dispose()
		{
			try
			{
				Cancel = true;
				EnqueueTask(null);      // 插入一个Null任务，通知工作线程退出
				if(_wh!=null) _wh.Set();  // 给工作线程发信号
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
				_SensorServices = null;
				_SensorDataServices = null;
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
			}
            catch (Exception ex)
            {
				Console.WriteLine(string.Format("插入数据出错 {0}", ex.Message));
			}
		}


	}
	 
}
 
