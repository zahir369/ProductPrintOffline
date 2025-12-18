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
		/// 所在数据板编号，A - D
        /// </summary>		
        public string F_BoardId { get; set; }
        /// <summary>
        /// 插槽序号
        /// </summary>		
        public int F_SlotNO { get; set; }

        /// <summary>
        ///  产品序列号
        /// </summary>		
        public string F_SerialNO { get; set; }

        /// <summary>
        /// 芯片串号
        /// </summary>		
        public string F_ICNO { get; set; }


        [SugarColumn(IsIgnore = true)]
        public bool? IsChecked { get; set; }
        /// <summary>
        ///  显示标签名字
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public string PosString { get { return string.Format("{0}{1}", F_BoardId, (F_SlotNO * 2 - 1).ToString("00")); } }
        /// <summary>
        ///  显示顺序号
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int VisibleXh { get { return (F_SlotNO % 2 == 0 ? 8 : 0) + F_SlotNO / 2; } }


        public static string CreateCensorId(long batch_no, string boardNO, int posNo)
        {
            return (string.Format("{0}{1}{2}", batch_no, boardNO , posNo.ToString("00")));
        }

        public override string ToString()
        {
            return (string.Format("{0}-{1}", this.F_SensorId, PosString));
        }

    }
}