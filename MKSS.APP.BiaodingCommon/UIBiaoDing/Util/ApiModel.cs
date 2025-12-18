using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class ApiModel
    {}


    #region 企业信息化调用相关   

    /// <summary>
    /// 企业信息化接口调用端类型
    /// </summary>
    public enum EnumEnInfoEndpoint
    {
        /// <summary>
        /// web端应用
        /// </summary>
        Web = 0,
        /// <summary>
        /// 桌面端应用
        /// </summary>
        Desktop = 1,
        /// <summary>
        /// 移动端应用
        /// </summary>
        Mobile = 2
    }


    public class EnInfoLoginBody
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public EnumEnInfoEndpoint Endpoint { get; set; } = EnumEnInfoEndpoint.Desktop;

    }



    #endregion

    //{"Msg":"已存在相同的SN","Code":0,"HttpStatus":200,"Data":null}
    public class ResultModelIot
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public int HttpStatus { get; set; }
        public object Data { get; set; }
    }
    public class IotYunResult<T>
    {
        public string Msg { get; set; }
        public int Code { get; set; }
        public int HttpStatus { get; set; }
        public T Data { get; set; }

    }

    public class CreateTokenByAppKeyAndSercert
    {
        public string AccessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }

    public class EnInfoLoginToken
    {
        public bool Success { get; set; }
        public string Token_Type { get; set; }
        public string Token { get; set; }
        public double Expires_In { get; set; } = -1.0;
        public DateTime ExpireDate { get; set; }
    }

    public class QueryProductList
    {

        public int page { get; set; }
        public int pageCount { get; set; }
        public int dataCount { get; set; }
        public int PageSize { get; set; }
        public string AccessToken { get; set; }
        public List<QueryProductListItem> data { get; set; }
    }
    public class QueryProductListItem
    {

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string FactoryName { get; set; }
        public string ParentId { get; set; }
        public int ProductType { get; set; }
        public int TransType { get; set; }
        public override string ToString()
        {
            return ProductName;
        }
    }
    public class QueryNBProductList
    {

        public int page { get; set; }
        public int pageCount { get; set; }
        public int dataCount { get; set; }
        public int PageSize { get; set; }
        public string AccessToken { get; set; }
        public List<QueryNBProductListItem> data { get; set; }
    }
    public class QueryNBProductListItem
    {

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string FactoryName { get; set; }
        public string ParentId { get; set; }
        public int ProductType { get; set; }
        public int TransType { get; set; }
        public override string ToString()
        {
            return ProductName;
        }
    }

    public class QueryOrderList
    {
        /// <summary>
        /// 
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int pageCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int dataCount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<QueryOrderListItem> data { get; set; }

        public static explicit operator List<object>(QueryOrderList v)
        {
            throw new NotImplementedException();
        }
    }
    public class QueryOrderListItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string F_Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_OrderNumber { get; set; }
        /// <summary>
        /// 02号任务单
        /// </summary>
        public string F_OrderName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_ProductId { get; set; }
        /// <summary>
        /// 新型号413
        /// </summary>
        public string F_ProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_IotProductId { get; set; }
        /// <summary>
        /// 家报MK-413(NB通用)-甲烷
        /// </summary>
        public string F_IotProductName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int F_IsRegisterIoT { get; set; }
        /// <summary>
        /// 软件中心
        /// </summary>
        public string F_Department { get; set; }
        /// <summary>
        /// 周工
        /// </summary>
        public string F_SalesPersonName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int F_Qty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int F_ToQty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_Date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_ToDate { get; set; }
        /// <summary>
        /// 宿迁
        /// </summary>
        public string F_Comment { get; set; }
        /// <summary>
        /// 定制外壳
        /// </summary>
        public string F_CommentExt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_SNRule { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string F_PrintTemplate { get; set; }
    }
}
