using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_device_event
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数searchValue: 类型String, 参数可以为空
        //  描述:可填值：设备Id
        //参数eventType: 类型long, 参数可以为空
        //  描述:LWM2M,MQTT,TUP协议可选填:
        //  1：信息
        //  2：警告
        //  3：故障
        //  T-link协议可选填:
        //  0:普通事件
        //  1：告警事件(普通级)
        //  2：告警事件(重大级)
        //  3：告警事件(严重级)
        //参数startTime: 类型String, 参数可以为空
        //  描述:精确到毫秒的时间戳
        //参数endTime: 类型String, 参数可以为空
        //  描述:精确到毫秒的时间戳
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryEventList(string appKey, string appSecret, string MasterKey, string productId, string searchValue = "", string eventType = "", string startTime = "", string endTime = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_device_event/events";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("eventType", eventType);
            param.Add("startTime", startTime);
            param.Add("endTime", endTime);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20210327081940";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryDeviceEventList(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_event/device/events";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20210327064751";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryDeviceEventTotal(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_event/device/events/total";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20210327064755";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
