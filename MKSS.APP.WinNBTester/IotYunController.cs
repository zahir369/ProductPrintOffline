using MKSS.Core.Common.HttpRestSharp;
using MKSS.Core.Model.SysBase;
using MKSS.Model;
using RestSharp;
using SqlSugar;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AepSdk.Apis;
using MKSS.Core.Common.Helper;
using MKSS.Core.Services.SysCTWings;

namespace MKSS.APP.WinNBTester
{
    /// <summary>
    /// 管家婆接口
    /// </summary> 
    public class IotYunController  
    { 
        public IotYunController( )  
        {
             
        }


        public static CreateTokenByAppKeyAndSercert Token = null;
        public static CreateTokenByAppKeyAndSercert CreateTokenByAppKeyAndSercert()
        {

            try
            {
                if (Token == null || DateTime.Now >= Token.ExpireDate)
                {
                    Token = null;
                    
                    string IotYunAddr = ConfigurationManager.AppSettings["IotYunAddr"];
                    string IotYunAppkey = ConfigurationManager.AppSettings["IotYunAppkey"];
                    string IotYunAppsercert = ConfigurationManager.AppSettings["IotYunAppsercert"];
                    string IotYunsub1 = ConfigurationManager.AppSettings["IotYunAddr"] + "/api/openiot/OpenIotToken/CreateTokenByAppKeyAndSercert";
                    string param = string.Format("appkey={0}&appsercert={1}", IotYunAppkey, IotYunAppsercert);
                    var client = new RestSharpClient(IotYunAddr);
                    var request = client.Execute(new RestRequest($"{IotYunsub1}?{param}", Method.GET));
                    if (request.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(request.ErrorMessage);
                    }
                    IotYunResult<CreateTokenByAppKeyAndSercert> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<IotYunResult<CreateTokenByAppKeyAndSercert>>(request.Content);
                    Token = temp.Data;
                }

                return Token;

            }
            catch (Exception ex)
            {
                throw new Exception("云平台获取数据出错：" + ex);
            }

            throw new Exception("云平台获取数据出错。");
        }

        /// <summary>
        /// 获取型号列表
        /// </summary>
        /// <returns></returns>
        
        public QueryProductList QueryProductList()
        {
             
            try
            {

                var token = CreateTokenByAppKeyAndSercert( );
                string IotYunAddr = ConfigurationManager.AppSettings["IotYunAddr"];
                string AccessToken = Token.AccessToken;
                string IotYunsub1 = ConfigurationManager.AppSettings["IotYunAddr"] + "/api/openiot/OpenIotProduct/QueryProductList";
                string param = string.Format("AccessToken={0}", HttpUtility.UrlEncode(AccessToken));
                RestRequest req = new RestRequest($"{IotYunsub1}?{param}", Method.POST, DataFormat.Json);
                req.AddHeader("Content-Type", "application/json");
                req.AddJsonBody(new
                {
                    Page = 1,
                    PageSize = 1000,
                    ProductId = "",
                    EventId = "",
                    OrderId = ""
                });

                var client = new RestSharpClient(IotYunAddr);
                var request = client.Execute(req);

                if (request.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(request.ErrorMessage);
                }
                IotYunResult<QueryProductList> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<IotYunResult<QueryProductList>>(request.Content);
                QueryProductList dataResult = temp.Data;
                dataResult.data = dataResult.data.OrderBy(w => w.ProductName).ToList();
                return dataResult;

            }
            catch (Exception ex)
            {
                throw new Exception("云平台获取数据出错：" + ex);
            }

            throw new Exception("云平台获取数据出错。");


        }


