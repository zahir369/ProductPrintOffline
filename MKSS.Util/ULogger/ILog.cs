using MKSS.Util.Loger ;
using MKSS.Util.Log;
using System;

namespace MKSS.Util.Loger
{
    public class ILog
    {
        private Type type;
        private string str;

        public ILog(Type type)
        {
            this.type = type;
        }

        public ILog(string str)
        {
            this.str = str;
        }

        public void Debug(object message) {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Debug);
            }
            else {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Debug);
            }
        }
        public void Error(object message) {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error);
            }
        }
        public void Info(object message) {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Info);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Info);
            }
        }
        public void Warn(object message) {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Warning);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Warning);
            }
        }

        public void DebugFormat(string v1, params object[] v2)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Debug);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Debug);
            }
        }

        public void InfoFormat(string v1, params object[] v2)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Info);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Info);
            }
        }

        public void Error(string message, Exception e)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error);
                ULogger.LogBy(e.Message, type, LogLevel.Error);
                ULogger.LogBy(e.StackTrace, type, LogLevel.Error);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error);
                ULogger.LogBy(e.Message, str, LogLevel.Error);
                ULogger.LogBy(e.StackTrace, str, LogLevel.Error);
            }
        }

        public void ErrorFormat(string v1, params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Error);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Error);
            }
        }

        public void WarnFormat(string v1, params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Warning);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Warning);
            }
        }

        #region 带过滤器

        public void Debug(object message, ULogDataFilter filter)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Debug, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Debug, filter);
            }
        }
        public void Error(object message, ULogDataFilter filter)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error, filter);
            }
        }
        public void Info(object message, ULogDataFilter filter)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Info, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Info, filter);
            }
        }
        public void Warn(object message, ULogDataFilter filter)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Warning, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Warning, filter);
            }
        }

        public void DebugFormat(string v1, ULogDataFilter filter, params object[] v2)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Debug, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Debug, filter);
            }
        }

        public void InfoFormat(string v1, ULogDataFilter filter, params object[] v2)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Info, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Info, filter);
            }
        }

        public void Error(string message, Exception e, ULogDataFilter filter)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error, filter);
                ULogger.LogBy(e.Message, type, LogLevel.Error, filter);
                ULogger.LogBy(e.StackTrace, type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error, filter);
                ULogger.LogBy(e.Message, str, LogLevel.Error, filter);
                ULogger.LogBy(e.StackTrace, str, LogLevel.Error, filter);
            }
        }

        public void ErrorFormat(string v1, ULogDataFilter filter, params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Error, filter);
            }
        }

        public void WarnFormat(string v1, ULogDataFilter filter, params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Warning, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Warning, filter);
            }
        }
        #endregion



        #region 带过滤器

        public void Debug(ULogDataFilter filter, object message )
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Debug, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Debug, filter);
            }
        }
        public void Error(ULogDataFilter filter,object message )
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error, filter);
            }
        }
        public void Info(ULogDataFilter filter, object message)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Info, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Info, filter);
            }
        }
        public void Warn(ULogDataFilter filter,object message)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Warning, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Warning, filter);
            }
        }

        public void DebugFormat(ULogDataFilter filter, string v1,  params object[] v2)
        {
            //if (type != null)
            //{
            //    ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Debug, filter);
            //}
            //else
            //{
            //    ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Debug, filter);
            //}
        }

        public void InfoFormat(ULogDataFilter filter, string v1,params object[] v2)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, v2), type, LogLevel.Info, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, v2), str, LogLevel.Info, filter);
            }
        }

        public void Error(ULogDataFilter filter,string message, Exception e)
        {
            if (type != null)
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), type, LogLevel.Error, filter);
                ULogger.LogBy(e.Message, type, LogLevel.Error, filter);
                ULogger.LogBy(e.StackTrace, type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(message == null ? "" : message.ToString(), str, LogLevel.Error, filter);
                ULogger.LogBy(e.Message, str, LogLevel.Error, filter);
                ULogger.LogBy(e.StackTrace, str, LogLevel.Error, filter);
            }
        }

        public void ErrorFormat(ULogDataFilter filter, string v1,  params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Error, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Error, filter);
            }
        }

        public void WarnFormat(ULogDataFilter filter, string v1,  params object[] message)
        {
            if (type != null)
            {
                ULogger.LogBy(string.Format(v1, message), type, LogLevel.Warning, filter);
            }
            else
            {
                ULogger.LogBy(string.Format(v1, message), str, LogLevel.Warning, filter);
            }
        }
        #endregion

    }


    /// <summary>
    ///  消息历史
    /// </summary>
    [LogTagClass(Title = "消息历史", DefaultShow = false)]
    public class HydJobMessageLogger
    {
      
    }


    /// <summary>
    ///  消息历史
    /// </summary>
    [LogTagClass(Title = "关键消息", DefaultShow = false)]
    public class HydJobWriteMessageLogger
    {
       
    }


}