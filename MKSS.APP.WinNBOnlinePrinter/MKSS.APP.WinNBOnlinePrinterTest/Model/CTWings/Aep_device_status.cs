using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_device_status
    {
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryDeviceStatus(string appKey, string appSecret, string body)
        {
            string path = "/aep_device_status/deviceStatus";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20181031202028";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryDeviceStatusList(string appKey, string appSecret, string body)
        {
            string path = "/aep_device_status/deviceStatusList";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20181031202403";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string getDeviceStatusHisInTotal(string appKey, string appSecret, string body)
        {
            string path = "/aep_device_status/api/v1/getDeviceStatusHisInTotal";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20190928013529";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string getDeviceStatusHisInPage(string appKey, string appSecret, string body)
        {
            string path = "/aep_device_status/getDeviceStatusHisInPage";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20190928013337";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
