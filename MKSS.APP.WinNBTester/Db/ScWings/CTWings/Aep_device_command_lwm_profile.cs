using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_device_command_lwm_profile
    {
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateCommandLwm2mProfile(string appKey, string appSecret, string body, string MasterKey = "")
        {
            string path = "/aep_device_command_lwm_profile/commandLwm2mProfile";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20191231141545";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
