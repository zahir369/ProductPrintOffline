using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_device_model
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数searchValue: 类型String, 参数可以为空
        //  描述:可填值：属性名称，属性标识符
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryPropertyList(string appKey, string appSecret, string MasterKey, string productId, string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_device_model/dm/app/model/properties";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190712223330";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数searchValue: 类型String, 参数可以为空
        //  描述:可填： 服务Id, 服务名称,服务标识符
        //参数serviceType: 类型long, 参数可以为空
        //  描述:服务类型
        //  1. 数据上报 
        //  2. 事件上报 
        //  3.数据获取 
        //  4.参数查询 
        //  5.参数配置
        //  6.指令下发 
        //  7.指令下发响应
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryServiceList(string appKey, string appSecret, string MasterKey, string productId, string searchValue = "", string serviceType = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_device_model/dm/app/model/services";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("serviceType", serviceType);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190712224233";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
