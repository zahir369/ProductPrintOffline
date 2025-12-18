using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using SqlSugar;
using MKSS.Core.Model.BaseEntity;

namespace MKSS.Core.Model.SysBase
{
    /// <summary>
    /// 第三方应用管理
    /// </summary>
    [SugarTable("base_thirdpartyapp")]
    public class BaseThirdPartyApp: EntityBase<string>
    {
        /// <summary>
        /// 应用编码(不重复)
        /// </summary>
        public string F_AppCode { get; set; }
        /// <summary>
        /// AppKey
        /// </summary>
        public string F_AppKey { get; set; }
        /// <summary>
        /// AppSecret
        /// </summary>
        public string F_AppSecret { get; set; }
        /// <summary>
        /// Token
        /// </summary>
        public string F_Token { get; set; }
        /// <summary>
        /// API地址
        /// </summary>
        public string F_ApiUrl { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime F_ExpireDate { get; set; }
        /// <summary>
        /// 自增列
        /// </summary>
        [SugarColumn(IsIgnore =true)]
        public long F_AutoId { get; set; }
    }
}
