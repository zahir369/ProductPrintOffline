using MKSS.APP.WinNBOnlinePrinterTest.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            //加载提示 
            SplashForm s = new SplashForm();
            s.Show();
            Application.Run(new FormAssemble());
            ////登录
            //Application.Run(new FormLogin());

            

            //if(AppConfig.Token != "")
            //{
            //    //加载提示 
            //    SplashForm s = new SplashForm();
            //    s.Show();
            //    Application.Run(new FormAssemble());

            //}
            //else
            //{
            //   System.Environment.Exit(0);
            //}


            //if (FormPacking.VersionPacking== VersionPacking.Packing)
            //{
            //    Application.Run(new FormPacking());
            //}

            //if (FormPacking.VersionPacking == VersionPacking.Assemble)
            //{
            //    Application.Run(new FormAssemble());
            //}


            //if (FormPacking.VersionPacking == VersionPacking.AppIMEI)
            //{
            //    Application.Run(new FormAppIMEI());
            //}

        }



        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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

        static void Log(string str)
        {
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { System.Environment.NewLine });
            System.IO.File.AppendAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\log.log",
                new string[] { str });
        }
    }

    public enum ProcessType {
        /// <summary>
        ///  组装过程
        /// </summary>
        AssembleProcess = 1,
        /// <summary>
        ///  装箱过程
        /// </summary>
        PackingProcess = 1,
    }
}
