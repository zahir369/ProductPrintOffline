using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WpfChartTest.Model;
using System.ComponentModel;

namespace WpfChartTest
{
    public delegate void NewDataEventHandler( SensorGroupData datas_history, double F_AddTime);
    public class TestData
    {
        public event NewDataEventHandler OnNewData;
        public DateTime StartTime = DateTime.Now;
        public TimeSpan StartTaskInterval { get; set; } = new TimeSpan(0, 0, 0, 0, 500);
        private BackgroundWorker bgWorker = null;
        public void StartTask() {
            if (bgWorker != null)
            {
                try
                {
                    bgWorker.CancelAsync();
                    bgWorker.Dispose();
                    bgWorker = null;
                }
                catch (Exception)
                {

                }
                Thread.Sleep(200);
            }

            bgWorker = new BackgroundWorker();
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.ProgressChanged -= new ProgressChangedEventHandler(bgWorker_ProgessChanged);
            bgWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
            bgWorker.DoWork -= new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);

            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);


            StartTime = DateTime.Now;
            bgWorker.RunWorkerAsync();
            Thread.Sleep(50);

        }

        public void StopTask()
        {
            
            if (bgWorker != null)
            {
                try
                {
                    bgWorker.CancelAsync();
                    bgWorker.Dispose();
                    bgWorker = null;
                }
                catch (Exception)
                {

                }
            }
        }



        bool DoWorking = false;
        public void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

            
                DoWorking = true;

              
                try
                {

  
                    while (true)
                    {
                        try
                        {
  

                            bool read_sucess = false;
                            ReadVoltageRecycleInner( );

                             

                            int s_temp = 0;
                            int sleep = ((int)StartTaskInterval.TotalMilliseconds);
                             

                            while (s_temp < (int)sleep)//避免长时间睡眠
                            {
                                //等待下次数据采集
                                s_temp += 50;
                                Thread.Sleep(50);
                            }

                        }
                        catch (Exception exx)
                        {
                            ULogger.Info("出错结束！错误：" + exx.Message + "");
                            break;
                        }
                        finally
                        {

                        }

                    }

                    ULogger.Info("正常退出X ！");

                }
                catch (Exception ex)
                {
                    ULogger.Info(ex.Message);
                    ULogger.Info("串口打开失败，请检查系统串口是否可用！");
                    return;
                }

            }
            catch (Exception ex)
            {
                ULogger.Log(string.Format("刷新数据时出错 {0}", ex.Message));
                ULogger.Log(ex.Message);
            }
            finally
            {

                
                DoWorking = false;
            }
        }

        public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ULogger.Info(e.Error.ToString());
                return;
            }
            if (!e.Cancelled)
                ULogger.Info("处理完毕!");
            else
                ULogger.Info("处理终止!");

        }

        bool ReadVoltageRecycleInnerDoing = false; 
        void ReadVoltageRecycleInner( )
        {
 
             
            //记录当前时间，如有必要回滚时间
            try
            {
                 
                ReadVoltageRecycleInnerDoing = true;

                try
                {
                     
                    DateTime beforeDT0 = System.DateTime.Now;
                    ushort[] ret = GetRandNums(0, 650, 64).ToArray();
                    SensorGroupData datag = new SensorGroupData(ret, Math.Round((DateTime.Now - StartTime).TotalSeconds, 2)) ;
                    if (OnNewData != null ) OnNewData(datag, datag.F_AddTime);
                }
                catch (Exception ex)
                {
                    ULogger.Info(ex.Message);
                    ULogger.Info(ex.StackTrace);
                }


            }
            catch (Exception ex)
            {
                ULogger.Log(string.Format("读取出错 {0}", ex.Message));
                ULogger.Log(string.Format("读取出错 {0}", ex.StackTrace));
            }
            finally
            {
  
                ReadVoltageRecycleInnerDoing = false;
            }


        }

        public static List<ushort> GetRandNums(int min, int max, int num)

        {
            List<ushort> list = new List<ushort>();
            for (int i = 0; i < num; i++)
            {
                Random rd = new Random();
                ushort temp = (ushort)rd.Next(min, max);
                //while (list.Contains(temp))
                //{
                //	temp = (ushort)rd.Next(min, max);
                //}
                list.Add(temp);
            }

            return list;

        }

    }

}
