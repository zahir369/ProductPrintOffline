using System;
using System.Collections.Generic;

namespace MKSS.Service.UIBiaoDing
{
    public class SerialNoRule
    {
        static Dictionary<SerialNoRuleEnnum,SerialNoRule> all = new Dictionary<SerialNoRuleEnnum, SerialNoRule>();
        public static Dictionary<SerialNoRuleEnnum, SerialNoRule> ALL
        {
            get {
                if (all.Count == 0) {
                    foreach (var item in Enum.GetValues(typeof(SerialNoRuleEnnum)))
                    {
                        all.Add((SerialNoRuleEnnum)item,new SerialNoRule((SerialNoRuleEnnum)item)  );
                    }
                }
                return all;
            }
        }
        public SerialNoRule( )
        { 
        }
        public SerialNoRule(SerialNoRuleEnnum r) {
            Rule = r;
            RuleString = ToString();
        }
        public SerialNoRuleEnnum Rule { get; set; }
        public string RuleString 
        {
            get;
            set;
        }
        public override string ToString()
        {
            switch (Rule)
            {
                case SerialNoRuleEnnum.COMMON_12:
                    return "通用12位编号";
                case SerialNoRuleEnnum.JK_12_CH4:
                    return "专用14位定制编号（甲烷）";
                case SerialNoRuleEnnum.JK_12_CO:
                    return "专用14位定制编号（CO）";
                case SerialNoRuleEnnum.JK_12_CH4_CO:
                    return "专用14位定制编号（甲烷+CO）";
                case SerialNoRuleEnnum.NONE:
                    return "未知";
                default:
                    break;
            }
            return base.ToString();
        }
    }


    public class SerialNoNet
    {
        static Dictionary<SerialNoNetEnnum,SerialNoNet> all = new Dictionary<SerialNoNetEnnum, SerialNoNet>();
        public static Dictionary<SerialNoNetEnnum, SerialNoNet> ALL
        {
            get
            {
                if (all.Count == 0)
                {
                    foreach (var item in Enum.GetValues(typeof(SerialNoNetEnnum)))
                    {
                        all.Add((SerialNoNetEnnum)item,new SerialNoNet((SerialNoNetEnnum)item) );
                    }
                }
                return all;
            }
        }
        public SerialNoNet( )
        { 
        }
        public SerialNoNet(SerialNoNetEnnum r)
        {
            Net = r;
            NetString = ToString();
        }
        public SerialNoNetEnnum Net { get; set; }
        public string NetString
        {
            get;
            set;
        }
        public override string ToString()
        {
            switch (Net)
            {
                case SerialNoNetEnnum.NO_NET:
                    return "非NB";
                case SerialNoNetEnnum.NB:
                    return "NB";
                case SerialNoNetEnnum.NONE:
                    return "未知";
                default:
                    break;
            }
            return base.ToString();
        }
    }


    public enum SerialNoRuleEnnum
    {
        NONE = -1,
        COMMON_12 = 0,
        JK_12_CH4 = 1,
        JK_12_CO = 2,
        JK_12_CH4_CO = 3
    }
    public enum SerialNoNetEnnum
    {
        NONE = -1,
        NO_NET = 0,
        NB = 1, 
    }

}
 
