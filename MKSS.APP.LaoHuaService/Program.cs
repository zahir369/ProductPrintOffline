using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MKSS.Util.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Spi;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quartz.Impl;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using MKSS.WindwsService.DeviceMonitor.Job;
using MKSS.Service.LaoHua;
using MKSS.APP.LaoHuaService.MQTT;

namespace MKSS.WindwsService.DeviceMonitor
{
    [LogTagClass(Title = "程序启动")]
    public class Program
    {


        #region 防止程序多开
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        private static Process RuningInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (Process process in Processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        #endregion


        /// <summary>
        ///   老化任务操作类
        /// </summary>
        public static LaohuaService LaohuaService { get; set; }
        public static void Main(string[] args)
        {
            //防止程序多开
            Process process = RuningInstance();
            if (process == null)
            {
                if (args.Length == 1) //make sure an argument is passed
                {
                    
                }
            }
            else
            { 
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(1);
            }

            CreateHostBuilder(args).Build().Run();
        }
         

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()//指定项目可以部署为Windows服务
                .ConfigureServices((hostContext, services) =>
                {

                    MKSS.Util.Log.ULogger.LogDirecrory = "老化服务端日志";
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    ULogger.Info("初始数据...");
                    LaohuaService = new LaohuaService();
                    ULogger.Info("第一次刷新任务状态...");
                    Program.LaohuaService.RefreshTask();
                    ULogger.Info("第一次读取数据...");
                    Program.LaohuaService.StartLahuaTask().Wait();

                    ULogger.Info("初始MQTT...");
                    LaoHuaMqttServer.Instance.StartMqttServerAssert();
                    LaoHuaMqttClientProducer.Instance.Start();

                    ULogger.Info("启动任务...");

                    ULogger.Info("AddHostedService Worker...");
                    services.AddHostedService<Worker>();

                    // Add Quartz services 在StartUp的ConfigureServices中注入
                    ULogger.Info("AddSingleton SingletonJobFactory...");
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    ULogger.Info("AddSingleton StdSchedulerFactory...");
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    // Add our job 
                    ULogger.Info("AddSingleton LaoHuaTaskReadJob...");
                    services.AddSingleton<LaoHuaTaskReadJob>();

                    //设置任务调度执行频率
                    ULogger.Info("AddJob...");
                    services.AddJob();

                    ULogger.Info("AddHostedService QuartzHostedService...");
                    services.AddHostedService<QuartzHostedService>();

                    ULogger.Info("启动任务成功...");

                })
                .UseDefaultServiceProvider(options => options.ValidateScopes = false);


        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            ULogger.Info("UnhandledException");
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                ULogger.Info(ex.Message);
                ULogger.Info(ex.StackTrace); 
            }

        }


    }
}