        /// <summary>
        /// 获取NB平台型号列表
        /// </summary>
        /// <returns></returns> 
        public QueryNBProductList QueryNBProductList()
        {
 

            try
            {

                var token = CreateTokenByAppKeyAndSercert( );
                string IotYunAddr = ConfigurationManager.AppSettings["IotYunAddr"];
                string AccessToken = Token.AccessToken;
                string IotYunsub1 = ConfigurationManager.AppSettings["IotYunAddr"] + "/api/openiot/OpenIotProduct/QueryNBProductList";
                string param = string.Format("AccessToken={0}", HttpUtility.UrlEncode(AccessToken));
                RestRequest req = new RestRequest($"{IotYunsub1}?{param}", Method.POST, DataFormat.Json);
                req.AddHeader("Content-Type", "application/json");
                req.AddJsonBody(new
                {
                    Page = 1,
                    PageSize = 1000,
                    ProductId = "",
                    EventId = "",
                    OrderId = ""
                });

                var client = new RestSharpClient(IotYunAddr);
                var request = client.Execute(req);

                if (request.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(request.ErrorMessage);
                }
                IotYunResult<QueryNBProductList> temp = Newtonsoft.Json.JsonConvert.DeserializeObject<IotYunResult<QueryNBProductList>>(request.Content);
                QueryNBProductList dataResult = temp.Data;
                dataResult.data = dataResult.data.OrderBy(w => w.ProductName).ToList();
                return dataResult;

            }
            catch (Exception ex)
            {
                throw new Exception("云平台获取数据出错：" + ex);
            }
            throw new Exception("云平台获取数据出错。");


        }

