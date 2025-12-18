using System;
using SqlSugar;

namespace MKSS.Model
{
    [SugarTable("pd_batch_condition")]
    public class BatchCondition
    {

        /// <summary>
        /// 批次号
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_ConditionId { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>		

        public long F_BatchId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>		
        public double F_StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>		
        public double F_EndTime { get; set; }

        /// <summary>
        /// 温度
        /// </summary>		
        public double F_Temperature { get; set; }

        /// <summary>
        /// 湿度
        /// </summary>		
        public double F_Humidity { get; set; }

        public double F_PAR1 { get; set; }
        public double F_PAR2 { get; set; }
        public double F_PAR3 { get; set; }
        public double F_PAR4 { get; set; }
        public double F_PAR5{ get; set; }
        public double F_PAR6 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>		
        public string F_Memo { get; set; }



    }


}