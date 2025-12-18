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
        /// 标定参考时间
        /// </summary>		
        public double F_SpanTime { get; set; }

        /// <summary>
        /// 标定参考浓度
        /// </summary>		
        public double F_SpanValue { get; set; }

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
        ///  检测批次计划结束时间，秒数
        ///  改存 FullQueryTimes
        /// </summary>		
        public double F_AgingEndTime { get; set; }
        /// <summary>
        /// 检测批次实际结束时间
        /// </summary>		
        public DateTime F_AgingEndTimeActual { get; set; }

        

        /// <summary>
        /// 检测状态，1 检测中，2 已结束，3 已暂停
        /// </summary>		
        public int F_AgingStatus { get; set; }
         


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
        ///  备注
        /// </summary>		
        public string F_Memo { get; set; }
         
        [SugarColumn(IsIgnore = true)]
        public EnumAgingStatus EnumAgingStatus { get { return (EnumAgingStatus)F_AgingStatus; } set { F_AgingStatus = (int)value; } }

        [SugarColumn(IsIgnore = true)]
        public string EnumAgingStatusCN { 
            get {
                switch (EnumAgingStatus)
                {
                    case EnumAgingStatus.None:
                        return "未知";
                    case EnumAgingStatus.InAgingPaused:
                        return "暂停";
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
        public int FullQueryTimes
        {
            get { return (int)this.F_AgingEndTime; }
            set { this.F_AgingEndTime = value; }
        }
        [SugarColumn(IsIgnore = true)]
        public string FullQueryTimesCN
        {
            get { return FullQueryTimes.ToString("000000"); }
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
        public string F_AgingEndTimeActualStr
        {
            get
            {
                if (F_AgingEndTimeActual == DateTime.MinValue)
                {
                    return "";
                }
                return this.F_AgingEndTimeActual.ToString("yy-MM-dd HH:mm");
            }
        }

        [SugarColumn(IsIgnore = true)]
        public string F_SpanTimeStr
        {
            get
            {
                if (this.F_SpanTime == 0) return "";
                TimeSpan d = TimeSpan.FromSeconds(this.F_SpanTime);
                return string.Format("{0}天{1}时{2}分{3}秒", d.Days, d.Hours, d.Minutes, d.Seconds);
            }
        }
        [SugarColumn(IsIgnore = true)]
        public string F_SpanValueStr
        {
            get
            {
                if (this.F_SpanValue == 0) return "";
                return F_SpanValue.ToString("f1");
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
        

    }

    /// <summary>
    ///  检测状态，1 检测中，2 已结束
    /// </summary>
    public enum EnumAgingStatus
    {
        None = 0, Finished = 2, InAging = 1, InAgingPaused = 3
    }

}