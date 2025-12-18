using DeviceDataMonitorWPF.UIControls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MKSS.IServices;
using MKSS.Services;
using MKSS.IRepository;
using MKSS.Repository;
using MKSS.Util;
using System;
using SqlSugar;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MKSS.IRepository.Base;
using MKSS.Repository.Base;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading;
using System.Globalization;
using MKSS.ICommon;
using MKSS.Common;
using System.Windows.Navigation;
using System.Reflection;
using Stylet; 
using System.Runtime.InteropServices;
using System.Diagnostics;
using MKSS.Service.LaoHuaElectroChemical;
using MKSS.APP.LaoHuaService.MQTT;

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region 防止程序多开
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。
        ///  系统给创建前台窗口的线程分配的权限稍高于其他线程。 
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWNOMAL = 1;
        private static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOWNOMAL);//显示
            SetForegroundWindow(instance.MainWindowHandle);//当到最前端
        }

        private static Process RuningInstance()
        {
            Process currentProcess = Process.GetCurrentProcess();
            Process[] Processes = Process.GetProcessesByName(currentProcess.ProcessName);
            foreach (Process process in Processes)
            {
                if (process.Id != currentProcess.Id)
                {
                    if (
                        Assembly.GetExecutingAssembly().Location.Replace("/", "\\").TrimEnd(".exe".ToCharArray()).TrimEnd(".dll".ToCharArray())
                        == 
                        currentProcess.MainModule.FileName.TrimEnd(".exe".ToCharArray()).TrimEnd(".dll".ToCharArray())
                        )
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        #endregion
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            Log("enter");


            //防止程序多开
            Process process = RuningInstance();
            if (process == null)
            {
                //if (e.Args.Length == 1) //make sure an argument is passed
                //{
                //    FileInfo file = new FileInfo(e.Args[0]);
                //    if (file.Exists) //make sure it's actually a file
                //    {
                //        //Do whatever
                //        MessageBox.Show(file.FullName);
                //    }
                //}
            }
            else
            {
                process.Kill();
                //HandleRunningInstance(process);
                //System.Threading.Thread.Sleep(1000);
                //System.Environment.Exit(1);
            }

            //三种模式 
            LaoHuaDataProvider.UIMode = LaoHuaDataProviderUIMode.SerialPort;
            if (e.Args != null && e.Args.Length > 0)
            {
                if (e.Args[0] == "SerialPort")
                {
                    LaoHuaDataProvider.UIMode = LaoHuaDataProviderUIMode.SerialPort;
                }
                if (e.Args[0] == "NetWork") LaoHuaDataProvider.UIMode = LaoHuaDataProviderUIMode.NetWork;
            }

            if (LaoHuaDataProvider.UIMode == LaoHuaDataProviderUIMode.SerialPort) {
                ConnectionConfig cfg = new ConnectionConfig()
                {
                    ConnectionString = string.Format("DataSource={0}", string.Format(string.Format(AppDomain.CurrentDomain.BaseDirectory + @"MKSS.APP.LaoHuaElectroChemical.sqlite"))),//必填, 数据库连接字符串
                    DbType = SqlSugar.DbType.Sqlite,//必填, 数据库类型
                    IsAutoCloseConnection = true,//默认false，为手动或延时关闭数据库连接；设置为true，自动关闭连接，无需使用using或者Close操作
                    IsShardSameThread = true,//共享线程
                    InitKeyType = InitKeyType.SystemTable//默认SystemTable, 字段信息读取, 如：该属性是不是主键，标识列等等信息
                };
                MainDb.MainConnectionReplace = cfg;
            }
            

            ElectroChemicalService.Debug = Appsettings.App(new string[] { "Debug" }).ObjToString().ToLower()=="true";
            MKSS.Util.Log.ULogger.Delete(3);
            int xxx = 100_000;
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                var css = (new BatchServices()).Query<MKSS.Model.Batch>(null).Result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+System.Environment.NewLine+ex.StackTrace, "数据库未连接");
                return;
            }
            Log("db sucess");
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            base.OnStartup(e);
        }

        void Log(string str)
        {
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { System.Environment.NewLine } );
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { str });
        }

        static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            //停止定时服务
            base.OnExit(e);
        }


        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            base.OnLoadCompleted(e);
        }


        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Log("UnhandledException");
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                Log(ex.Message);
                Log(ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Exception exinn = ex.InnerException;
                    Log(exinn.Message);
                    Log(exinn.StackTrace);
                    if (exinn.InnerException != null)
                    {
                        Exception exinn2 = exinn.InnerException;
                        Log(exinn2.Message);
                        Log(exinn2.StackTrace);
                        MessageBox.Show(exinn2.Message + System.Environment.NewLine + exinn.Message + System.Environment.NewLine + ex.Message);
                        return;
                    }
                    MessageBox.Show(exinn.Message + System.Environment.NewLine + ex.Message);
                    return;
                }
                MessageBox.Show(ex.Message);
                return;
            }

        }


        private void ConfigureServices(IServiceCollection services)
        {
            //services.Configure<Appsettings>(Configuration.GetSection(nameof(Appsettings)));
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    Init();
        //    base.OnStartup(e);
        //}

        //private void ConfigureServices(IServiceCollection services)
        //{
        //    services.TryAddTransient<MainWindow>();
        //    services.AddJsonLocalization(options => options.ResourcesPathType = ResourcesPathType.CultureBased);
        //}

        //private void Init()
        //{
        //    #region Init Settings

        //    var settings = new SettingsViewModel();
        //    settings.ConnectionString = settings.DefaultConnectionString;
        //    // set current culture
        //    Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(settings.DefaultCulture);
        //    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(settings.DefaultCulture);

        //    #endregion Init Settings

        //    IServiceCollection services = new ServiceCollection();
        //    ConfigureServices(services);

        //    services.BuildServiceProvider()
        //         .GetRequiredService<MainWindow>()
        //         .Show();
        //}



        //====================注入配置文件信息==============================
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        //    Configuration = builder.Build();


        //    var serviceCollection = new ServiceCollection();
        //    ConfigureServices(serviceCollection);
        //    ServiceProvider = serviceCollection.BuildServiceProvider();
        //}

        ///// <summary>
        ///// 注入对象
        ///// </summary>
        ///// <param name="services"></param>
        //private void ConfigureServices(IServiceCollection services)
        //{
        //    //services.Configure<Appsettings>(Configuration.GetSection(nameof(Appsettings)));

        //    //services.AddSqlsugarSetup();

        //    //services.AddSingleton(new Appsettings(Configuration)); 
        //}

    }
}
