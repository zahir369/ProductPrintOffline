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
using MKSS.WindwsService.DeviceMonitor.Job;
using Quartz.Impl;
using MKSS.Service.LaoHua;
using MKSS.APP.LaoHuaService.MQTT;

namespace MKSS.WindwsService.DeviceMonitor
{
    [LogTagClass(Title = "程序启动")]
    public class Program
    {
        /// <summary>
        ///   老化任务操作类
        /// </summary>
        public static LaohuaService LaohuaService { get; set; }
        public static void Main(string[] args)
        {

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
            CreateHostBuilder(args).Build().Run();
            ULogger.Info("启动任务成功...");
             
        }
         

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    // Add Quartz services 在StartUp的ConfigureServices中注入
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    // Add our job 
                    services.AddSingleton<LaoHuaTaskReadJob>();

                    //设置任务调度执行频率
                    services.AddJob();

                    services.AddHostedService<QuartzHostedService>();

                });


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
