using AepSdk.Apis.Core;
using System.Collections.Generic;


namespace AepSdk.Apis
{
    class Aep_public_product_management
    {
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryPublicByPublicProductId(string appKey, string appSecret, string body)
        {
            string path = "/aep_public_product_management/publicProducts";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20190507003930";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string QueryPublicByProductId(string appKey, string appSecret, string body)
        {
            string path = "/aep_public_product_management/publicProductList";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20190507004139";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数body: 类型json, 参数不可以为空
        //  描述:body,具体参考平台api说明
        public static string InstantiateProduct(string appKey, string appSecret, string body)
        {
            string path = "/aep_public_product_management/instantiateProduct";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = null;
            string version = "20200801233037";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, body, version, application, key, "POST");
            if (response != null)
                return response;
            return null;
        }
        //参数searchValue: 类型String, 参数可以为空
        //  描述:设备类型、厂商ID、厂商名称
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        public static string QueryAllPublicProductList(string appKey, string appSecret, string searchValue = "", string pageNow = "", string pageSize = "")
        {
            string path = "/aep_public_product_management/allPublicProductList";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);

            string version = "20200829005548";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }
        //参数searchValue: 类型String, 参数可以为空
        //  描述:产品名称
        //参数pageNow: 类型long, 参数可以为空
        //  描述:当前页数
        //参数pageSize: 类型long, 参数可以为空
        //  描述:每页记录数
        //参数productIds: 类型String, 参数可以为空
        //  描述:私有产品idList
        public static string QueryMyPublicProductList(string appKey, string appSecret, string searchValue = "", string pageNow = "", string pageSize = "", string productIds = "")
        {
            string path = "/aep_public_product_management/myPublicProductList";
            Dictionary<string, string> headers = null;
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("searchValue", searchValue);
            param.Add("pageNow", pageNow);
            param.Add("pageSize", pageSize);
            param.Add("productIds", productIds);

            string version = "20200829005359";

            string application = appKey;
            string key = appSecret;


            string response = AepHttpRequest.SendAepHttpRequest(path, headers, param, null, version, application, key, "GET");
            if (response != null)
                return response;
            return null;
        }

    }
}
