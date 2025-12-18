using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.WinNBOnlinePrinterTest.Model
{
    public class BatchModel
    {
        public class ListItem
        {
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
            public string clientOrderId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string clientOrderCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string orderSource { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string sourceCode { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string productId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string productCode { get; set; }

            /// <summary>
            /// 火燎子安
            /// </summary>
            public string productName { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string productSpc { get; set; }

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
            public double quantityChanged { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public double quantityScheduled { get; set; }

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
            public string requestDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string parentId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string ancestors { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }

            /// <summary>
            /// 烤的焦一点，脆
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
            public string workorderType { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string createUserId { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string updateUserId { get; set; }

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
