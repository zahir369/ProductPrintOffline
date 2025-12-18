using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Tenant_device_statistics
    {
        public static string QueryTenantDeviceCount(string appKey, string appSecret)
        {
            string path = "/tenant_device_statistics/queryTenantDeviceCount";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201225122555";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数dateType: 类型String, 参数不可以为空
        //  描述:时间类型：d:天；m:月
        //参数type: 类型String, 参数不可以为空
        //  描述:数据类型：1：设备总数量，激活数，活跃数；3：设备活跃数，活跃率
        public static string QueryTenantDeviceTrend(string appKey, string appSecret, string dateType, string type)
        {
            string path = "/tenant_device_statistics/queryTenantDeviceTrend";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("dateType", dateType);
            param.Add("type", type);

            string version = "20201225122550";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        public static string QueryTenantDeviceActiveCount(string appKey, string appSecret)
        {
            string path = "/tenant_device_statistics/queryTenantDeviceActiveCount";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20201225122558";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
