using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_software_management
    {
        //参数id: 类型long, 参数不可以为空
        //  描述:升级包id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，在产品概况中可以查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string UpdateSoftware(string appKey, string appSecret, string id, string MasterKey, string body)
        {
            string path = "/aep_software_management/software";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20200529232851";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:升级包id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey,在产品概况中可以查询
        public static string DeleteSoftware(string appKey, string appSecret, string id, string productId, string MasterKey)
        {
            string path = "/aep_software_management/software";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20200529232809";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:升级包ID
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，可在产品概况中查看
        public static string QuerySoftware(string appKey, string appSecret, string id, string productId, string MasterKey)
        {
            string path = "/aep_software_management/software";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20200529232806";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询条件，可以根据升级包名称模糊查询
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，可以在产品概况中查看
        public static string QuerySoftwareList(string appKey, string appSecret, string productId, string MasterKey, string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_software_management/softwares";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20200529232801";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
