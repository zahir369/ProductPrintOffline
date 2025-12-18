using DeviceDataMonitorWPF.UIControls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MKSS.IServices;
using MKSS.Services;
using MKSS.IRepository;
using MKSS.Repository;
using MKSS.Util;
using System;
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

namespace DeviceDataMonitorWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            Log("enter");
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                var css = (new BoardCaseServices()).Query<MKSS.Model.BoardCase>(null).Result;
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
                MessageBox.Show(ex.Message);
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
