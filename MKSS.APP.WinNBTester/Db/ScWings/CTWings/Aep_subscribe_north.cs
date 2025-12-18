using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_subscribe_north
    {
        //参数subId: 类型long, 参数不可以为空
        //  描述:订阅记录id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:产品MasterKey
        public static string GetSubscription(string appKey, string appSecret, string subId, string productId, string MasterKey)
        {
            string path = "/aep_subscribe_north/subscription";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("subId", subId);
            param.Add("productId", productId);

            string version = "20181031202033";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品ID
        //参数pageNow: 类型long, 参数不可以为空
        //  描述:当前页
        //参数pageSize: 类型long, 参数不可以为空
        //  描述:每页条数
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        //参数subType: 类型long, 参数可以为空
        //  描述:订阅类型
        //参数searchValue: 类型String, 参数可以为空
        //  描述:检索deviceId,模糊匹配
        public static string GetSubscriptionsList(string appKey, string appSecret, string productId, string pageNow, string pageSize, string MasterKey, string subType = "", string searchValue = "")
        {
            string path = "/aep_subscribe_north/subscribes";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);
            param.Add("subType", subType);
            param.Add("searchValue", searchValue);

            string version = "20181031202027";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数subId: 类型String, 参数不可以为空
        //  描述:订阅记录id
        //参数deviceId: 类型String, 参数可以为空
        //  描述:设备id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数subLevel: 类型long, 参数不可以为空
        //  描述:订阅级别
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:产品MasterKey
        public static string DeleteSubscription(string appKey, string appSecret, string subId, string productId, string subLevel, string MasterKey, string deviceId = "")
        {
            string path = "/aep_subscribe_north/subscription";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("subId", subId);
            param.Add("deviceId", deviceId);
            param.Add("productId", productId);
            param.Add("subLevel", subLevel);

            string version = "20181031202023";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateSubscription(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_subscribe_north/subscription";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20181031202018";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
