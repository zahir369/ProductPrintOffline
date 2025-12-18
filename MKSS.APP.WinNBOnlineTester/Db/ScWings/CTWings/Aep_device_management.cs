using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    public class Aep_device_management
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数searchValue: 类型String, 参数可以为空
        //  描述:T-link协议可选填:设备名称，设备编号，设备Id
        //  MQTT协议可选填:设备名称，设备编号，设备Id
        //  LWM2M协议可选填:设备名称，设备Id ,IMEI号
        //  TUP协议可选填:设备名称，设备Id ,IMEI号
        //  TCP协议可选填:设备名称，设备编号，设备Id
        //  HTTP协议可选填:设备名称，设备编号，设备Id
        //  JT/T808协议可选填:设备名称，设备编号，设备Id
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数,最大100
        public static string QueryDeviceList(string appKey, string appSecret, string MasterKey, string productId, string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_device_management/devices";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190507012134";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数deviceId: 类型String, 参数不可以为空
        //  描述:
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        public static string QueryDevice(string appKey, string appSecret, string MasterKey, string deviceId, string productId)
        {
            string path = "/aep_device_management/device";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("deviceId", deviceId);
            param.Add("productId", productId);

            string version = "20181031202139";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数deviceIds: 类型String, 参数不可以为空
        //  描述:可以删除多个设备（最多支持200个设备）。多个设备id，中间以逗号 "," 隔开 。样例：05979394b88a45b0842de729c03d99af,06106b8e1d5a458399326e003bcf05b4
        public static string DeleteDevice(string appKey, string appSecret, string MasterKey, string productId, string deviceIds)
        {
            string path = "/aep_device_management/device";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("deviceIds", deviceIds);

            string version = "20181031202131";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        //参数deviceId: 类型String, 参数不可以为空
        //  描述:
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string UpdateDevice(string appKey, string appSecret, string MasterKey, string deviceId, string body)
        {
            string path = "/aep_device_management/device";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("deviceId", deviceId);

            string version = "20181031202122";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateDevice(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_management/device";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20181031202117";

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
        public static string BindDevice(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_management/bindDevice";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20191024140057";

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
        public static string UnbindDevice(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_management/unbindDevice";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20191024140103";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数imei: 类型String, 参数不可以为空
        //  描述:
        public static string QueryProductInfoByImei(string appKey, string appSecret, string imei)
        {
            string path = "/aep_device_management/device/getProductInfoFormApiByImei";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("imei", imei);

            string version = "20191213161859";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
