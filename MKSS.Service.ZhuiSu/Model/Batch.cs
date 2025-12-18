using System;
using SqlSugar;

namespace MKSS.Model
{
    [SugarTable("pd_batch")]
    public class Batch
    {

         
        /// <summary>
        /// 批次号
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_BatchId { get; set; }


        /// <summary>
        /// 批次名称
        /// </summary>		
        public string F_BatchName { get; set; }

        /// <summary>
        /// 检测批次开始时间
        /// </summary>		
        public DateTime F_AgingStartTime { get; set; }

        /// <summary>
        /// 数据最后更新时间
        /// </summary>		
        public DateTime F_AgingLastUpdateTime { get; set; }

        /// <summary>
        /// 检测批次计划结束时间
        /// </summary>		
        public DateTime F_AgingEndTime { get; set; }
        /// <summary>
        /// 检测批次实际结束时间
        /// </summary>		
        public DateTime F_AgingEndTimeActual { get; set; }

        

        /// <summary>
        /// 检测状态，1 检测中，2 已结束
        /// </summary>		
        public int F_AgingStatus { get; set; }

        /// <summary>
        /// 采集间隔 秒
        /// </summary>		
        public double F_Intraval { get; set; }
        

        /// <summary>
        /// 传感器名称
        /// </summary>		
        public string F_SensorName { get; set; }
        /// <summary>
        /// 传感器名称
        /// </summary>		
        public string F_TestTimePoints { get; set; }
        
        /// <summary>
        /// 传感器类型
        /// </summary>		
        public string F_SensorTypeId { get; set; }


        /// <summary>
        /// 传感器类型名称(氨气，一氧化碳，硫化氢...等等)
        /// </summary>		
        public string F_SensorTypeName { get; set; }

         
        /// <summary>
        ///  备注
        /// </summary>		
        public string F_Memo { get; set; }

        /// <summary>
        ///  批次配置
        /// </summary>		
        public string F_XmlConfig { get; set; }

        /// <summary>
        ///  批次配置
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public ProductItem F_ProductConfig { 
            get {
                return ProductItem.FromXml(F_XmlConfig); 
            }  
        }



        [SugarColumn(IsIgnore = true)]
        public EnumAgingStatus EnumAgingStatus { get { return (EnumAgingStatus)F_AgingStatus; } set { F_AgingStatus = (int)value; } }

        [SugarColumn(IsIgnore = true)]
        public string EnumAgingStatusCN { 
            get {
                switch (EnumAgingStatus)
                {
                    case EnumAgingStatus.None:
                        return "未知";
                    case EnumAgingStatus.Finished:
                        return "完成";
                    case EnumAgingStatus.InAging:
                        return "检测中";
                    default:
                        break;
                }
                return "";
            } 
        }


        [SugarColumn(IsIgnore = true)]
        public string F_AgingStartTimeStr
        {
            get
            {
                return F_AgingStartTime.ToString("yy-MM-dd HH:mm");
            }
        }

        [SugarColumn(IsIgnore = true)]
        public string F_AgingEndTimeStr
        {
            get
            {
                if (EnumAgingStatus == EnumAgingStatus.InAging)
                {
                    return this.F_AgingEndTime.ToString("yy-MM-dd HH:mm");
                }
                return this.F_AgingEndTimeActual.ToString("yy-MM-dd HH:mm");
            }
        }

        [SugarColumn(IsIgnore = true)]
        public string F_AgingLastUpdateTimeStr
        {
            get
            { 
                return this.F_AgingLastUpdateTime.ToString("yy-MM-dd HH:mm");
            }
        }
        
        [SugarColumn(IsIgnore = true)]
        public string F_AgingLeftTime
        {
            get
            {
                if (EnumAgingStatus == EnumAgingStatus.InAging) {
                    TimeSpan t =(this.F_AgingEndTime - DateTime.Now);
                    return string.Format("{0}D {1}:{2}:{3}", t.Days, t.Hours, t.Minutes, t.Seconds);
                }
                return "";
            }
        }

    }

    /// <summary>
    ///  检测状态，1 检测中，2 已结束
    /// </summary>
    public enum EnumAgingStatus
    {
        None = 0, Finished = 2, InAging = 1
    }


}