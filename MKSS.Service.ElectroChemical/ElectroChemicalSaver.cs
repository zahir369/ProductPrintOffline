using System.Collections.Generic;
using System;
using MKSS.Services;
using MKSS.Model;
using MKSS.Util.Log;
using System.Threading;
using SqlSugar;

namespace MKSS.Service.ElectroChemical
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

		public static string DbNameof(Batch _bat) {
			if (!System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "data")) System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\data");
			return string.Format(string.Format(AppDomain.CurrentDomain.BaseDirectory + @"data\MKSS.APP.ElectroChemical.{0}.sqlite", _bat.F_BatchId));
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
				//DataSource=D:\WORK\Factory\MKSS.APP.ElectroChemical\bin\Debug\net5.0-windows\MKSS.APP.ElectroChemical.sqlite
				if (value != null) {
					SqlSugarClient client = _SensorServices.BaseDal.Db as SqlSugarClient;
					ConnectionConfig cfgold = client.CurrentConnectionConfig;
					ConnectionConfig cfg = new ConnectionConfig()
					{
						ConnectionString = string.Format("DataSource={0}", DbNameof(value)),//必填, 数据库连接字符串
						DbType = cfgold.DbType,//必填, 数据库类型
						IsAutoCloseConnection = cfgold.IsAutoCloseConnection,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
						IsShardSameThread = cfgold.IsShardSameThread,//共享线程
						InitKeyType = cfgold.InitKeyType//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
					};
					Util.MainDb.MainConnectionReplace = cfg;
					_SensorServices = new SensorServices();

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
			}
            catch (Exception ex)
            {
				ULogger.Log(string.Format("插入数据出错 {0}", ex.Message));
			}
		}


	}
	 
}
 
