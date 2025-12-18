using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SqlSugar;

namespace MKSS.Model
{

    [SugarTable("pd_device", "mkssdbbiaoding")]
    public class Device
    {

        /// <summary>
        ///   设备主键
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public string F_Id { get; set; }


        /// <summary>
        ///   设备类别：0 无网络版本报警器，1 NB版本报警器，默认NB报警器
        /// </summary>		
        public int F_DeviceNetType { get; set; } = 1;
        /// <summary>
        /// Wings平台DeviceId
        /// </summary>		
        public string F_DeviceId { get; set; }


        /// <summary>
        /// Wings平台 IMEI
        /// </summary>		
        public string F_IMEI { get; set; }
        /// <summary>
        /// 流量卡卡号
        /// </summary>		
        public string F_IMSI { get; set; }
        /// <summary>
        /// 流量卡卡号
        /// </summary>		
        public string F_ICCID { get; set; }


        /// <summary>
        /// 传感器自动编号
        /// </summary>		
        public string F_SensorId { get; set; }

        /// <summary>
        ///   批次号
        /// </summary>		
        public long F_BatchId { get; set; }


        /// <summary>
        /// 产品序列号
        /// </summary>		
        public string F_SerialNO { get; set; }
        public DateTime F_SerialNOTime { get; set; }
        /// <summary>
        /// 产品接入验证码
        /// </summary>		
        public string F_SerialValidCode { get; set; }

        /// <summary>
        /// 物联网平台型号
        /// </summary>		
        public string F_ProductId { get; set; }

        /// <summary>
        /// 电信网平台型号
        /// </summary>		
        public string F_NBProductId { get; set; }

        /// <summary>
        /// 物联网平台型号
        /// </summary>		
        public string F_ProductName { get; set; }

        /// <summary>
        /// 电信网平台型号
        /// </summary>		
        public string F_NBProductName { get; set; }

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
        ///  状态，0 仅添加 1 已在WING平台登记，2 有数据但没有F_DeviceId或F_SerialNO信息，3 产品上线成功 ， -1 交付后从WING平台注销
        /// </summary>		
        public int F_Status { get; set; }
        public int F_PrintStatus { get; set; }
        public DateTime F_CreateDate { get; set; }  


        /// <summary>
        /// 创建时间 IMEI添加时间
        /// </summary>
        public DateTime F_PrintDate { get; set; } 
        /// <summary>
        /// 创建人编号
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string F_CreateUserName { get; set; }

        /// <summary>
        /// 完工时间 
        /// </summary>
        public DateTime F_FinishDate { get; set; }
        /// <summary>
        /// 完工人编号
        /// </summary>
        public string F_FinishUserId { get; set; }
        /// <summary>
        /// 完工人名称
        /// </summary>
        public string F_FinishUserName { get; set; }

        /// <summary>
        /// 第一次登记时间
        /// </summary>		
        public DateTime F_RegisterTime { get; set; }

        /// <summary>
        /// 最后一次收包时间
        /// </summary>		
        public DateTime F_LastRefreshTime { get; set; }

        /// <summary>
        /// 最后一次收包
        /// </summary>		
        public string F_LastPackage { get; set; }


        /// <summary>
        ///  装箱编号
        /// </summary>
        public string F_PackageNo { get; set; }
        /// <summary>
        ///  装箱时间
        /// </summary>
        public DateTime F_PackageTime { get; set; }
        /// <summary>
        ///  装箱顺序号
        /// </summary>
        public int F_PackageXh { get; set; }
        /// <summary>
        ///  发货时间
        /// </summary>
        public DateTime F_DeliverDate { get; set; }
        /// <summary>
        /// 发货批次
        /// </summary>	
        public string F_DeliverBatchNo { get; set; }

        public override string ToString()
        {
            return this.F_Id;
        }
    }
     
}