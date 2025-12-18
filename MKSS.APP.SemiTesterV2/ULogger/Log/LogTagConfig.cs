using System;
using System.Collections.Generic;
using System.Text;

namespace MKSS.Util.Log
{

    /// <summary>
    /// 是否显示日志
    /// </summary>
    public class LogTagConfig
    {
        public string Title { get; set; }
        public bool Visible { get; set; }
        public string TypeFullName { get; set; }
        public LogTagClass ULog { get; set; }
    }
}
