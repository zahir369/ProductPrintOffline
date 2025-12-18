using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_standard_management
    {
        //参数standardVersion: 类型String, 参数可以为空
        //  描述:标准物模型版本号
        //参数thirdType: 类型long, 参数不可以为空
        //  描述:三级分类Id
        public static string QueryStandardModel(string appKey, string appSecret, string thirdType, string standardVersion = "")
        {
            string path = "/aep_standard_management/standardModel";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("standardVersion", standardVersion);
            param.Add("thirdType", thirdType);

            string version = "20190713033424";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
