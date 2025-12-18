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
using MKSS.Service.ZhuiSu;
using MKSS.APP.ZhuiSuService.MQTT;

namespace MKSS.WindwsService.DeviceMonitor
{
    [LogTagClass(Title = "程序启动")]
    public class Program
    {
        /// <summary>
        ///   检测任务操作类
        /// </summary>
        public static ZhuiSuService ZhuiSuService { get; set; }
        public static void Main(string[] args)
        {

            CreateHostBuilder(args).Build().Run();
            ULogger.Info("启动任务成功...");

        }



        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()//指定项目可以部署为Windows服务
                .ConfigureServices((hostContext, services) =>
                {

                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                    ULogger.Info("初始数据...");
                    ZhuiSuService = new ZhuiSuService();
                    ULogger.Info("第一次刷新任务状态...");
                    Program.ZhuiSuService.RefreshTask();
                    //ULogger.Info("开始读取数据...");
                    //Program.ZhuiSuService.ReadVoltageRecycle().Wait();

                    ULogger.Info("初始MQTT...");
                    ZhuiSuMqttServer.Instance.StartMqttServerAssert();
                    ZhuiSuMqttClientProducer.Instance.Start();

                    ULogger.Info("启动任务...");

                    ULogger.Info("AddHostedService Worker...");
                    services.AddHostedService<Worker>();

                    // Add Quartz services 在StartUp的ConfigureServices中注入
                    ULogger.Info("Add SingletonJobFactory...");
                    services.AddSingleton<IJobFactory, SingletonJobFactory>();
                    ULogger.Info("Add StdSchedulerFactory...");
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    // Add our job 
                    ULogger.Info("Add ZhuiSuTaskRefreshJob...");
                    services.AddSingleton<ZhuiSuTaskRefreshJob>();

                    //设置任务调度执行频率
                    ULogger.Info("Add AddJob...");
                    services.AddJob();

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
