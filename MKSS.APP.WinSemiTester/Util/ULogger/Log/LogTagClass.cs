using System;
using System.Collections.Generic;
using System.Text;

namespace MKSS.Util.Log
{

    /// <summary>
    /// 类注释,现用于日志标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class LogTagClass : Attribute
    {
        /// <summary>
        ///  名称
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        ///  默认显示
        /// </summary>
        public bool DefaultShow { get; set; }
        /// <summary>
        ///  不写入文件
        /// </summary>
        public bool IgnoreWriteToFile { get; set; }

        public LogTagClass() {
            DefaultShow = true;
        }

    }


}
