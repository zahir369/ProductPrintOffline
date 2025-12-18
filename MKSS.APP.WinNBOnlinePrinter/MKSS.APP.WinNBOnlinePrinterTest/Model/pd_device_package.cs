using System;

namespace MKSS.APP.WinNBOnlinePrinterTest
{

    [TableAttribute(PrimaryKey = "F_PackageNo")]
    public  class pd_device_package
    {

        /// <summary>
        ///   装箱编号
        /// </summary>		
        [ColumnAttribute()]
        public string F_PackageNo { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>		
        [ColumnAttribute()]
        public DateTime F_CreateDate { get; set; }

        /// <summary>
        /// 设备数量
        /// </summary>	
        [ColumnAttribute()]
        public int F_DeviceCount { get; set; }
        /// <summary>
        /// 发货批次
        /// </summary>	
        [ColumnAttribute()]
        public string F_DeliverBatchNo { get; set; }

        /// <summary>
        /// 发货时间
        /// </summary>		
        [ColumnAttribute()]
        public DateTime F_DeliverDate { get; set; }


        /// <summary>
        /// 发货类型，1 整箱 2 单品
        /// </summary>		
        [ColumnAttribute()]
        public int F_PackageType { get; set; }

        /// <summary>
        /// 发货当前箱或单品序号
        /// </summary>		
        [ColumnAttribute()]
        public int F_PackageIndex { get; set; }

    }

     
}
