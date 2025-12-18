using System;
using System.Reflection;

namespace MKSS.Util.Loger
{
    /// <summary>
    /// 版 本 Hgjw-ADMS V7.0.0 某某敏捷开发框架
    /// Copyright (c) 2013-2018 某某软件信息技术有限公司
    /// 创建人：某某-框架开发组
    /// 日 期：2017.03.04
    /// 描 述：redis操作方法
    /// </summary>
    public class LogFactory
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static LogFactory()
        {

        }
        /// <summary>
        /// 获取日志操作对象
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static Log GetLogger(Type type)
        {
            return new Log(new ILog(type));
        }
        /// <summary>
        /// 获取日志操作对象
        /// </summary>
        /// <param name="str">名字</param>
        /// <returns></returns>
        public static Log GetLogger(string str)
        {
            return new Log(new ILog(str));
        }
    }

    public class LogManager
    {
        public static ILog GetLogger(Type type)
        {
            return new ILog(type);
        }

        public static ILog GetLogger(Assembly assembly, string str)
        {
            return new ILog(str);
        }
    }
}
