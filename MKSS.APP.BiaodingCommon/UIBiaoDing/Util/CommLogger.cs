using NModbus;
using System;
using NModbus.Logging;
using System.Text;
using System.IO;
using MKSS.Service.LaoHua;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class CommLogger : ModbusLogger
	{
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);

        public BoardConnection Connection { get; set; }
        public CommLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug, BoardConnection conn =null)
			: base(minimumLoggingLevel)
		{
            Connection = conn;
        }

        static object locked = new object();
		protected override void LogCore(LoggingLevel level, string message)
		{
            message = message?.Replace(Environment.NewLine, BlankHeader);
            try
            {
                lock (locked) {
                    string n = System.Threading.Thread.CurrentThread.Name;
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                    
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    string filename = dir + DateTime.Now.ToString("HH") + "_" + n + "_COMMAND.log";
                    if (!System.IO.File.Exists(filename)) System.IO.File.Create(filename).Close();
                    System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, Encoding.Default);
                    sw.Write(DateTime.Now.ToString("HH:mm:ss.fff") + "["+ Connection + "]:" + message + System.Environment.NewLine);
                    sw.Close();
                    sw.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                }
            }
            catch (Exception)
            {
                 
            }
        }


        /// <summary>
        ///  删除日志
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns></returns>
        public static void Delete(int days)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\"  ;
            if (System.IO.Directory.Exists(dir))
            {
                var dirs = (new DirectoryInfo(dir)).GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    DirectoryInfo d = dirs[i];

                    try
                    {
                        if (int.Parse(d.Name) < int.Parse(DateTime.Now.ToString("yyyyMMdd")) - days)
                        {
                            Directory.Delete(d.FullName, true);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
                Directory.CreateDirectory(dir);
                
            }

        }

        static  object obj = new object();
        public void WriteLock(object user)
        {
            lock (obj)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";
                if (!System.IO.Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("----------------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--------------------------");
                sb.AppendLine(user.ToString());
                sb.AppendLine("---------------------------------------------------------------------------------");
                sb.AppendLine();
                if (!System.IO.File.Exists(path)) System.IO.File.Create(path).Close();

                System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, Encoding.Default);
                sw.Write(sb.ToString());
                sw.Close();
                sw.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
        }

    }
}
