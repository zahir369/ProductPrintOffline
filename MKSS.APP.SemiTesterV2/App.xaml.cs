using MKSS.APP.UIBiaoDing.Util;
using MKSS.Service.UIBiaoDing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace MKSS.APP.BiaodingAlcohol
{
    /// <summary>
    /// App.xaml 的交互逻辑
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
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == currentProcess.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
        #endregion


        protected override void OnStartup(StartupEventArgs e)
        {

            //防止程序多开
            Process process = RuningInstance();
            if (process == null)
            {
                if (e.Args.Length == 1) //make sure an argument is passed
                {
                    FileInfo file = new FileInfo(e.Args[0]);
                    if (file.Exists) //make sure it's actually a file
                    {
                        //Do whatever
                        MessageBox.Show(file.FullName);
                    }
                }
            }
            else
            { 
                HandleRunningInstance(process);
                System.Threading.Thread.Sleep(1000);
                System.Environment.Exit(1);
            }


            int x1 = 0XAD;
            // AD 00 设置编号，AD 01 读取产品编号
            ushort xx0 = BitConverter.ToUInt16(new byte[] { (byte)0XAD, (byte)0X00 }, 0);
            ushort xx1 = BitConverter.ToUInt16(new byte[] { (byte)0XAD, (byte)0X01 }, 0);
            Log("enter");



            CommLogger.Delete(3);
            BiaoDingSaver.DbNameof(null);
            int xxx = 100_000;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);
        }

        void Log(string str)
        {
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { System.Environment.NewLine });
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { str });
        }
         
        protected override void OnExit(ExitEventArgs e)
        {
            //停止定时服务
            base.OnExit(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            Log("UnhandledException");
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                Log(ex.Message);
                Log(ex.StackTrace);
                if (ex.InnerException != null) {
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

    }


    //WM_COPYDATA消息所要求的数据结构
    public struct CopyDataStruct
    {
        public IntPtr dwData;
        public int cbData;

        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData;
    }

}
