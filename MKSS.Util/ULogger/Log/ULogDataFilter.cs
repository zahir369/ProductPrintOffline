using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MKSS.Util.Loger
{
    /// <summary>
    ///  根据数据过滤数据
    /// </summary>
    public class ULogDataFilter
    {
        /// <summary>
        ///  主类别
        /// </summary>
        public virtual string Main { get { return "过滤器"; } }
        /// <summary>
        ///  分类名称
        /// </summary>
        public virtual string DataTitle { get { return "默认"; } }
        /// <summary>
        ///  用户对象
        /// </summary>
        public virtual object Tag { get; set; }
    }
}
