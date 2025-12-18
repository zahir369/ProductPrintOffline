using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


	[SugarTable("pd_boardcase")]	
	public class BoardCase
	{

        /// <summary>
        /// 老化架或老化柜(存放托板的容器)
        /// </summary>		
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_BoardCaseId { get; set; }  
				
		/// <summary>
		/// 老化架地址码
        /// </summary>		
        public int F_BoardCaseAddress { get; set; }  
				
		/// <summary>
		/// 老化架名称
        /// </summary>		
        public string F_BoardCaseName { get; set; }  
				
		/// <summary>
		/// 摆放位置
        /// </summary>		
        public string F_BoardCasePostion { get; set; }  
				
		/// <summary>
		/// 所属数据总线编号
        /// </summary>		
        public long F_DataBusId { get; set; }
         

        /// <summary>
        ///  使用状态，1 使用中，2 空闲
        /// </summary>		
        public int F_UseStatus { get; set; }
        /// <summary>
        /// 最后使用时间
        /// </summary>		
        public DateTime F_LastUseUpdate { get; set; }

        [SugarColumn(IsIgnore = true)]
        public EnumUseInFree EnumUseInFree { get { return (EnumUseInFree)F_UseStatus; } set { F_UseStatus = (int)value; } }

        public override string ToString()
        {
            return string.Format("{0}#", this.F_BoardCaseAddress );
        }

    }

    /// <summary>
    ///  使用状态，1 使用中，2 空闲
    /// </summary>
    public enum EnumUseInFree { 
        None = 0,Free =2 , InUse =1
    }
    /// <summary>
    ///  老化、标定，1 老化，2标定
    /// </summary>
    public enum EnumUseType
    {
        None = 0, BiaoDing =2, LaoHua =1
    }

}