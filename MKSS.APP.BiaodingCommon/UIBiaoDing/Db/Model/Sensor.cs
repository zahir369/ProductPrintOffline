using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;
using MKSS.APP.UIBiaoDing.Config;

namespace MKSS.Model
{


	[SugarTable("pd_sensor")]	
	public class Sensor
	{

        /// <summary>
        /// 传感器自动编号
        /// </summary>		
        [SugarColumn(IsNullable = false, IsPrimaryKey = true )]
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
        public int F_BoardId { get; set; }
        /// <summary>
        /// 所在数据板编号 
        /// </summary>		
        public int F_BoardCaseId { get; set; }
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
        public string F_GasTunnel { get; set; }

        public string V_LiangCheng { get; set; }
        public string V_VoltageRange { get; set; }
        public string V_AutoAdjustStatus { get; set; }
        public string V_SerialNo { get; set; }
        public string V_DeviceDateTime { get; set; }
        public string V_OutPutVolage { get; set; }
        public string V_HEGE { get; set; }
        public DateTime V_HEGE_TIME { get; set; }

        /// <summary>
        /// 标定时间
        /// </summary>	
        public DateTime F_BD_ZERO_TIME { get; set; }
        /// <summary>
        /// 标定时间
        /// </summary>	
        public DateTime F_BD_SPAN_TIME { get; set; }
        /// <summary>
        /// 标定时间时间
        /// </summary>	
        public DateTime F_BD_TIME_TIME { get; set; }

        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }
        public int V4 { get; set; }
        public int V5 { get; set; }
        public int V6 { get; set; }
        public int V7 { get; set; }
        public int V8 { get; set; }
        public int V9 { get; set; }
        public int V10 { get; set; }

        /// <summary>
        /// F_AddTime  
        /// </summary>		
        public DateTime F_AddTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool? IsChecked { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string PosString { get { return string.Format("{0}{1}", F_BoardId, F_SlotNO); } }
        public string CreateCensorIdLike() { return String.IsNullOrEmpty(F_SensorId) ? null : F_SensorId.Substring(0, F_SensorId.Length - 3); }
        public int V_HEGE_INT() { return (V_HEGE + "").ToLower() == (false).ToString() ? 0 : 1; }
        [SugarColumn(IsIgnore = true)]
        public bool IsEmpData
        {
            get { return ( this.V1 == 0|| this.V1 == 170) && (this.V2 == 0 || this.V2 == 170); }
        }

        public static string CreateCensorId(long batch_no, int caseNo, int boardNO, int posNo)
        {
            return (string.Format("{0}{1}{2}{3}{4}", batch_no, caseNo.ToString("00"), boardNO.ToString("00"), posNo.ToString("00"), ((int)BiaoDingConfgig.Instance.GasTunnel).ToString("000")));
        }
        public static string CreateCensorIdLike(long batch_no, int caseNo, int boardNO, int posNo)
        {
            return (string.Format("{0}{1}{2}{3}", batch_no, caseNo.ToString("00"), boardNO.ToString("00"), posNo.ToString("00")));
        }
        public static string CreateCensorIdLike(long batch_no, int caseNo, int boardNO)
        {
            return (string.Format("{0}{1}{2}", batch_no, caseNo.ToString("00"), boardNO.ToString("00")));
        }
    }

}