using System;
using System.Collections.Generic;
using System.Text; 
//using ICSharpCode.SharpZipLib.Zip;
using System.IO; 
using System.Security.Cryptography;
using MKSS.Util.Log.License;
using MKSS.Util.Loger;

namespace MKSS.Util.Log
{

    /// <summary>
    ///  日志
    /// </summary>
    public class ULogger
    {

        /// <summary>
        ///  打开后，不区分，全部写入日志文件
        /// </summary>
        public static bool DebugAllLog { get; set; }
        public static bool LogToEventIgnore { get; set; }
        public static bool LogToFileIgnore { get; set; }
        public static bool LogDebugToCDriveOn { get; set; }
        static bool AuthroizChecked { get; set; }
        public static bool Authroized { get;private set; }
        public static event ULogOnLog OnLog;
        public static event ULogOnLog OnLogBase;
        public static event ULogOnLogOf OnLogOf;
        
        /// <summary>
        ///  关键性提示，例如：保存成功！
        /// </summary>
        public static event UKeyMessage OnKeyMessage;
        public static event ULogOnErrorLicense OnErrorLicense;
        public static event ULogError OnError; 
        public static Dictionary<string, LogTagConfig> LogConfigs = new Dictionary<string, LogTagConfig>();
        public static Dictionary<string, bool> LogDataVisibleConfigs = new Dictionary<string, bool>();
        public static Dictionary<string, LogTagClass> LogConfigTagged = new Dictionary<string, LogTagClass>();
        static Dictionary<string, LogTagConfigUserDefine> UserDefine = new Dictionary<string, LogTagConfigUserDefine>();

        /// <summary>
        /// 构造函数
        /// </summary>
        static ULogger()
        {

        }

        /// <summary>
        ///  删除日志
        /// </summary>
        /// <param name="days">保留天数</param>
        /// <returns></returns>
        public static void Delete(int days)
        {
            string dir = MKSS.Util.Log.ULogger.StartupWebRoot + "\\Log\\" ;
            if (System.IO.Directory.Exists(dir))
            {
                var dirs = (new DirectoryInfo(dir)).GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    DirectoryInfo d = dirs[i];
                    
                    try
                    {
                        if (int.Parse(d.Name) < int.Parse(DateTime.Now.ToString("yyyyMMdd")) - days) {
                            Directory.Delete(d.FullName,true);
                        }
                    }
                    catch (Exception ex)
                    {
                         
                    }
                }
                Directory.CreateDirectory(dir);
                //if (LoggerEf != null)
                //{
                //    LoggerEf.StopLogging();
                //    LoggerEf.Dispose();
                //    LoggerEf = null;
                //}
                //LoggerEf = new System.Data.Entity.Infrastructure.Interception.DatabaseLogger(dir + "\\00_EF.log", true);
                //LoggerEf.StartLogging();
            }
             
        }

