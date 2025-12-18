using MKSS.Core.Common.HttpRestSharp;
using MKSS.Model;
using RestSharp;
using SqlSugar;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Newtonsoft.Json;
using System.Windows;

namespace MKSS.APP.UIBiaoDing.Util
{
    /// <summary>
    /// 企业信息化接口调用
    /// </summary> 
    public class IotEnInfoController
    {
        public IotEnInfoController()
        {

        }

        public static EnInfoLoginToken Token = null;
        public static EnInfoLoginToken LoginByPassword(string uname, string upwd)
        {

            try
            {
                if (Token == null || DateTime.Now >= Token.ExpireDate)
                {
                    Token = null;
                    var IotEnInfoAddr = ConfigurationManager.AppSettings["IotEnInfoAddr"];
                    //var IotEnInfoAppkey = ConfigurationManager.AppSettings["IotEnInfoAppkey"];
                    //var IotEnInfoAppsercert = ConfigurationManager.AppSettings["IotEnInfoAppsercert"];
                    var apiController = "/api/sysbase/Login/Login";
                    var IotEnInfoLogin = IotEnInfoAddr + apiController;
                    var loginBody = new EnInfoLoginBody
                    {
                        Account = uname,
                        Password = upwd,
                        Endpoint = EnumEnInfoEndpoint.Desktop
                    };
                    RestRequest req = new RestRequest($"{IotEnInfoLogin}", Method.POST);
                    req.AddHeader("Content-Type", "application/json");
                    req.AddJsonBody(loginBody);

                    var client = new RestClient(IotEnInfoAddr);
                    var request = client.Execute(req);


                    if (request.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(request.ErrorMessage);
                    }

                    IotYunResult<EnInfoLoginToken> temp = JsonConvert.DeserializeObject<IotYunResult<EnInfoLoginToken>>(request.Content);

                    if (temp == null || temp.Data == null) return null;

                    temp.Data.ExpireDate = DateTime.Now.AddSeconds(temp.Data.Expires_In);

                    Token = temp.Data;
                }

                return Token;

            }
            catch (Exception ex)
            {
                MessageBox.Show("登录信息化平台出错：" + ex);
            }

            return null;
        }

        /// <summary>
        /// 获取生产任务单列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>

        public static List<QueryOrderListItem> GetOrderList()
        {
            List<QueryOrderListItem> list = new List<QueryOrderListItem>();

            try
            {
                if (Token == null)
                {
                    MessageBox.Show("登录状态失效，请重新登录！");
                    return null;
                }

                var IotEnInfoAddr = ConfigurationManager.AppSettings["IotEnInfoAddr"];
                var apiController = "/api/sysproduct/BusiProductGjporder/GetAll";
                var IotEnInfoGetProductModels = IotEnInfoAddr + apiController;
                string param = string.Format("key={0}", "");

                var client = new RestClient(IotEnInfoAddr);
                client.AddDefaultHeader("Authorization", $"Bearer {Token.Token}");

                var request = client.Execute(new RestRequest($"{IotEnInfoGetProductModels}?{param}", Method.GET));
                if (request.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(request.ErrorMessage);
                }

             
                IotYunResult<List<QueryOrderListItem>> temp = JsonConvert.DeserializeObject<IotYunResult<List<QueryOrderListItem>>>(request.Content);

                if (temp == null || temp.Data == null) return null;
                list =  temp.Data;

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("获取生产任务单列表出错：" + ex);
            }

        }





    }


}
