using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_gateway_position
    {
        //参数cardId: 类型String, 参数不可以为空
        //  描述:定位的物联网卡号
        //参数posReqType: 类型long, 参数不可以为空
        //  描述:返回的位置类型：1为初始位置；2为最新位置；3为最新or历史位置；7目前未用到，保留数字
        public static string getPosition(string appKey, string appSecret, string cardId, string posReqType)
        {
            string path = "/aep_gateway_position/api/getPosition";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("cardId", cardId);
            param.Add("posReqType", posReqType);

            string version = "20190301085737";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
