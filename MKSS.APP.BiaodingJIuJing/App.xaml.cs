using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Threading;
using System.Globalization;
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
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            base.OnStartup(e);
        }

        void Log(string str)
        {
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
         System.Environment.NewLine);
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
         str);
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
        }
         
    }
}
