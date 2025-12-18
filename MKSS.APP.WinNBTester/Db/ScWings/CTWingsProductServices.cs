//using MKSS.Core.IRepository.Base;
//using MKSS.Core.Services.BASE;
//using System.Threading.Tasks;
//using MKSS.Core.IServices.SysBase;
//using MKSS.Core.Model.SysBase;
//using System.Linq;
//using AepSdk.Apis;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using MKSS.Core.Common.Helper;
//using System;

using Newtonsoft.Json;
using System.Collections.Generic;

namespace MKSS.Core.Services.SysCTWings
{

    ////  平台型号
    //public class CTWingsProductServices : BaseServices<CTWingsProduct>, ICTWingsProductServices
    //{
    //    readonly IBaseRepository<CTWingsProduct> dal;
    //    readonly IBaseThirdPartyAppServices baseThirdPartyAppServices;
    //    public CTWingsProductServices(IBaseRepository<CTWingsProduct> dal, IBaseThirdPartyAppServices baseThirdPartyAppServices)
    //    {
    //        this.dal = dal;
    //        base.BaseDal = dal;
    //        this.baseThirdPartyAppServices = baseThirdPartyAppServices;
    //    }

    //    /// <summary>
    //    /// 同步CTWings平台的产品信息
    //    /// </summary>
    //    /// <returns></returns>
    //    public async Task<bool> SyncCTWingsProducts(Base_User userInfo)
    //    {
    //        BaseThirdPartyApp wingInfo = null;
    //        var appList = await baseThirdPartyAppServices.Query(it => it.F_AppCode.Equals("ctwing"));
    //        if (appList.Any())
    //        {
    //            wingInfo = appList.First();
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //        var result = Aep_product_management.QueryProductList(wingInfo.F_AppKey, wingInfo.F_AppSecret);
    //        var resultModel = JsonHelper.ToObject<CTWingsReplyModel>(result);
    //        if (resultModel == null)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            if (!resultModel.Code.Equals(0))
    //            {
    //                return false;
    //            }
    //            foreach (var item in resultModel.Result.List)
    //            {
    //                var existList = await dal.Query(it => it.F_ProductId.Equals(item.ProductId.ToString()) && it.F_TenantId.Equals(item.TenantId));
    //                if (existList.Any())
    //                {
    //                    var updateProductInfo = existList.First();
    //                    updateProductInfo.F_APIkey = item.ApiKey;
    //                    updateProductInfo.F_ProductName = item.ProductName;
    //                    updateProductInfo.F_ProductType = item.ProductType;
    //                    updateProductInfo.F_SecondaryType = item.SecondaryType;
    //                    updateProductInfo.F_ThirdType = item.ThirdType;
    //                    updateProductInfo.F_Protocol = item.ProductProtocol;
    //                    updateProductInfo.F_AuthType = item.AuthType;
    //                    updateProductInfo.F_NodeType = item.NodeType;
    //                    updateProductInfo.F_AccessType = item.AccessType;
    //                    updateProductInfo.F_NetworkType = item.NetworkType;
    //                    updateProductInfo.F_PowerModel = item.PowerModel;
    //                    updateProductInfo.F_ModifyDate = DateTime.Now;
    //                    updateProductInfo.F_ModifyUserId = userInfo.F_Id;
    //                    updateProductInfo.F_ModifyUserName = userInfo.F_RealName;
    //                    var updateResult = await dal.Update(updateProductInfo);
    //                }
    //                else
    //                {
    //                    var addProductInfo = new CTWingsProduct()
    //                    {
    //                        F_Id = StringHelper.GetGUID(),
    //                        F_ProductId = item.ProductId.ToString(),
    //                        F_APIkey = item.ApiKey,
    //                        F_ProductName = item.ProductName,
    //                        F_TenantId = item.TenantId,
    //                        F_ProductType = item.ProductType,
    //                        F_SecondaryType = item.SecondaryType,
    //                        F_ThirdType = item.ThirdType,
    //                        F_Protocol = item.ProductProtocol,
    //                        F_AuthType = item.AuthType,
    //                        F_NodeType = item.NodeType,
    //                        F_AccessType = item.AccessType,
    //                        F_NetworkType = item.NetworkType,
    //                        F_PowerModel = item.PowerModel,
    //                        F_CreateDate = DateTime.Now,
    //                        F_CreateUserId = userInfo.F_Id,
    //                        F_CreateUserName = userInfo.F_RealName
    //                    };
    //                    var addResult = await dal.Add(addProductInfo);
    //                }
    //            }
    //            return true;
    //        }
    //    }
    //}



