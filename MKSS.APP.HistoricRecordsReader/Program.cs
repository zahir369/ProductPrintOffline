using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.HistoricRecordsReader
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
            Application.Run(new FormSensor());
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
}
