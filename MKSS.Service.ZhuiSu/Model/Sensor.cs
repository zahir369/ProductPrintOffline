using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


	[SugarTable("pd_sensor")]	
	public class Sensor
	{

        /// <summary>
        /// 传感器自动编号
        /// </summary>		
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string F_SensorId { get; set; }

        /// <summary>
        /// 检测批次号
        /// </summary>		
        public long F_BatchId { get; set; }

        /// <summary>
        /// 传感器名称
        /// </summary>		
        public string F_SensorName { get; set; }  
				
		/// <summary>
		/// 传感器类型
        /// </summary>		
        public string F_SensorTypeId { get; set; }  
        
				
		/// <summary>
		/// 传感器类型名称(氨气，一氧化碳，硫化氢...等等)
        /// </summary>		
        public string F_SensorTypeName { get; set; }  
         
		/// <summary>
		/// 所在数据板编号
        /// </summary>		
        public long F_BoardId { get; set; }  
         
		/// <summary>
		/// 所在架、柜编号
        /// </summary>		
        public long F_BoardCaseId { get; set; }

        /// <summary>
        /// 产品序列号
        /// </summary>		
        public string F_SerialNO { get; set; }

        /// <summary>
        /// 芯片串号
        /// </summary>		
        public string F_ICNO { get; set; }
        /// <summary>
        /// 插槽序号
        /// </summary>		
        public int F_SlotNO { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string PosString { get { 
                return string.Format("{0}{1}", IsA ? "A":"B", F_SlotNO.ToString("00")); } }
        [SugarColumn(IsIgnore = true)]
        public bool IsA { get { return F_BoardId % 10==1; } }

        public static string CreateCensorId(long batch_no, int caseNO, int floorNO, int posNo)
        {
            return (string.Format("{0}{1}{2}{3}", batch_no, caseNO.ToString("00"), floorNO.ToString("0"), posNo.ToString("00")));
        }

    }
}