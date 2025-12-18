using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MKSS.APP.BiaodingAlcohol.UIBiaoDing.Util.Beans
{
    public class LoginBean
    {
        /// <summary>
        /// 
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int HttpStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data Data { get; set; }
       
    }
    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public string success { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
    }
}
