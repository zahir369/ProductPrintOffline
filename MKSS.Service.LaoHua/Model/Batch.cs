using System;
using System.Collections.Generic;
using SqlSugar;

namespace MKSS.Model
{


    [SugarTable("pd_batch")]
    public class BatchWithXml {
        /// <summary>
        /// 批次号
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_BatchId { get; set; }

        /// <summary>
        /// 统计结果XML，为了避免查询过慢，这里不读取显示统计内容
        /// </summary>		
        public string F_STA_XML { get; set; }
    }


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
        /// 老化批次开始时间
        /// </summary>		
        public DateTime F_AgingStartTime { get; set; }

        /// <summary>
        /// 数据最后更新时间
        /// </summary>		
        public DateTime F_AgingLastUpdateTime { get; set; }

        /// <summary>
        /// 老化批次计划结束时间
        /// </summary>		
        public DateTime F_AgingEndTime { get; set; }
        /// <summary>
        /// 老化批次实际结束时间
        /// </summary>		
        public DateTime F_AgingEndTimeActual { get; set; }

        

        /// <summary>
        /// 老化状态，1 老化中，2 已结束
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
        /// 信号AD基准值
        /// </summary>		
        public int F_ADBase { get; set; }
        /// <summary>
        /// 信号AD上浮动
        /// </summary>		
        public int F_ADAdd { get; set; }
        /// <summary>
        /// 信号AD下浮动
        /// </summary>		
        public int F_ADMinus { get; set; }

        /// <summary>
        ///  备注
        /// </summary>		
        public string F_Memo { get; set; }


        /// <summary>
        /// 统计时间
        /// </summary>		
        public DateTime F_STA_TIME { get; set; }

        /// <summary>
        /// 统计结果XML，为了避免查询过慢，这里不读取显示统计内容
        /// </summary>		
        //public string F_STA_XML { get; set; }

        /// <summary>
        /// 统计时老化状态，1 老化中，2 已结束
        /// </summary>		
        public int F_STA_AgingStatus { get; set; }

        /// <summary>
        ///  生产任务单ID
        /// </summary>
        public string F_SCRWD_OrderID { get; set; }
        /// <summary>
        ///  生产任务单编号
        /// </summary>
        public string F_SCRWD_OrderNumber { get; set; }
        /// <summary>
        ///  生产任务产品代码
        /// </summary>
        public string F_SCRWD_ProductCode { get; set; }
        /// <summary>
        ///  生产任务产品名称
        /// </summary>
        public string F_SCRWD_ProductFullName { get; set; }
        /// <summary>
        ///  生产任务类别编码
        /// </summary>
        public string F_SCRWD_ProductType { get; set; }
        /// <summary>
        ///  生产任务所属部门
        /// </summary>
        public string F_SCRWD_Department { get; set; }
        /// <summary>
        ///  生产任务数量
        /// </summary>
        public int F_SCRWD_Qty { get; set; }
        /// <summary>
        ///  生产任务下发日期
        /// </summary>
        public DateTime F_SCRWD_Date { get; set; }
     

        [SugarColumn(IsIgnore = true)]
        public EnumAgingStatus EnumF_STA_AgingStatus { get { return (EnumAgingStatus)F_STA_AgingStatus; } set { F_STA_AgingStatus = (int)value; } }

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
                        return "老化中"; 
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
    ///  老化状态，1 老化中，2 已结束
    /// </summary>
    public enum EnumAgingStatus
    {
        None = 0,  InAging = 1, Finished = 2,  
    }

    /// <summary>
    ///  0 未生成编号需要全部重编  1 已生成过编号不需要产生编号 2 再次对无效编号生成 其他 未生成 
    /// </summary>
    public enum EnumSerialCoded
    {
        /// <summary>
        ///  未生成编号需要全部重编
        /// </summary>
        CodeAll = 0,
        /// <summary>
        ///  已生成过编号不需要产生编号
        /// </summary>
        CodeFinised = 1,
        /// <summary>
        ///  再次对无效编号生成
        /// </summary>
        CodeInvalid = 2,
    }
    /// <summary>
    ///  0 未校准时间需要全部重校准  1 已校准过时间不需要校准时间 2 再次对无效时间校准 其他 未生成
    ///  已生成过校准过时间， 1 已生成 其他 未生成
    /// </summary>
    public enum EnumDeviceTimed
    {
        /// <summary>
        ///  未校准时间需要全部重校准
        /// </summary>
        CodeAll = 0,
        /// <summary>
        ///  已校准过时间不需要校准时间
        /// </summary>
        CodeFinised = 1,
        /// <summary>
        ///  再次对无效时间校准
        /// </summary>
        CodeInvalid = 2,
    }
    

}