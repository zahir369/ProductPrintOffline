using System;

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    [TableAttribute(PrimaryKey = "F_Id")]
    public class pd_device
    {

        /// <summary>
        ///   设备主键
        /// </summary>		
        [ColumnAttribute()]
        public string F_Id { get; set; }


        /// <summary>
        /// Wings平台DeviceId
        /// </summary>		
        [ColumnAttribute()]
        public string F_DeviceId { get; set; }


        /// <summary>
        /// Wings平台 IMEI
        /// </summary>	
        [ColumnAttribute()]
        public string F_IMEI { get; set; }
        /// <summary>
        /// 流量卡卡号
        /// </summary>	
        [ColumnAttribute()]
        public string F_IMSI { get; set; }
        /// <summary>
        /// 流量卡卡号
        /// </summary>	
        [ColumnAttribute()]
        public string F_ICCID { get; set; }

        /// <summary>
        /// 产品序列号
        /// </summary>	
        [ColumnAttribute()]
        public string F_SerialNO { get; set; }
        /// <summary>
        /// 产品接入验证码
        /// </summary>		
        [ColumnAttribute()]
        public string F_SerialValidCode { get; set; }

        /// <summary>
        /// 物联网平台型号
        /// </summary>	
        [ColumnAttribute()]
        public string F_ProductId { get; set; }

        /// <summary>
        /// 电信网平台型号
        /// </summary>		
        [ColumnAttribute()]
        public string F_NBProductId { get; set; }

        /// <summary>
        /// 物联网平台型号
        /// </summary>	
        [ColumnAttribute()]
        public string F_ProductName { get; set; }

        /// <summary>
        /// 电信网平台型号
        /// </summary>	
        [ColumnAttribute()]
        public string F_NBProductName { get; set; }


        /// <summary>
        /// 创建时间 IMEI添加时间
        /// </summary>
        [ColumnAttribute()]
        public DateTime F_CreateDate { get; set; }  
        [ColumnAttribute()]
        public DateTime F_RegisterTime { get; set; } 

        /// <summary>
        ///  状态，0 仅添加 1 已在WING平台登记，2 有数据但没有F_DeviceId或F_SerialNO信息，3 产品上线成功 ， -1 交付后从WING平台注销
        /// </summary>	
        [ColumnAttribute()]
        public int F_Status { get; set; }

        /// <summary>
        ///  打印时间
        /// </summary>
        [ColumnAttribute()]
        public DateTime F_PrintDate { get; set; }
        /// <summary>
        ///  发货状态，0 未打印，1 已打印 ，2 已装箱，3 已发货
        /// </summary>
        [ColumnAttribute()]
        public int F_PrintStatus { get; set; }
        /// <summary>
        ///  装箱编号
        /// </summary>
        [ColumnAttribute()]
        public string F_PackageNo { get; set; }
        /// <summary>
        ///  装箱时间
        /// </summary>
        [ColumnAttribute()]
        public DateTime F_PackageTime { get; set; }
        /// <summary>
        ///  装箱顺序号
        /// </summary>
        [ColumnAttribute()]
        public int F_PackageXh { get; set; }
        
        /// <summary>
        /// 最后一次收包时间
        /// </summary>		
        [ColumnAttribute()]
        public DateTime F_LastRefreshTime { get; set; }

        /// <summary>
        ///  标定是否合格，1 合格，其他 不良品
        /// </summary>
        [ColumnAttribute()]
        public int F_BD_HEGE { get; set; }


        public DeviceStatus DeviceStatus { get { return (DeviceStatus)F_Status; } set { F_Status = (int)value; } }
        public DeliverStatus DeliverStatus { get { return (DeliverStatus)F_PrintStatus; } set { F_PrintStatus = (int)value; } }
        

        string DeviceStatusStr { get { return ((DeviceStatus)F_Status) + ""; } }


        public string DeliverStatusCN
        {
            get
            {
                switch (DeliverStatus)
                {
                    case DeliverStatus.NoPrint:
                        return "未打印";
                    case DeliverStatus.Printed:
                        return "已打印";
                    case DeliverStatus.Packaged:
                        return "已装箱";
                    case DeliverStatus.Deliverd:
                        return "已发货";
                    default:
                        break;
                }
                return "";
            }
        }
        public string DeviceStatusCN
        {
            get
            {
                switch (DeviceStatus)
                {
                    case DeviceStatus.None:
                        return "未入网登记";
                    case DeviceStatus.Register:
                        return "已入网登记";
                    case DeviceStatus.RegisterFaild:
                        return "入网登记失败";
                    case DeviceStatus.UnRegister:
                        return "已出网登记";
                    case DeviceStatus.UnRegisterFaild:
                        return "未完成，出网登记失败";
                    case DeviceStatus.OnlineNoData:
                        return "上线异常";
                    case DeviceStatus.OnlineSucess:
                        return "上线成功";
                    case DeviceStatus.OnlineSucessAlarm:
                        return "气体超标";
                    case DeviceStatus.OnlineSucessTestAlarm:
                        return "报警测试";
                    case DeviceStatus.OnlineSucessDeviceError:
                        return "已上线，故障";
                    case DeviceStatus.RegisterSoldDevice:
                        return "已售设备";
                    case DeviceStatus.RegisterSoldDeviceError:
                        return "已售设备失败";
                    default:

                        break;
                }
                return "";
            }
        }


    }


}
