using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_software_upgrade_management
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，在产品概况中查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string OperationalSoftwareUpgradeTask(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_software_upgrade_management/operational";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20200529233236";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数taskStatus: 类型long, 参数可以为空
        //  描述:子任务状态：
        //  0.待升级，1.查询设备版本号，2.新版本通知，3.请求升级包，4.设备上报下载状态，5.执行升级，6.通知升级结果
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询值，设备ID或设备编号(IMEI)或设备名称模糊查询
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页码
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页显示数
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，可在产品概况中查看
        public static string QuerySoftwareUpgradeSubtasks(string appKey, string appSecret, string id, string productId, string MasterKey, string taskStatus = "", string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_software_upgrade_management/details";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("taskStatus", taskStatus);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20200529233212";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey,产品概况中查看
        public static string QuerySoftwareUpgradeTask(string appKey, string appSecret, string id, string productId, string MasterKey)
        {
            string path = "/aep_software_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20200529233136";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，产品概况可以查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateSoftwareUpgradeTask(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_software_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20200529233123";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，在产品概况中查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string ModifySoftwareUpgradeTask(string appKey, string appSecret, string id, string MasterKey, string body)
        {
            string path = "/aep_software_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20200529233103";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，在产品概况中查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string ControlSoftwareUpgradeTask(string appKey, string appSecret, string id, string MasterKey, string body)
        {
            string path = "/aep_software_upgrade_management/control";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20200529233046";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数updateBy: 类型String, 参数可以为空
        //  描述:修改人
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，在产品概况中查看
        public static string DeleteSoftwareUpgradeTask(string appKey, string appSecret, string id, string productId, string MasterKey, string updateBy = "")
        {
            string path = "/aep_software_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("updateBy", updateBy);

            string version = "20200529233037";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数可以为空
        //  描述:主任务id,isSelectDevice为1时必填，为2不必填
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数isSelectDevice: 类型String, 参数不可以为空
        //  描述:查询类型（1.查询加入升级设备，2.查询可加入升级设备）
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页，默认1
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页显示数，默认20
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，产品概况中查看
        //参数deviceIdSearch: 类型String, 参数可以为空
        //  描述:根据设备id精确查询
        //参数deviceNameSearch: 类型String, 参数可以为空
        //  描述:根据设备名称精确查询
        //参数imeiSearch: 类型String, 参数可以为空
        //  描述:根据imei号精确查询，仅支持LWM2M协议
        //参数deviceGroupIdSearch: 类型String, 参数可以为空
        //  描述:根据群组id精确查询
        public static string QuerySoftwareUpradeDeviceList(string appKey, string appSecret, string productId, string isSelectDevice, string MasterKey, string id = "", string pageNow = "", string pageSize = "", string deviceIdSearch = "", string deviceNameSearch = "", string imeiSearch = "", string deviceGroupIdSearch = "")
        {
            string path = "/aep_software_upgrade_management/devices";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("isSelectDevice", isSelectDevice);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);
            param.Add("deviceIdSearch", deviceIdSearch);
            param.Add("deviceNameSearch", deviceNameSearch);
            param.Add("imeiSearch", imeiSearch);
            param.Add("deviceGroupIdSearch", deviceGroupIdSearch);

            string version = "20200529233027";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:
        public static string QuerySoftwareUpgradeDetail(string appKey, string appSecret, string id, string productId, string MasterKey)
        {
            string path = "/aep_software_upgrade_management/detail";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20200529233010";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数，默认1
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页显示数，默认20
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey，产品概况中查看
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询条件，支持主任务名称模糊查询
        public static string QuerySoftwareUpgradeTaskList(string appKey, string appSecret, string productId, string MasterKey, string pageNow = "", string pageSize = "", string searchValue = "")
        {
            string path = "/aep_software_upgrade_management/tasks";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);
            param.Add("searchValue", searchValue);

            string version = "20200529232940";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
