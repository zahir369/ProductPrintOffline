using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.WinNBOnlinePrinterTest.Model
{
    public class TaskModel
    {
        //如果好用，请收藏地址，帮忙分享。
        public class ListItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string taskId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string taskCode { get; set; }
            /// <summary>
            /// 火燎子安【1000】个
            /// </summary>
            public string taskName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string workorderId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string workorderCode { get; set; }
            /// <summary>
            /// 火燎子安
            /// </summary>
            public string workorderName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string workstationId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string workstationCode { get; set; }
            /// <summary>
            /// 印刷工作站
            /// </summary>
            public string workstationName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string routeId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string routeCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string processId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string processCode { get; set; }
            /// <summary>
            /// 印刷
            /// </summary>
            public string processName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string itemId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string itemCode { get; set; }
            /// <summary>
            /// 火燎子安
            /// </summary>
            public string itemName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string specification { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string unitOfMeasure { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double quantity { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double quantityProduced { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double quantityQuanlify { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double quantityUnquanlify { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public double quantityChanged { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string clientId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string clientCode { get; set; }
            /// <summary>
            /// 金融街
            /// </summary>
            public string clientName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string startTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int duration { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string endTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string colorCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string requestDate { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string remark { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int attr3 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int attr4 { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string createBy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string createTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string updateBy { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string updateTime { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string createUserId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string updateUserId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string clientOrderCode { get; set; }
        }

        public class Data
        {
            /// <summary>
            /// 
            /// </summary>
            public List<ListItem> list { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string total { get; set; }
        }

        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public int code { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Data data { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string msg { get; set; }
        }

    }
}
