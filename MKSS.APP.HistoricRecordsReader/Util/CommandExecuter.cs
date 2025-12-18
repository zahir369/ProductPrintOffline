using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace MKSS.APP.HistoricRecordsReader
{
    public class CommandExecuter
    {
        // 任务队列
        public static Queue<CommandExt> Tasks = new Queue<CommandExt>();

        // 为保证线程安全，使用一个锁来保护_task的访问
        readonly static object _locker = new object();

        // 通过 _wh 给工作线程发信号
        static EventWaitHandle _wh = null; 
        static Thread _worker;

        public static int milli = 180;
        public static bool Running { get; private set; }
        public static FormSensor FormSensor { get; private set; }
        public static void Start(FormSensor form,int m)
        {
            Tasks.Clear();
            milli = m;
            FormSensor = form;
            FormSensor.Commands.Clear();
            Cancel = false;
            _wh = new AutoResetEvent(false); 

            // 任务开始，启动工作线程
            _worker = new Thread(Work);
            _worker.Start();
        }

        public static void ClearTask() {
            Tasks.Clear();
        }

        static bool Cancel = false;
        /// <summary>执行工作</summary>
        static void Work()
        {
            Running = true;
            while (true)
            {

                if (Cancel) break;
                List<CommandExt> works = new List<CommandExt>();
                lock (_locker)
                {
                    while (works.Count <= 0 && Tasks.Count > 0)
                    {
                        CommandExt work = Tasks.Dequeue(); // 有任务时，出列任务
                        if (work == null)  // 退出机制：当遇见一个null任务时，代表任务结束
                            break;
                        works.Add(work);
                    }
                }

                try
                {
                    if (works.Count > 0)
                    {
                        ExeData(works[0]);  // 任务不为null时，处理并保存数据
                    }
                    else {
                        //FormSensor.ProgressBarVisible = false;
                        _wh.WaitOne();   // 没有任务了，等待信号
                    }
                }
                catch (Exception ex)
                {
                    FormSensor.Log(string.Format("Work {0}", ex.Message));
                }

            }
            Running = false;
        }

        /// <summary>插入任务</summary>
        public static void EnqueueTask(CommandExt task)
        {
            try
            {
                lock (_locker) {
                    if(FormSensor!=null) FormSensor.Commands.Add(task);
                    Tasks.Enqueue(task);  // 向队列中插入任务 ccccccccc
                }

                if (_wh != null) _wh.Set();  // 给工作线程发信号
            }
            catch (Exception ex)
            {
                FormSensor.Log(string.Format("EnqueueTask {0}", ex.Message));
            }

        }

        /// <summary>结束释放</summary>
        public static void Dispose()
        {
            try
            {
                Cancel = true;
                Tasks.Clear();
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
            finally
            {
                
            }

        }

        static DateTime PreExeData { get; set; } = new DateTime(2000, 1, 1);
        /// <summary>处理保存</summary>
        static void ExeData(CommandExt data)
        {
            try
            {

                if (data.Result == CommandExt.ResultEmpString) return;

                //如果同类型数据，前一条是空数据那么忽略之后所有项目
                var xxall = FormSensor.Commands.Commands.Values.ToList();
                int empcount = xxall.Count(w =>
                    w.CommandType == data.CommandType
                    && w.Reponsed && w.Result == CommandExt.ResultEmpString);
                if (empcount > 0) {
                    var xx = xxall.Where(w =>
                            w.CommandType == data.CommandType
                            && w.Par1>= data.Par1 ).Select(w=>w).ToList();
                    foreach (var item in xx)
                    {
                        item.Result = CommandExt.ResultEmpString;
                    }
                    return;
                }

                //DateTime pre = PreExeData;
                //TimeSpan ts = DateTime.Now - pre;
                //if (ts.TotalSeconds < milli)
                //{

                //}
                //PreExeData = DateTime.Now;
                Thread.Sleep(milli);
                FormSensor.ExeuteCommand(data.CommandType, data.Par1);
                FormSensor.Log(string.Format("查询数据 {0} {1}", data.CommandType, data.Par1));
            }
            catch (Exception ex)
            {
                FormSensor.Log(string.Format("查询[{1} {2}]出错, {0}", ex.Message, data.CommandType, data.Par1));
            }
        }


    }

}