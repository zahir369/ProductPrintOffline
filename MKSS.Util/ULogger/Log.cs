using System;
namespace MKSS.Util.Loger
{
    /// <summary>
    /// 版 本 Hgjw-ADMS V7.0.0 某某敏捷开发框架
    /// Copyright (c) 2013-2018 某某软件信息技术有限公司
    /// 创建人：某某-框架开发组
    /// 日 期：2017.03.04
    /// 描 述：日志
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志实体类
        /// </summary>
        private ILog logger;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="log">日志操作对象</param>
        public Log(ILog log)
        {
            this.logger = log;
        }
        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Debug(object message, ULogDataFilter filter)
        {
            this.logger.Debug(message, filter);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Error(object message, ULogDataFilter filter)
        {
            this.logger.Error(message, filter);
        }
        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Info(object message, ULogDataFilter filter)
        {
            this.logger.Info(message, filter);
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Warn(object message, ULogDataFilter filter)
        {
            this.logger.Warn(message, filter);
        }
        #region 过滤器

        /// <summary>
        /// 调试日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Debug(object message )
        {
            this.logger.Debug(message);
        }
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Error(object message )
        {
            this.logger.Error(message);
        }
        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Info(object message )
        {
            this.logger.Info(message);
        }
        /// <summary>
        /// 警告日志
        /// </summary>
        /// <param name="message">消息</param>
        public void Warn(object message)
        {
            this.logger.Warn(message);
        }
        #endregion
    }
}
