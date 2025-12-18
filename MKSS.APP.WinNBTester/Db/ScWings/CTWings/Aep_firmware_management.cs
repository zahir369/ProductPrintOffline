using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_firmware_management
    {
        //参数id: 类型long, 参数不可以为空
        //  描述:固件id
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string UpdateFirmware(string appKey, string appSecret, string id, string body, string MasterKey = "")
        {
            string path = "/aep_firmware_management/firmware";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20190615001705";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询条件，可以根据固件名称模糊查询
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:
        public static string QueryFirmwareList(string appKey, string appSecret, string productId, string searchValue = "", string pageNow = "", string pageSize = "", string MasterKey = "")
        {
            string path = "/aep_firmware_management/firmwares";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190615001608";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:固件id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string QueryFirmware(string appKey, string appSecret, string id, string productId, string MasterKey = "")
        {
            string path = "/aep_firmware_management/firmware";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20190618151645";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:固件id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数updateBy: 类型String, 参数可以为空
        //  描述:修改人
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string DeleteFirmware(string appKey, string appSecret, string id, string productId, string updateBy = "", string MasterKey = "")
        {
            string path = "/aep_firmware_management/firmware";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("updateBy", updateBy);
            param.Add("MasterKey", MasterKey);

            string version = "20190615001534";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }

    }
}
