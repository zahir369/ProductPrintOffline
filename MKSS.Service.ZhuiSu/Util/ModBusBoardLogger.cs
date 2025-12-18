using NModbus;
using System;
using NModbus.Logging;
using System.Text;

namespace MKSS.Service.ZhuiSu.Util
{
    public class ModBusBoardLogger : ModbusLogger
	{
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);

		public ModBusBoardLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug)
			: base(minimumLoggingLevel)
		{
		}

        private readonly object locked = new object();
		protected override void LogCore(LoggingLevel level, string message)
		{
            message = message?.Replace(Environment.NewLine, BlankHeader);
            try
            {
                lock (this.locked) {
                    string n = System.Threading.Thread.CurrentThread.Name;
                    string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                    if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
                    string filename = dir + DateTime.Now.ToString("HH") + "_" + n + "_COMMAND.log";
                    if (!System.IO.File.Exists(filename)) System.IO.File.Create(filename).Close();
                    System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, Encoding.Default);
                    sw.Write(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + message + System.Environment.NewLine);
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

        private readonly object obj = new object();
        public void WriteLock(object user)
        {
            lock (this.obj)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("----------------------------" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--------------------------");
                sb.AppendLine(user.ToString());
                sb.AppendLine("---------------------------------------------------------------------------------");
                sb.AppendLine();
                if (!System.IO.File.Exists(path))
                    System.IO.File.Create(path).Close();
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
