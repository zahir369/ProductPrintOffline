using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


	[SugarTable("pd_dataitemdetail")]	
	public class DataItemDetail
	{


        /// <summary>
        /// F_DetailId
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_DetailId { get; set; }  
        
        
				
		/// <summary>
		/// F_ItemId
        /// </summary>		
        
	 
        public long F_ItemId { get; set; }  
        
        
				
		/// <summary>
		/// F_ParentId
        /// </summary>		
        
	 
        public long F_ParentId { get; set; }  
        
        
				
		/// <summary>
		/// F_DetailCode
        /// </summary>		
        
	 
        public string F_DetailCode { get; set; }  
        
        
				
		/// <summary>
		/// F_DetailName
        /// </summary>		
        
	 
        public string F_DetailName { get; set; }  
        
        
				
		/// <summary>
		/// F_DetailValue
        /// </summary>		
        
	 
        public string F_DetailValue { get; set; }  
        
        
		   
	}
}