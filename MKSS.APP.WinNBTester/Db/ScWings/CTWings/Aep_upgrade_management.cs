using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_upgrade_management
    {
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数productId: 类型long, 参数不可以为空
        //  描述:产品id
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string QueryRemoteUpgradeDetail(string appKey, string appSecret, string id, string productId, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/detail";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20190615001517";

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
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string QueryRemoteUpgradeTask(string appKey, string appSecret, string id, string productId, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);

            string version = "20190615001509";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string ControlRemoteUpgradeTask(string appKey, string appSecret, string id, string body, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/control";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20190615001456";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型String, 参数可以为空
        //  描述:主任务id,isSelectDevice为1时必填，为2不必填
        //参数productId: 类型String, 参数不可以为空
        //  描述:产品id
        //参数isSelectDevice: 类型String, 参数不可以为空
        //  描述:查询类型（1.查询加入升级设备，2.查询可加入升级设备）
        //参数pageNow: 类型String, 参数可以为空
        //  描述:当前页，默认1
        //参数pageSize: 类型String, 参数可以为空
        //  描述:每页显示数，默认20
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数deviceIdSearch: 类型String, 参数可以为空
        //  描述:根据设备id精确查询
        //参数deviceNameSearch: 类型String, 参数可以为空
        //  描述:根据设备名称精确查询
        //参数imeiSearch: 类型String, 参数可以为空
        //  描述:根据imei号精确查询，仅支持LWM2M协议
        //参数deviceNoSearch: 类型String, 参数可以为空
        //  描述:根据设备编号精确查询，仅支持T_Link协议
        //参数deviceGroupIdSearch: 类型String, 参数可以为空
        //  描述:根据群组id精确查询
        public static string QueryRemoteUpradeDeviceList(string appKey, string appSecret, string productId, string isSelectDevice, string id = "", string pageNow = "", string pageSize = "", string MasterKey = "", string deviceIdSearch = "", string deviceNameSearch = "", string imeiSearch = "", string deviceNoSearch = "", string deviceGroupIdSearch = "")
        {
            string path = "/aep_upgrade_management/devices";
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
            param.Add("deviceNoSearch", deviceNoSearch);
            param.Add("deviceGroupIdSearch", deviceGroupIdSearch);

            string version = "20190615001451";

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
        //参数updateBy: 类型String, 参数可以为空
        //  描述:修改人
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string DeleteRemoteUpgradeTask(string appKey, string appSecret, string id, string productId, string updateBy = "", string MasterKey = "")
        {
            string path = "/aep_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("updateBy", updateBy);

            string version = "20190615001444";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "DELETE");
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
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询条件，支持主任务名称模糊查询
        public static string QueryRemoteUpgradeTaskList(string appKey, string appSecret, string productId, string pageNow = "", string pageSize = "", string MasterKey = "", string searchValue = "")
        {
            string path = "/aep_upgrade_management/tasks";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);
            param.Add("searchValue", searchValue);

            string version = "20190615001440";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数id: 类型long, 参数不可以为空
        //  描述:主任务id
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string ModifyRemoteUpgradeTask(string appKey, string appSecret, string id, string body, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);

            string version = "20190615001433";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "PUT");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CreateRemoteUpgradeTask(string appKey, string appSecret, string body, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/task";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20190615001416";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string OperationalRemoteUpgradeTask(string appKey, string appSecret, string body, string MasterKey = "")
        {
            string path = "/aep_upgrade_management/operational";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20190615001412";

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
        //  描述:子任务状态
        //  T-Link协议必填（1.待升级，2.升级中，3.升级成功，4.升级失败）
        //  LWM2M协议选填（0:升级可行性判断,1:升级可行性判断失败,2:分派升级任务,3:分派升级任务失败,4:分派下载任务,5:分派下载任务失败,6:开始升级,7:升级失败,8:升级完成,9:取消当前设备的升级,10:取消当前设备升级成功,11:取消当前设备升级失败）
        //参数searchValue: 类型String, 参数可以为空
        //  描述:查询值，设备ID或设备编号(IMEI)或设备名称模糊查询
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页码
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页显示数
        //参数MasterKey: 类型String, 参数可以为空
        //  描述:MasterKey
        public static string QueryRemoteUpgradeSubtasks(string appKey, string appSecret, string id, string productId, string taskStatus = "", string searchValue = "", string pageNow = "", string pageSize = "", string MasterKey = "")
        {
            string path = "/aep_upgrade_management/details";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("id", id);
            param.Add("productId", productId);
            param.Add("taskStatus", taskStatus);
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190615001406";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
