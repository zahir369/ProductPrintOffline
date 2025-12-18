using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_command_modbus
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型String, 参数不可以为空
        //  描述:产品ID，必填
        //参数deviceId: 类型String, 参数不可以为空
        //  描述:设备ID，必填
        //参数status: 类型String, 参数可以为空
        //  描述:状态可选填： 1：指令已保存 2：指令已发送 3：指令已送达 4：指令已完成 6：指令已取消 999：指令失败
        //参数startTime: 类型String, 参数可以为空
        //  描述:
        //参数endTime: 类型String, 参数可以为空
        //  描述:
        //参数pageNow: 类型String, 参数可以为空
        //  描述:
        //参数pageSize: 类型String, 参数可以为空
        //  描述:
        public static string QueryCommandList(string appKey, string appSecret, string MasterKey, string productId, string deviceId, string status = "", string startTime = "", string endTime = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_command_modbus/modbus/commands";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("deviceId", deviceId);
            param.Add("status", status);
            param.Add("startTime", startTime);
            param.Add("endTime", endTime);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20200904171008";

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
        //  描述:产品ID
        //参数deviceId: 类型String, 参数不可以为空
        //  描述:设备ID
        //参数commandId: 类型String, 参数不可以为空
        //  描述:指令ID
        public static string QueryCommand(string appKey, string appSecret, string MasterKey, string productId, string deviceId, string commandId)
        {
            string path = "/aep_command_modbus/modbus/command";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("deviceId", deviceId);
            param.Add("commandId", commandId);

            string version = "20200904172207";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string CancelCommand(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_command_modbus/modbus/cancelCommand";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20200404012453";

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
        public static string CreateCommand(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_command_modbus/modbus/command";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20200404012449";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
