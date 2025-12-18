using System;
using SqlSugar;

namespace MKSS.Model
{
    [SugarTable("pd_env")]
    public class Env
    {
         
        /// <summary>
        /// ID
        /// </summary>		
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_EnvId { get; set; }

        /// <summary>
        /// 温度
        /// </summary>		
        public float F_Temperature { get; set; }

        /// <summary>
        ///  湿度
        /// </summary>		
        public float F_Humidity { get; set; }

        /// <summary>
        /// 时间
        /// </summary>		
        public DateTime F_DateTime { get; set; }
        /// <summary>
        /// 时间
        /// </summary>		
        public DateTime F_EndDateTime { get; set; }

        
    }
     

}