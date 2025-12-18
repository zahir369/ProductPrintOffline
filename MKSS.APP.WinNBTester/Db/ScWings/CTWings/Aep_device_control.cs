using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_device_control
    {
        //参数MasterKey: 类型String, 参数不可以为空
        //  描述:MasterKey在该设备所属产品的概况中可以查看
        //参数productId: 类型long, 参数不可以为空
        //  描述:
        //参数searchValue: 类型String, 参数可以为空
        //  描述:可选填：设备Id
        //参数type: 类型long, 参数可以为空
        //  描述:可选填枚举值：
        //  1：设备重启
        //  2：退出平台
        //  3：重新登录
        //  4：设备自检
        //  6：开始发送数据
        //  7：停止发送数据
        //  8：恢复出厂设置
        //参数status: 类型long, 参数可以为空
        //  描述:可选填：1：指令已保存
        //  2：指令已发送
        //  3：指令已送达
        //  4：指令已完成
        //  999：指令发送失败
        //参数startTime: 类型String, 参数可以为空
        //  描述:精确到毫秒的时间戳
        //参数endTime: 类型String, 参数可以为空
        //  描述:精确到毫秒的时间戳
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryRemoteControlList(string appKey, string appSecret, string MasterKey, string productId, string searchValue = "", string type = "", string status = "", string startTime = "", string endTime = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_device_control/controls";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("productId", productId);
            param.Add("searchValue", searchValue);
            param.Add("type", type);
            param.Add("status", status);
            param.Add("startTime", startTime);
            param.Add("endTime", endTime);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20190507012630";

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
        public static string CreateRemoteControl(string appKey, string appSecret, string MasterKey, string body)
        {
            string path = "/aep_device_control/control";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("MasterKey", MasterKey);

            Dictionary<string, string> param = null;
            string version = "20181031202247";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }

    }
}