        public static string CurrentLogFile(string title) {
            string dir = MKSS.Util.Log.ULogger.StartupWebRoot + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!System.IO.Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir); 
                //if (LoggerEf != null)
                //{
                //    LoggerEf.StopLogging();
                //    LoggerEf.Dispose();
                //    LoggerEf = null;
                //}
                //LoggerEf = new System.Data.Entity.Infrastructure.Interception.DatabaseLogger(dir + "\\00_EF.log", true);
                //LoggerEf.StartLogging();
            }
            string filename = dir + DateTime.Now.ToString("HH") + "_" + title + ".log";
            if (!File.Exists(filename)) File.Create(filename).Close();
            return filename;
        }

        public static void Info(object stringValue, ULogDataFilter filter = null)
        { 
            Log(stringValue + "",   filter = null);
        }

        public static void Log(string str, ULogDataFilter filter = null)
        {
            try
            {
                 

                if (LogDebugToCDriveOn)
                {
                    string filename = "c:\\Sgj.log";
                    if (!File.Exists(filename)) File.Create(filename).Close();
                    File.AppendAllText(filename, DateTime.Now.ToString("HH:mm:ss") + ":" + str + System.Environment.NewLine);
                }


                LogInnerBefore(ref str);

                if (LogToEventIgnore && LogToFileIgnore) return;

                bool showlog = true;
                LogTagClass class_current = null;
                DateTime start = DateTime.Now;

                //找到栈列表的日志类 
                System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
                foreach (System.Diagnostics.StackFrame frmae in stack.GetFrames())
                {



                    #region 从方法中获取日志类别信息
                    object[] attributes_methord = frmae.GetMethod().GetCustomAttributes(typeof(LogTagClass), false);
                    if(attributes_methord!=null && attributes_methord.Length>0){
                        Type currentType = frmae.GetMethod().DeclaringType;
                        System.Reflection.MethodBase _MethodBase = frmae.GetMethod();
                        string key = currentType.FullName + "_" + _MethodBase.Name;
                        if (!LogConfigTagged.ContainsKey(key))
                        {
                            LogTagClass attr = (LogTagClass)attributes_methord[0];
                            LogConfigTagged.Add(key, attr);
                        }

                        if (LogConfigTagged[key] != null)
                        {
                            if (LogConfigs.ContainsKey(key))
                            {
                                showlog = LogConfigs[key].Visible;
                                class_current = LogConfigs[key].ULog;
                                break;
                            }
                            else
                            {
                                LogConfigs.Add(key, new LogTagConfig()
                                {
                                    Title = LogConfigTagged[key].Title,
                                    Visible = LogConfigTagged[key].DefaultShow,
                                    TypeFullName = key,
                                    ULog = LogConfigTagged[key]
                                });
                                showlog = LogConfigs[key].Visible;
                                class_current = LogConfigs[key].ULog;
                                break;
                            }
                        }
                    } 
                    #endregion

                    #region 从类中获取日志类别信息
                    Type current = frmae.GetMethod().DeclaringType;
                    if (current == null) continue;
                    if (!LogConfigTagged.ContainsKey(current.FullName))
                    {
                        object[] attributes = current.GetCustomAttributes(typeof(LogTagClass), false);
                        if (attributes != null && attributes.Length > 0 && !LogConfigTagged.ContainsKey(current.FullName))
                        {
                            LogTagClass attr = (LogTagClass)attributes[0];
                            LogConfigTagged.Add(current.FullName, attr);
                        }
                        else
                        {
                            LogConfigTagged.Add(current.FullName, null);
                        }
                    }

                    #endregion

                    if (LogConfigTagged[current.FullName] != null)
                    {
                        if (LogConfigs.ContainsKey(current.FullName))
                        {
                            showlog = LogConfigs[current.FullName].Visible;
                            class_current = LogConfigs[current.FullName].ULog;
                            break;
                        }
                        else
                        {
                            LogConfigs.Add(current.FullName, new LogTagConfig()
                            {
                                Title = LogConfigTagged[current.FullName].Title,
                                Visible = LogConfigTagged[current.FullName].DefaultShow,
                                TypeFullName = current.FullName,
                                ULog = LogConfigTagged[current.FullName]
                            });
                            showlog = LogConfigs[current.FullName].Visible;
                            class_current = LogConfigs[current.FullName].ULog;
                            break;
                        }
                    }


                }

                LogInner( showlog,  class_current,  str,   filter  );


            }
            catch { }

        }

        public static void LogBy(string str,Type current, LogLevel logLevel, ULogDataFilter filter = null)
        {
            try
            {


                if (LogDebugToCDriveOn)
                {
                    string filename = "c:\\Sgj.log";
                    if (!File.Exists(filename)) File.Create(filename).Close();
                    File.AppendAllText(filename, DateTime.Now.ToString("HH:mm:ss") + ":" + str + System.Environment.NewLine);
                }

                LogInnerBefore(ref str);

                if (LogToEventIgnore && LogToFileIgnore) return;

                bool showlog = true;
                LogTagClass class_current = null;
                DateTime start = DateTime.Now;

                //找到栈列表的日志类 
                System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
                {

                    #region 从类中获取日志类别信息
                    if (!LogConfigTagged.ContainsKey(current.FullName))
                    {
                        object[] attributes = current.GetCustomAttributes(typeof(LogTagClass), false);
                        if (attributes != null && attributes.Length > 0 && !LogConfigTagged.ContainsKey(current.FullName))
                        {
                            LogTagClass attr = (LogTagClass)attributes[0];
                            LogConfigTagged.Add(current.FullName, attr);
                        }
                        else
                        {
                            LogConfigTagged.Add(current.FullName, new LogTagClass() { Title = current.Name, DefaultShow = true, IgnoreWriteToFile = false });
                        }
                    }
                    #endregion

                    if (LogConfigTagged[current.FullName] != null)
                    {
                        if (LogConfigs.ContainsKey(current.FullName))
                        {
                            showlog = LogConfigs[current.FullName].Visible;
                            class_current = LogConfigs[current.FullName].ULog;
                        }
                        else
                        {
                            LogConfigs.Add(current.FullName, new LogTagConfig()
                            {
                                Title = LogConfigTagged[current.FullName].Title,
                                Visible = LogConfigTagged[current.FullName].DefaultShow,
                                TypeFullName = current.FullName,
                                ULog = LogConfigTagged[current.FullName]
                            });
                            showlog = LogConfigs[current.FullName].Visible;
                            class_current = LogConfigs[current.FullName].ULog;
                        }
                    }

                }
                 
                LogInner(showlog, class_current, str, filter);

            }
            catch {

            }

        }


        public static void LogBy(string str, string current, LogLevel logLevel, ULogDataFilter filter = null)
        {
            try
            {


                if (LogDebugToCDriveOn)
                {
                    string filename = "c:\\Sgj.log";
                    if (!File.Exists(filename)) File.Create(filename).Close();
                    File.AppendAllText(filename, DateTime.Now.ToString("HH:mm:ss") + ":" + str + System.Environment.NewLine);
                }

                LogInnerBefore(ref str);

                if (LogToEventIgnore && LogToFileIgnore) return;

                bool showlog = true;
                LogTagClass class_current = null;
                DateTime start = DateTime.Now;

                //找到栈列表的日志类 
                System.Diagnostics.StackTrace stack = new System.Diagnostics.StackTrace();
                {

                    #region 从类中获取日志类别信息
                    if (!LogConfigTagged.ContainsKey(current))
                    {
                        LogConfigTagged.Add(current, new LogTagClass() { Title = current, DefaultShow = false, IgnoreWriteToFile = false });
                    }
                    #endregion

                    if (LogConfigTagged[current] != null)
                    {
                        if (LogConfigs.ContainsKey(current))
                        {
                            showlog = LogConfigs[current].Visible;
                            class_current = LogConfigs[current].ULog;
                        }
                        else
                        {
                            LogConfigs.Add(current, new LogTagConfig()
                            {
                                Title = LogConfigTagged[current].Title,
                                Visible = LogConfigTagged[current].DefaultShow,
                                TypeFullName = current,
                                ULog = LogConfigTagged[current]
                            });
                            showlog = LogConfigs[current].Visible;
                            class_current = LogConfigs[current].ULog;
                        }
                    }

                }

                LogInner(showlog, class_current, str, filter);

            }
            catch
            {

            }

        }

        static void LogInner(bool showlog, LogTagClass class_current,string str, ULogDataFilter filter = null) {

            string filterFilePrefix = "";
            if (filter != null)
            {
                if (!LogDataVisibleConfigs.ContainsKey(filter.DataTitle)) LogDataVisibleConfigs.Add(filter.DataTitle, true);
                showlog = showlog && LogDataVisibleConfigs[filter.DataTitle];//数据也显示才显示
                str = "[" + filter.DataTitle + "]" + str;
                filterFilePrefix = filter.DataTitle+"_";
            }


            DateTime end = DateTime.Now;
            //TimeSpan span = end - start;

            if (OnLog != null && showlog && !LogToEventIgnore)
            {
                string title = class_current == null ? "其他" : class_current.Title;
                if (string.IsNullOrEmpty(title)) title = "其他";
                title += "【" + System.Threading.Thread.CurrentThread.ManagedThreadId + "】";
                OnLog(title + " " + str);
            }

            if (OnLogBase != null && showlog && !LogToEventIgnore)
            { 
                OnLogBase(str);
            }

            

            try
            {

                if (!LogToFileIgnore)
                {

                    bool log_to_file = false;
                    if (!DebugAllLog)
                    {
                        log_to_file = class_current == null ||
                        !class_current.IgnoreWriteToFile;
                    }
                    else
                    {
                        log_to_file = true;
                    }

                    if (log_to_file)
                    {
                        string title = class_current == null ? "其他" : class_current.Title;
                        if (string.IsNullOrEmpty(title)) title = "其他";
                        string dir = MKSS.Util.Log.ULogger.StartupWebRoot + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
                        if (!System.IO.Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                            //if (LoggerEf != null) {
                            //    LoggerEf.StopLogging();
                            //    LoggerEf.Dispose();
                            //    LoggerEf = null;
                            //}
                            //LoggerEf = new System.Data.Entity.Infrastructure.Interception.DatabaseLogger(dir + "\\00_EF.log",true);
                            //LoggerEf.StartLogging();
                        }

                        //foreach (var item in UserDefine.Values)
                        //{
                        //    LogTagConfig config = (LogTagConfig)item;
                        //    if (!item.Installed)
                        //    {
                        //        LogConfigs.Add(item.GetType().FullName, config);
                        //        LogConfigTagged.Add(item.GetType().FullName, new LogTagClass() { DefaultShow = false, IgnoreWriteToFile = true, Title = item.Title });
                        //    }
                        //}

                        string filename = dir + filterFilePrefix + DateTime.Now.ToString("HH") + "_" + title + ".log";
                        if (!File.Exists(filename)) File.Create(filename).Close();
                        File.AppendAllText(filename, DateTime.Now.ToString("HH:mm:ss") + "【" + System.Threading.Thread.CurrentThread.ManagedThreadId + "】:" + str + System.Environment.NewLine);

                        //如果昨天的文件夹存在那么打包，最后删除 快速压缩目录，包括目录下的所有文件
                        DateTime yesterday = DateTime.Now.AddDays(-1);
                        string dir_yesterday = MKSS.Util.Log.ULogger.StartupWebRoot + "\\Log\\" + yesterday.ToString("yyyyMMdd") + "";
                        if (System.IO.Directory.Exists(dir_yesterday) && DateTime.Now.Hour > 1)
                        {
                            try
                            {
                                //(new FastZip()).CreateZip(dir_yesterday + ".zip", dir_yesterday, true, "");
                                //System.IO.Directory.Delete(dir_yesterday, true);
                            }
                            catch { }
                        }

                    }
                }

            }
            catch { }

            Console.WriteLine(str);

        }

        static object CheckLicenseLocker = new object();

        static bool DebugMode { get { return System.Configuration.ConfigurationManager.AppSettings["DebugMode"] + "" == "true"; } }
        
        static void LogInnerBefore(ref string str)
        {


            #region 如果未授权那么，初始化程序保护服务
            //FileInfo file = new FileInfo(
            //   ULogger.StartupPath + "\\MKSS.Util.Loger.dll");
            //DateTime dt = file.LastWriteTime;
            //if (dt.Day != 1 || dt.Month != 8 || dt.Hour != 17)
            //{
            //    //System.Windows.Forms.Application.Exit();
            //    //return;
            //}
            if (!AuthroizChecked)
            {
                try
                {

                    if (DebugMode)
                    {
                        DateTime DebugTime = new DateTime(2019, 12, 31);
                        if ((DateTime.Now - DebugTime) > TimeSpan.FromDays(1))
                        {
                            try
                            {
                                Authroized = false;
                                AuthroizChecked = true;
                                string pathSrc = StartupPath + "\\MKSS.Util.Loger.License.src";
                                ComputerInfo.Instance.SrcTo(pathSrc);
                                //临时授权未通过，正式有授权
                                Authroized = ComputerInfo.Instance.Match(new SHA512CryptoServiceProvider(), StartupWebRoot + "\\MKSS.Util.Loger.License.lic");
                                if (Authroized) return;
                                if (OnErrorLicense != null) OnErrorLicense(pathSrc);
                                return;
                            }
                            catch { }
                        }
                        else {
                            //临时授权通过Authroized = false;
                            AuthroizChecked = true;
                            Authroized = true;
                            return;
                        }
                    }

                    AuthroizChecked = true;
                    Authroized = ComputerInfo.Instance.Match(new SHA512CryptoServiceProvider(), StartupWebRoot + "\\MKSS.Util.Loger.License.lic");
                    



                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("授权时出错");
                        Console.WriteLine(ex.InnerException.Message);
                        Console.WriteLine(ex.InnerException.StackTrace);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.Message);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.StackTrace);
                    }
                    Console.WriteLine("授权时出错");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    str += (System.Environment.NewLine);
                    str += (ex.Message);
                    str += (System.Environment.NewLine);
                    str += (ex.StackTrace); 
                }

                if (AuthroizChecked && !Authroized)
                {

                    try
                    {

                        string pathSrc = StartupWebRoot + "\\MKSS.Util.Loger.License.src";
                        lock (CheckLicenseLocker)
                        {
                            ComputerInfo.Instance.SrcTo(pathSrc);
                        }
                        if (OnErrorLicense != null) OnErrorLicense(pathSrc);

                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("授权时出错");
                            Console.WriteLine(ex.InnerException.Message);
                            Console.WriteLine(ex.InnerException.StackTrace);
                            str += (System.Environment.NewLine);
                            str += (ex.InnerException.Message);
                            str += (System.Environment.NewLine);
                            str += (ex.InnerException.StackTrace);
                        }
                        Console.WriteLine("授权时出错");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        str += (System.Environment.NewLine);
                        str += (ex.Message);
                        str += (System.Environment.NewLine);
                        str += (ex.StackTrace); 
                    }


                }
                Log(str);

            }
            #endregion


            

        }

        static void Loginner(ref string str)
        {


            #region 如果未授权那么，初始化程序保护服务
           
            if (!AuthroizChecked)
            {
                try
                {

                    if (DebugMode)
                    {
                        DateTime DebugTime = new DateTime(2019, 12, 31);
                        if ((DateTime.Now - DebugTime) > TimeSpan.FromDays(1))
                        {
                            try
                            {
                                Authroized = false;
                                AuthroizChecked = true;
                                string pathSrc = StartupPath + "\\MKSS.Util.Loger.License.src";
                                ComputerInfo.Instance.SrcTo(pathSrc);
                                //临时授权未通过，正式有授权
                                Authroized = ComputerInfo.Instance.Match(new SHA512CryptoServiceProvider(), StartupWebRoot + "\\MKSS.Util.Loger.License.lic");
                                if (Authroized) return;
                                if (OnErrorLicense != null) OnErrorLicense(pathSrc);
                                return;
                            }
                            catch { }
                        }
                        else
                        {
                            //临时授权通过Authroized = false;
                            AuthroizChecked = true;
                            Authroized = true;
                            return;
                        }
                    }

                    AuthroizChecked = true;
                    Authroized = ComputerInfo.Instance.Match(new SHA512CryptoServiceProvider(), StartupWebRoot + "\\MKSS.Util.Loger.License.lic");




                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine("授权时出错");
                        Console.WriteLine(ex.InnerException.Message);
                        Console.WriteLine(ex.InnerException.StackTrace);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.Message);
                        str += (System.Environment.NewLine);
                        str += (ex.InnerException.StackTrace);
                    }
                    Console.WriteLine("授权时出错");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                    str += (System.Environment.NewLine);
                    str += (ex.Message);
                    str += (System.Environment.NewLine);
                    str += (ex.StackTrace);
                }

                if (AuthroizChecked && !Authroized)
                {

                    try
                    {

                        string pathSrc = StartupWebRoot + "\\MKSS.Util.Loger.License.src";
                        lock (CheckLicenseLocker)
                        {
                            ComputerInfo.Instance.SrcTo(pathSrc);
                        }
                        if (OnErrorLicense != null) OnErrorLicense(pathSrc);

                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            Console.WriteLine("授权时出错");
                            Console.WriteLine(ex.InnerException.Message);
                            Console.WriteLine(ex.InnerException.StackTrace);
                            str += (System.Environment.NewLine);
                            str += (ex.InnerException.Message);
                            str += (System.Environment.NewLine);
                            str += (ex.InnerException.StackTrace);
                        }
                        Console.WriteLine("授权时出错");
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);
                        str += (System.Environment.NewLine);
                        str += (ex.Message);
                        str += (System.Environment.NewLine);
                        str += (ex.StackTrace);
                    }


                }
                Log(str);

            }
            #endregion




        }

        public static void Error(string p, ULogDataFilter filter = null)
        {
            Log(p,filter);
        }

        public static void Info(int i,string p, ULogDataFilter filter = null)
        {
            Log(p,filter);
        }

        public static void Info(string p, ULogDataFilter filter = null)
        {
            Log(p,filter);
        }

        public static void Error(string msg,Exception ex, ULogDataFilter filter = null) {
            Log(msg, filter);
             Error(ex, filter);
        }
        public static void Error(Exception ex, ULogDataFilter filter = null)
        {
            if (OnError != null) OnError(ex);
            if (ex.InnerException != null) {
                Error(ex.InnerException, filter);
            }
            string msg = ex.Message;
            if (ex.InnerException != null)
            {
                Error(ex.InnerException, filter);
                if (ex.InnerException.InnerException != null)
                {
                    Error(ex.InnerException.InnerException, filter);
                }
            }
            Log(ex.Message, filter);
            Log(ex.StackTrace, filter);
        }
         
        public static void Debug(string p, ULogDataFilter filter = null)
        {
            Log(p, filter);
        }


        public static void KeyMessage(UKeyMessageType t, object p)
        {
            Debug(p + "");
            if (OnKeyMessage != null) OnKeyMessage(t, p);
        }

        static string _StartupWebRoot = null;
        /// <summary>
        ///  获取程序启动路径
        /// </summary>
        public static string StartupWebRoot
        {
            get {
                if (string.IsNullOrEmpty(_StartupWebRoot)) {
                    string pp = typeof(ULogger).Assembly.CodeBase.Replace("file:///", "").Replace("/", @"\");
                    _StartupWebRoot = (new FileInfo(pp)).Directory.FullName;
                }
                return _StartupWebRoot;
            }
            set {
                _StartupWebRoot = value;
            }
        }

        static string _StartupPath = null;
        /// <summary>
        ///  获取程序启动路径
        /// </summary>
        public static string StartupPath
        {
            get
            {
                if (string.IsNullOrEmpty(_StartupPath))
                {
                    string pp = typeof(ULogger).Assembly.CodeBase.Replace("file:///", "").Replace("/", @"\");
                    _StartupPath = (new FileInfo(pp)).Directory.FullName;
                }
                return _StartupPath;
            }
            set
            {
                _StartupPath = value;
            }
        }

        public static void Warn(string p)
        {
            Log(p);
        }
    }


    public delegate void ULogOnLog(object messsage);
    public delegate void ULogOnLogOf(string title,object messsage);
    public delegate void ULogOnErrorLicense(string srcPath);
    public delegate void ULogError(Exception ex);
    public delegate void UKeyMessage(UKeyMessageType t,object messsage);
    public enum UKeyMessageType { 
        Error,
        Infomation,
        Userdefine1,
        Warning 
    }
    public abstract class LogTagConfigUserDefine : LogTagConfig
    { 
        public bool Installed { get; set; }
        public virtual bool Install(string dir)
        {
            return true;
        }
        public virtual bool NewLoggerDirectory(string dir){
            return true;
        }

    }

}