    /// <summary>
    /// 应答信息
    /// </summary>
    public class CTWingsReplyModel
    {
        [JsonProperty(PropertyName = "code")]
        public int Code { get; set; }
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "result")]
        public CTWingsPageModel Result { get; set; }
    }
    public class CTWingsPageModel
    {
        /// <summary>
        /// 页码
        /// </summary>
        [JsonProperty(PropertyName = "pageNum")]
        public int PageNum { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        [JsonProperty(PropertyName = "pageSize")]
        public int PageSize { get; set; }
        /// <summary>
        /// 总条数
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        /// <summary>
        /// 数据行集合
        /// </summary>
        [JsonProperty(PropertyName = "list")]
        public List<CTWingsProductInfoModel> List { get; set; }
    }


    public  class CTWingsProductInfoModel
    {
        [JsonProperty(PropertyName = "productId")]
        public long ProductId { get; set; }
        [JsonProperty(PropertyName = "productName")]
        public string ProductName { get; set; }
        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId { get; set; }
        [JsonProperty(PropertyName = "productDesc")]
        public string ProductDesc { get; set; }
        [JsonProperty(PropertyName = "productType")]
        public int ProductType { get; set; }
        [JsonProperty(PropertyName = "secondaryType")]
        public int SecondaryType { get; set; }
        [JsonProperty(PropertyName = "thirdType")]
        public int ThirdType { get; set; }
        [JsonProperty(PropertyName = "productProtocol")]
        public int ProductProtocol { get; set; }
        [JsonProperty(PropertyName = "authType")]
        public int AuthType { get; set; }
        [JsonProperty(PropertyName = "payloadFormat")]
        public string PayloadFormat { get; set; }
        [JsonProperty(PropertyName = "createTime")]
        public long CreateTime { get; set; }
        [JsonProperty(PropertyName = "updateTime")]
        public long UpdateTime { get; set; }
        [JsonProperty(PropertyName = "networkType")]
        public int NetworkType { get; set; }
        [JsonProperty(PropertyName = "endpointFormat")]
        public int EndpointFormat { get; set; }
        [JsonProperty(PropertyName = "powerModel")]
        public int PowerModel { get; set; }
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }
        [JsonProperty(PropertyName = "onlineDeviceCount")]
        public int OnlineDeviceCount { get; set; }
        [JsonProperty(PropertyName = "deviceCount")]
        public string DeviceCount { get; set; }//出现空数据造成错误
        [JsonProperty(PropertyName = "productTypeValue")]
        public string ProductTypeValue { get; set; }
        [JsonProperty(PropertyName = "secondaryTypeValue")]
        public string SecondaryTypeValue { get; set; }
        [JsonProperty(PropertyName = "thirdTypeValue")]
        public string ThirdTypeValue { get; set; }
        [JsonProperty(PropertyName = "encryptionType")]
        public int EncryptionType { get; set; }
        [JsonProperty(PropertyName = "rootCert")]
        public string RootCert { get; set; }
        [JsonProperty(PropertyName = "createBy")]
        public string CreateBy { get; set; }
        [JsonProperty(PropertyName = "updateBy")]
        public string UpdateBy { get; set; }
        [JsonProperty(PropertyName = "tupDeviceModel")]
        public string tupDeviceModel { get; set; }
        [JsonProperty(PropertyName = "accessType")]
        public int AccessType { get; set; }
        [JsonProperty(PropertyName = "nodeType")]
        public int NodeType { get; set; }
        [JsonProperty(PropertyName = "tupIsThrough")]
        public int TupIsThrough { get; set; }
        [JsonProperty(PropertyName = "dataEncryption")]
        public int DataEncryption { get; set; }
        [JsonProperty(PropertyName = "lwm2mEdrxTime")]
        public string Lwm2mEdrxTime { get; set; }
    }


    /**
     * 
         {
          "deviceName": "xxxxx",
          "deviceSn": "xxxxx",
          "imei": "xxxxx",
          "operator": "xxxxx",
          "other": {"autoObserver":0,
                    "imsi":"xxxxx",
                    "pskValue":"xxxxx"},
          "productId": 15062683
        }
    **/
    public class CreateDevicceRequestInfoModel
    {
        public long productId { get; set; }
        public string @operator { get; set; }
        public CreateDevicceRequestInfoModelOther other { get; set; }
        public string deviceName { get; set; }
        public string deviceSn { get; set; }
        public string imei { get; set; }
    }

    public class CreateDevicceRequestInfoModelOther
    {
        public int autoObserver { get; set; }
        public string imsi { get; set; }
        public string pskValue { get; set; }
    }


    /**
     * 
     {
	        "code": 0,
	        "msg": "ok",
	        "result": {
		        "deviceId": "89e920fa0eda47a89f04f52a88b17146",
		        "deviceName": "test003",
		        "tenantId": "300",
		        "productId": 10003304,
		        "imei": "125658789874565",
		        "deviceSn": ""
	        }
        }
     * **/
    public class CreateDeviceReplyInfoModel
    {
        public int code { get; set; }
        public string msg { get; set; }
        public CreateDeviceReplyInfoModelResult result { get; set; }
    }

    public class CreateDeviceReplyInfoModelResult
    {
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string tenantId { get; set; }
    }

    //{"code":0,"msg":"ok","result":null}
    //{"code":1127,"msg":"删除失败的设备","result":[{"deviceId":"199f55d250d34c6e8319e4089f0f7567","reason":"此设备不存在"}]}
    //
    public class DeleteDeviceReplyInfoModel 
    {
        public int code { get; set; }
        public string msg { get; set; }
        public List<DeleteDeviceReplyInfoModelResult> result { get; set; }
    }
    public class DeleteDeviceReplyInfoModelResult
    {
        public string deviceId { get; set; }
        public string reason { get; set; } 
    }

    /**
     * 
{
  "code": 0,
  "msg": "ok",
  "result": {
    "pageNum": 1,
    "pageSize": 100,
    "total": 1,
    "list": [
      {
        "deviceId": "dbcdbe5907824d92b4a5efd69d213ecf",
        "deviceName": "预留",
        "tenantId": "2000042201",
        "productId": 15062683,
        "imei": "861999063119433",
        "imsi": null,
        "firmwareVersion": null,
        "deviceStatus": 1,
        "autoObserver": 0,
        "createTime": 1645425356452,
        "createBy": "程文龙",
        "updateTime": null,
        "updateBy": null,
        "netStatus": 1,
        "onlineAt": 1645425358862,
        "offlineAt": null
      }
    ]
  }
}
     * 
     * **/
    public class QueryDeviceListResult
    {
        public int code { get; set; }
        public string msg { get; set; }
        public QueryDeviceListResultResult result { get; set; }
    }

    public class QueryDeviceListResultResult
    {
        public int pageNum { get; set; }
        public int pageSize { get; set; }
        public int total { get; set; }
        public List<QueryDeviceListResultResultItem> list { get; set; }
    }

    public class QueryDeviceListResultResultItem
    {
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string tenantId { get; set; }
        public int productId { get; set; }
        public string imei { get; set; }
        public string imsi { get; set; } 
    }

}
