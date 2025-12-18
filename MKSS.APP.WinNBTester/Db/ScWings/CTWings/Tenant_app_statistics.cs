using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Tenant_app_statistics
    {
        public static string QueryTenantApiMonthlyCount(string appKey, string appSecret)
        {
            string path = "/tenant_app_statistics/queryTenantApiMonthlyCount";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201225122609";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        public static string QueryTenantAppCount(string appKey, string appSecret)
        {
            string path = "/tenant_app_statistics/queryTenantAppCount";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201225122611";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数dateType: 类型String, 参数不可以为空
        //  描述:日期格式 m：月  d：日
        //参数dataType: 类型String, 参数不可以为空
        //  描述:数据格式 1：api调用量分析
        public static string QueryTenantApiTrend(string appKey, string appSecret, string dateType, string dataType)
        {
            string path = "/tenant_app_statistics/queryTenantApiTrend";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("dateType", dateType);
            param.Add("dataType", dataType);

            string version = "20201225122606";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
