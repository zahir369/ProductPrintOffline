using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_edge_gateway
    {
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string DeleteEdgeInstanceDevice(string appKey, string appSecret, string body)
        {
            string path = "/aep_edge_gateway/instance/devices";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201226000026";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数gatewayDeviceId: 类型String, 参数不可以为空
        //  描述:
        //参数pageNow: 类型long, 参数可以为空
        //  描述:
        //参数pageSize: 类型long, 参数可以为空
        //  描述:
        public static string QueryEdgeInstanceDevice(string appKey, string appSecret, string gatewayDeviceId, string pageNow = "", string pageSize = "")
        {
            string path = "/aep_edge_gateway/instance/devices";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("gatewayDeviceId", gatewayDeviceId);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20201226000022";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateEdgeInstance(string appKey, string appSecret, string body)
        {
            string path = "/aep_edge_gateway/instance";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201226000017";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string EdgeInstanceDeploy(string appKey, string appSecret, string body)
        {
            string path = "/aep_edge_gateway/instance/settings";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201226000010";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:
        public static string DeleteEdgeInstance(string appKey, string appSecret, string id)
        {
            string path = "/aep_edge_gateway/instance";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20201225235957";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string AddEdgeInstanceDevice(string appKey, string appSecret, string body)
        {
            string path = "/aep_edge_gateway/instance/device";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201226000004";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string AddEdgeInstanceDrive(string appKey, string appSecret, string body)
        {
            string path = "/aep_edge_gateway/instance/drive";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201225235952";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
