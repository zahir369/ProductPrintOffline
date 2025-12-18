using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


	[SugarTable("pd_dataitem")]	
	public class DataItem
	{


        /// <summary>
        /// F_ItemId
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_ItemId { get; set; }  
        
        
				
		/// <summary>
		/// F_ItemName
        /// </summary>		
        public string F_ItemName { get; set; }


        /// <summary>
        /// F_ItemCode
        /// </summary>		
        public string F_ItemCode { get; set; }




        /// <summary>
        /// F_ParentId
        /// </summary>		


        public long F_ParentId { get; set; }  
        
        
				
		/// <summary>
		/// F_SortCode
        /// </summary>		
        
	 
        public int F_SortCode { get; set; }  
        
        
		   
	}
}