using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_nb_device_management
    {
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string BatchCreateNBDevice(string appKey, string appSecret, string body)
        {
            string path = "/aep_nb_device_management/batchNBDevice";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20200828140355";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
