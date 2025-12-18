using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Usr
    {
        //参数sdk_type: 类型String, 参数可以为空
        //  描述:SDK语言类型，默认为Java(字典项: sdk_type)
        //参数file_name: 类型String, 参数不可以为空
        //  描述:SDK描述，用以标识其中的biz包
        //参数application_id: 类型String, 参数不可以为空
        //  描述:应用编码，下载的SDK会根据该编码收集所有有权限的API打包
        //参数api_version: 类型String, 参数可以为空
        //  描述:API版本信息 TODO
        public static string SdkDownload(string appKey, string appSecret, string file_name, string application_id, string sdk_type = "", string api_version = "")
        {
            string path = "/usr/sdk/download";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("sdk_type", sdk_type);
            param.Add("file_name", file_name);
            param.Add("application_id", application_id);
            param.Add("api_version", api_version);

            string version = "20180000000000";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
