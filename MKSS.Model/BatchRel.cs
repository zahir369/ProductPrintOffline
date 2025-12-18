using SqlSugar;
using System;

namespace MKSS.Model
{
    [SugarTable("pd_batch_rel")]
    public class BatchRel
    {

        /// <summary>
        /// 主键，自增
        /// </summary>		
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_RelId { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>		
        public long F_BatchId { get; set; }

        /// <summary>
        /// 板子编号
        /// </summary>		
        public long F_BoardId { get; set; }
        /// <summary>
        ///  使用状态，1 使用中，2 空闲
        /// </summary>		
        public int F_UseStatus { get; set; }


        /// <summary>
        /// 最后使用开始时间
        /// </summary>		
        public DateTime F_LastUseStart { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>		
        public DateTime F_LastUseUpdate { get; set; }


        [SugarColumn(IsIgnore = true)]
        public EnumUseInFree EnumUseInFree { get { return (EnumUseInFree)F_UseStatus; } set { F_UseStatus = (int)value; } }
         
    }

}