        BaseThirdPartyApp wingInfo;
        CTWingsProductInfoModel productModel;
        public bool RegisterNetInner( Device dev )
        {
            
            SqlSugarClient _dbProduct = MysqlDB.GetDBProduct();
            SqlSugarClient _db = MysqlDB.GetDB();
            if (wingInfo == null)
            {
                var appList = _dbProduct.Queryable<BaseThirdPartyApp>().Where(it => it.F_AppCode.Equals("t_win_code")).ToList();
                if (appList.Any())
                {
                    wingInfo = appList.First();
                }
                else
                {
                    throw new Exception("找不到电信Wings授权码");
                }
            }

            if (productModel == null)
            {
                var result = Aep_product_management.QueryProductList(wingInfo.F_AppKey, wingInfo.F_AppSecret);
                var resultModel = JsonHelper.ToObject<CTWingsReplyModel>(result);
                if (resultModel.Result.List.Count < 1)
                {
                    throw new Exception("找不到电信产品类别");
                }
                productModel = resultModel.Result.List[0];
            }


            var createDeviceBody = new CreateDevicceRequestInfoModel();
            createDeviceBody.deviceName = "预留";
            createDeviceBody.deviceSn = "预留";
            createDeviceBody.imei = dev.F_IMEI;
            createDeviceBody.productId = productModel.ProductId;
            createDeviceBody.@operator = "程文龙";
            createDeviceBody.other = new CreateDevicceRequestInfoModelOther();
            createDeviceBody.other.autoObserver = 0;
            createDeviceBody.other.imsi = string.Empty;
            createDeviceBody.other.pskValue = string.Empty;


            dev.F_RegisterTime = DateTime.Now;

            try
            {
                var resultDevice = Aep_device_management.CreateDevice(wingInfo.F_AppKey, wingInfo.F_AppSecret, wingInfo.F_Token, JsonHelper.ToJson(createDeviceBody));
                if (!string.IsNullOrEmpty(resultDevice))
                {
                    var createResult = JsonHelper.ToObject<CreateDeviceReplyInfoModel>(resultDevice);
                    if (createResult.code.Equals(0) && !string.IsNullOrEmpty(createResult.result.deviceId))
                    {
                        dev.F_DeviceId = createResult.result.deviceId;
                        dev.DeviceStatus = DeviceStatus.Register;
                        dev.F_LastRefreshTime = DateTime.Now;
                        var updatedeviceResult = _db.Updateable<Device>(dev).ExecuteCommand();
                        return true;
                    } else if (createResult.code.Equals(2010115))//IMEI号已存在
                    {
                        string str = Aep_device_management.QueryDeviceList(wingInfo.F_AppKey, wingInfo.F_AppSecret, wingInfo.F_Token, productModel.ProductId+"", dev.F_IMEI);
                        var createQueryResult = JsonHelper.ToObject<QueryDeviceListResult>(str);
                        if (createQueryResult.code == 0)
                        {
                            dev.F_DeviceId = createQueryResult.result.list.FirstOrDefault().deviceId;
                            dev.DeviceStatus = DeviceStatus.Register;
                            dev.F_LastRefreshTime = DateTime.Now;
                            var updateQuerydeviceResult = _db.Updateable<Device>(dev).ExecuteCommand();
                            return true;
                        }
                        else {
                            dev.DeviceStatus = DeviceStatus.RegisterFaild;
                            dev.F_LastRefreshTime = DateTime.Now;
                            var updatedeviceRxxesult = _db.Updateable<Device>(dev).ExecuteCommand();
                            throw new Exception("查询现有设备CTWING出错：" + createQueryResult.msg);
                        }
                    }
                    else
                    {
                        dev.DeviceStatus = DeviceStatus.RegisterFaild;
                        dev.F_LastRefreshTime = DateTime.Now;
                        var updatedeviceResult = _db.Updateable<Device>(dev).ExecuteCommand();
                        throw new Exception("注册到CTWING出错："+createResult.msg);
                    }
                }
                else
                {
                    dev.DeviceStatus = DeviceStatus.RegisterFaild;
                    dev.F_LastRefreshTime = DateTime.Now;
                    var updatedeviceResult = _db.Updateable<Device>(dev).ExecuteCommand();
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        public bool Test(string F_IMEI)
        {

            SqlSugarClient _dbProduct = MysqlDB.GetDBProduct();
            SqlSugarClient _db = MysqlDB.GetDB();
            if (wingInfo == null)
            {
                var appList = _dbProduct.Queryable<BaseThirdPartyApp>().Where(it => it.F_AppCode.Equals("t_win_code")).ToList();
                if (appList.Any())
                {
                    wingInfo = appList.First();
                }
                else
                {
                    throw new Exception("找不到电信Wings授权码");
                }
            }

            if (productModel == null)
            {
                var result = Aep_product_management.QueryProductList(wingInfo.F_AppKey, wingInfo.F_AppSecret);
                var resultModel = JsonHelper.ToObject<CTWingsReplyModel>(result);
                if (resultModel.Result.List.Count < 1)
                {
                    throw new Exception("找不到电信产品类别");
                }
                productModel = resultModel.Result.List[0];
            }

            string str = Aep_device_management.QueryDeviceList(wingInfo.F_AppKey, wingInfo.F_AppSecret, wingInfo.F_Token, productModel.ProductId + "", F_IMEI);
            var createQueryResult = JsonHelper.ToObject<QueryDeviceListResult>(str);
            if (createQueryResult.code == 0)
            {
                
                return true;
            }
            else
            {
                return true;
            }


        }


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(string id)
        {

            try
            {

                SqlSugarClient _dbProduct = MysqlDB.GetDBProduct();
                SqlSugarClient _db = MysqlDB.GetDB();

                var resultDevice = _db.Queryable<Device>().Where(w=>w.F_Id==id).First();
                if (resultDevice == null) return false;

                if (wingInfo == null)
                {
                    var appList = _dbProduct.Queryable<BaseThirdPartyApp>().Where(it => it.F_AppCode.Equals("t_win_code")).ToList();
                    if (appList.Any())
                    {
                        wingInfo = appList.First();
                    }
                    else
                    {
                        throw new Exception("找不到电信Wings授权码");
                    }
                }
                 

                var result = Aep_product_management.QueryProductList(wingInfo.F_AppKey, wingInfo.F_AppSecret);
                var resultModel = JsonHelper.ToObject<CTWingsReplyModel>(result);
                if (resultModel.Result.List.Count < 1)
                {
                    throw new Exception("找不到电信产品类别");
                }
                CTWingsProductInfoModel productModel = resultModel.Result.List[0];


                try
                {
                    var delDevice = Aep_device_management.DeleteDevice(wingInfo.F_AppKey, wingInfo.F_AppSecret, wingInfo.F_Token, productModel.ProductId + "", resultDevice.F_DeviceId);
                    var createResult = JsonHelper.ToObject<DeleteDeviceReplyInfoModel>(delDevice);
                    if (createResult.code.Equals(0))
                    {
                        //成功 
                    }
                    else
                    {
                        //throw new Exception("CTWING平台" + createResult.msg + "：" + createResult.result.reason);
                    }

                }
                catch (Exception ex)
                {
                    //throw new Exception("注销CTWING平台设备时出错：" + ex.Message);
                }

                var _Device = _db.Deleteable<Device>().Where(w=>w.F_Id == id).ExecuteCommand()>0;
                return _Device;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

    }

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
     
}
