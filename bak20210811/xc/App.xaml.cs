using DeviceDataMonitorWPF.UIBiaoDing.Util;
using MKSS.Service.UIBiaoDing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MKSS.APP.BiaodingAlcohol
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            int x1 = 0XAD;
            // AD 00 设置编号，AD 01 读取产品编号
            ushort xx0 = BitConverter.ToUInt16(new byte[] { (byte)0XAD, (byte)0X00 }, 0);
            ushort xx1 = BitConverter.ToUInt16(new byte[] { (byte)0XAD, (byte)0X01 }, 0);
            Log("enter");
            ModBusBoardLogger.Delete(3);
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
}
