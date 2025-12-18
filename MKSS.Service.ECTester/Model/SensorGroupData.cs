using System;
using System.Text;
using System.Collections.Generic;
using SqlSugar;
using System.Reflection;
using Newtonsoft.Json;
using MKSS.Service.ECTester;

namespace MKSS.Model
{


    [SugarTable("pd_sensorgroupdata")]
    public class SensorGroupData
    {

        /// <summary>
        /// F_DataId
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string F_DataId { get; set; }

        /// <summary>
        /// 检测批次号
        /// </summary>		
        public long F_BatchId { get; set; }
        /// <summary>
        /// F_AddTime
        /// </summary>		
        public double F_AddTime { get; set; }

        public double A1 { get; set; }
        public double A2 { get; set; }
        public double A3 { get; set; }
        public double A4 { get; set; }
        public double A5 { get; set; }
        public double A6 { get; set; }
        public double A7 { get; set; }
        public double A8 { get; set; }
        public double A9 { get; set; }
        public double A10 { get; set; }
        public double A11 { get; set; }
        public double A12 { get; set; }
        public double A13 { get; set; }
        public double A14 { get; set; }
        public double A15 { get; set; }
        public double A16 { get; set; }

        public double B1 { get; set; }
        public double B2 { get; set; }
        public double B3 { get; set; }
        public double B4 { get; set; }
        public double B5 { get; set; }
        public double B6 { get; set; }
        public double B7 { get; set; }
        public double B8 { get; set; }
        public double B9 { get; set; }
        public double B10 { get; set; }
        public double B11 { get; set; }
        public double B12 { get; set; }
        public double B13 { get; set; }
        public double B14 { get; set; }
        public double B15 { get; set; }
        public double B16 { get; set; }


        public double C1 { get; set; }
        public double C2 { get; set; }
        public double C3 { get; set; }
        public double C4 { get; set; }
        public double C5 { get; set; }
        public double C6 { get; set; }
        public double C7 { get; set; }
        public double C8 { get; set; }
        public double C9 { get; set; }
        public double C10 { get; set; }
        public double C11 { get; set; }
        public double C12 { get; set; }
        public double C13 { get; set; }
        public double C14 { get; set; }
        public double C15 { get; set; }
        public double C16 { get; set; }

        public double D1 { get; set; }
        public double D2 { get; set; }
        public double D3 { get; set; }
        public double D4 { get; set; }
        public double D5 { get; set; }
        public double D6 { get; set; }
        public double D7 { get; set; }
        public double D8 { get; set; }
        public double D9 { get; set; }
        public double D10 { get; set; }
        public double D11 { get; set; }
        public double D12 { get; set; }
        public double D13 { get; set; }
        public double D14 { get; set; }
        public double D15 { get; set; }
        public double D16 { get; set; }

        public SensorGroupData()
        {
            InitPropertyInfos();//缓存 PropertyInfo
            Data = new Dictionary<PosEnum, SensorDataItem>();
        }

 
        public static Dictionary<PosEnum, PropertyInfo> ValuePropertyInfoCache = new Dictionary<PosEnum, PropertyInfo>();//缓存 PropertyInfo
        static void InitPropertyInfos()
        {
            if (ValuePropertyInfoCache.Count == 0)
            {
                Dictionary<PosEnum, PropertyInfo> ValuePropertyInfoCachetemp = new Dictionary<PosEnum, PropertyInfo>();
                foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
                {
                    PropertyInfo pro = typeof(SensorGroupData).GetProperty(item.ToString());
                    ValuePropertyInfoCachetemp.Add(item, pro);
                }
                ValuePropertyInfoCache = ValuePropertyInfoCachetemp;
            }
        }

        public Dictionary<PosEnum, SensorDataItem> Data { get; set; }
        /// <summary>
        ///  某位置值
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void SetDataValue(PosEnum position,double val)
        {
            ValuePropertyInfoCache[position].SetValue(this, val);
            if (!Data.ContainsKey(position))
            {
                SensorDataItem ret = new SensorDataItem(this,position,this.F_AddTime)
                {
                    F_DataValue = (double)ValuePropertyInfoCache[position].GetValue(this) 
                };
                Data.Add(position, ret);
            }
            else {
                Data[position].F_AddTime = F_AddTime;
                Data[position].F_DataValue = val;
            }
        }


        /// <summary>
        ///  某位置值
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SensorDataItem F_DataValue(PosEnum position) {
            if (!Data.ContainsKey(position)) {
                SensorDataItem ret = new SensorDataItem(this, position, this.F_AddTime)
                {
                    F_DataValue = (double)ValuePropertyInfoCache[position].GetValue(this),
                };
                return ret;
            }
            return Data[position];
        }

        /// <summary>
        ///  某位置唯一ID
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static string F_SensorId(long F_BatchId ,PosEnum position)
        {
            return Sensor.CreateCensorId(F_BatchId, position.ToString().Substring(0, 1), int.Parse(position.ToString().Substring(1))); ;
        }
        

        /// <summary>
        ///  空数据计算，超过 五次空数据，那么认可空数据值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Sensor Sensor { get; set; }

        public bool F_DataValueEmp(PosEnum position) {
            double d = F_DataValue(position).F_DataValue;
            return d == 0 || d == double.MinValue; 
        }

        /// <summary>
        ///   不同负载时表现出来的电压值
        /// </summary>		
        public double F_LoadDataValue(PosEnum position) 
        {
            return CalcDataValueByLoad(this, position);
        }


        /// <summary>
        ///  灵敏度(nA/PPM)
        /// </summary>		
        public double F_Sensibility(PosEnum position)
        {
            //根据模式切换布局
            if (ECTesterService.IndustryMode)
            {
                //工业传感器 8 * 8， 4 * * 
                double sensibility = 3.856 *  (F_DataValue(position).F_DataValue - SensorGroupDataStander.ZeroStd) / (double)ECTesterConfgig.ContainerPPM;//灵敏度(nA/PPM)
                return Math.Round(sensibility, 1);
            }
            else
            {
                //水性传感器 16 * 4， 8 * 2
                double voltage = SensorGroupDataStander.CalcDataValueDYDY(this, position);//端电压 mV
                double sensibility = voltage * Math.Pow(10, 6) / (double)(SensorGroupDataStander.InnerResistance) / (double)ECTesterConfgig.ContainerPPM;//灵敏度(nA/PPM)
                return Math.Round(sensibility, 1);
            }
             

        }
        

        /// <summary>
        ///  根据负载，显示类别计算测量值
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueByLoad(SensorGroupData group, PosEnum position) {
            SensorDataItem data = group.F_DataValue(position);
            if (data==null || data.F_DataValue == 0 || data.F_DataValue == double.MinValue) return 0;
            switch (ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    return SensorGroupDataStander.CalcDataValueDYDY(group, position);//默认AD
                case ShowModeEnum.YuLiu:
                    return SensorGroupDataStander.CalcDataValueYuLiu(group, position);//默认AD
                case ShowModeEnum.AD:
                    return data.F_DataValue;//默认AD
                default:
                    break;
            } 
            return 0;
        }


        /// <summary>
        ///  根据负载，显示类别计算测量值
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueByLoad(SensorGroupData group, PosEnum position, ShowModeEnum mode)
        {
            SensorDataItem data = group.F_DataValue(position);
            if (data == null || data.F_DataValue == 0 || data.F_DataValue == double.MinValue) return 0;
            switch (mode)
            {
                case ShowModeEnum.DuanDianYa:
                    return SensorGroupDataStander.CalcDataValueDYDY(group, position);//默认AD
                case ShowModeEnum.YuLiu:
                    return SensorGroupDataStander.CalcDataValueYuLiu(group, position);//默认AD
                case ShowModeEnum.AD:
                    return data.F_DataValue;//默认AD
                default:
                    break;
            }
            return 0;
        }

        /// <summary>
        ///  显示类别，默认 端电压 
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public static ShowModeEnum ShowMode { get; set; } 
        [SugarColumn(IsIgnore = true)]
        public static string ShowModeDesc { get { return ShowModeString(ShowMode); } }
        [SugarColumn(IsIgnore = true)]
        public static string ShowModeDescXX { get { return ShowModeStringXX(ShowMode); } }
        
        public static string ShowModeString(ShowModeEnum e)
        {
            switch (e)
            {
                case ShowModeEnum.DuanDianYa:
                    return "端电压";
                case ShowModeEnum.YuLiu:
                    return "预留浓度";
                case ShowModeEnum.AD:
                    return "AD值";
                default:
                    break;
            }
            return "未知";
        }


        public static string ShowModeStringXX(ShowModeEnum e)
        {
            switch (e)
            {
                case ShowModeEnum.DuanDianYa:
                    return "端电压（毫伏）";
                case ShowModeEnum.YuLiu:
                    return "预留浓度（%）";
                case ShowModeEnum.AD:
                    return "AD值（-）";
                default:
                    break;
            }
            return "未知";
        }


        public static string ShowDesc(SensorGroupData g, PosEnum p)
        {
            SensorStaData sta1 = SensorStaDataCache.Instance[p];
            return String.Format("{1}；{2}；第{6}秒；", 
                ShowValue(ShowModeEnum.YuLiu,g,p),
                ShowValue(ShowModeEnum.DuanDianYa, g, p), 
                ShowValue(ShowModeEnum.AD, g, p),
                p,
                sta1.T90,
                sta1.T10,
                g.F_AddTime.ToString("f1")
            );
        }

        public static string ShowValue(ShowModeEnum e, SensorGroupData g,PosEnum p)
        {
            switch (e)
            {
                case ShowModeEnum.DuanDianYa:
                    return string.Format("端电压：{0}毫伏", CalcDataValueByLoad(g, p, e));
                case ShowModeEnum.YuLiu:
                    return string.Format("预留浓度：{0}%", CalcDataValueByLoad(g, p, e));
                case ShowModeEnum.AD:
                    return string.Format("AD值：{0}", CalcDataValueByLoad(g, p, e));
                default:
                    break;
            }
            return "未知";
        }


        public static double T90(SensorGroupData start, PosEnum position)
        {
            if (start == null) return 0;
            return SensorStaDataCache.Instance[position].T90;
        }
        public static double T10(SensorGroupData start, PosEnum position)
        {
            if (start == null) return 0;
            return SensorStaDataCache.Instance[position].T10;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (PosEnum pos in ValuePropertyInfoCache.Keys)
            {
                s.Append(ValuePropertyInfoCache[pos].GetValue(this));
            }
            return string.Format("{0}:{1},{2}", this.F_DataId,this.F_AddTime, s);
        }

        /// <summary>
        ///  获取位置号.
        ///   Sensor.CreateCensorId(F_BatchId, position.ToString().Substring(0, 1), int.Parse(position.ToString().Substring(1)));
        /// </summary>
        /// <param name="region"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static PosEnum Pos(RegionEnum region, int pos)
        {
            return (PosEnum)((region == RegionEnum.A ? 100 : 200) + pos);
        }
        public static RegionEnum Region(string pos)
        {
            return  pos.ToString().Substring(0, 1) == "A" ? RegionEnum.A : RegionEnum.B;
        }
        public static PosEnum PosEnum(string pos)
        {
            return (PosEnum)Enum.Parse(typeof(PosEnum) ,pos);
        }

    }

    public class SensorDataItem
    {
        public SensorDataItem(SensorGroupData g, PosEnum p, double time) {
            SensorGroupData = g;
            PosEnum = p;
            F_AddTime = time;

            Random rd = new Random();
            double temp = (double)rd.Next(0, 600);
            F_LoadDataValueRandom = temp;
        }
        public SensorGroupData SensorGroupData { get; set; }
        public PosEnum PosEnum { get; set; }
        public double F_AddTime { get { return SensorGroupData.F_AddTime; } set { } }
        public double F_DataValue { get; set; }
        /// <summary>
        ///  曲线拟合数据
        /// </summary>
        public double F_DataValueExponential { get; set; }
        public bool F_DataValueEmp { get { return SensorGroupData.F_DataValueEmp(PosEnum); } }
        public string F_SensorId { get { return Sensor.CreateCensorId(SensorGroupData.F_BatchId, PosEnum); } }
        public double F_LoadDataValue { get { return SensorGroupData.CalcDataValueByLoad(SensorGroupData, PosEnum); } }
        public double F_LoadDataValueRandom { get; set; }
        public string PosString { get { return PosEnum.ToString(); } }

        public override string ToString()
        {
            return string.Format("{0}:{1},{2}",this.F_AddTime,this.F_DataValue,this.F_DataValueExponential);
        }

        public string ToStringTD()
        {
            return string.Format("{0}:{1}", this.F_AddTime, this.F_DataValue);
        }
    }

    /// <summary>
    ///  显示模式
    /// </summary>
    public enum ShowModeEnum
    {
        DuanDianYa = 1, YuLiu = 2, AD = 3, 
    }
    public enum RegionEnum { A  =100,B=200, C = 300, D = 400 }
    public enum PosEnum
    {
        A1 = 101,
        A2 = 102,
        A3 = 103,
        A4 = 104,
        A5 = 105,
        A6 = 106,
        A7 = 107,
        A8 = 108,
        A9 = 109,
        A10 = 110,
        A11 = 111,
        A12 = 112,
        A13 = 113,
        A14 = 114,
        A15 = 115,
        A16 = 116,
        B1 = 201,
        B2 = 202,
        B3 = 203,
        B4 = 204,
        B5 = 205,
        B6 = 206,
        B7 = 207,
        B8 = 208,
        B9 = 209,
        B10 = 210,
        B11 = 211,
        B12 = 212,
        B13 = 213,
        B14 = 214,
        B15 = 215,
        B16 = 216,
        C1 = 301,
        C2 = 302,
        C3 = 303,
        C4 = 304,
        C5 = 305,
        C6 = 306,
        C7 = 307,
        C8 = 308,
        C9 = 309,
        C10 = 310,
        C11 = 311,
        C12 = 312,
        C13 = 313,
        C14 = 314,
        C15 = 315,
        C16 = 316,
        D1 = 401,
        D2 = 402,
        D3 = 403,
        D4 = 404,
        D5 = 405,
        D6 = 406,
        D7 = 407,
        D8 = 408,
        D9 = 409,
        D10 = 410,
        D11 = 411,
        D12 = 412,
        D13 = 413,
        D14 = 414,
        D15 = 415,
        D16 = 416,

    }

    public class SensorGroupDataStander
    {
        //水性 主要看端电压 -0.1-0.6，0-4096，0-0.1 0.1-  
        //工业 主要看AD 700           0-3000，370,(0-4096)
        public static double ZeroStd = ECTesterService.IndustryMode ? (double)370 : (double)370;
        public static double StdPulse = ECTesterService.IndustryMode ? ((double)12 / (double)1556) : (double)0.3 / (double)379;//755 0.3mv  60//1556  12/1556
        public static double InnerResistance = ECTesterService.IndustryMode ? 200 * 1000 : 1000;

        public static double YuLiuStd = 20.9;
        /// <summary>
        ///   基准
        /// </summary>
        public static SensorGroupData STD = null;
        /// <summary>
        ///  根据ad计算预留浓度
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueYuLiu(SensorGroupData real, PosEnum position)
        {
            double d = (double)SensorGroupData.ValuePropertyInfoCache[position].GetValue(real);
            if (d == 0 || d == double.MinValue) return 0;
            if (STD!=null && STD.F_DataValue(position).F_DataValue <3700)
            {
                SensorGroupData std = STD;
                double d_std = (double)SensorGroupData.ValuePropertyInfoCache[position].GetValue(std);
                return double.Parse((YuLiuStd * (d) * 3.3 / 4096).ToString("f1"));
            }
            return 0;
        }


        /// <summary>
        ///   计算气体传感器端电压，单位毫伏
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueDYDY(SensorGroupData real, PosEnum position)
        {
            //double F_DataValue = (double)SensorGroupData.ValuePropertyInfoCache[position].GetValue(real);
            //if (F_DataValue == 0 || F_DataValue == double.MinValue) return 0;
            //double v = F_DataValue * 3.3 / 4096;
            //return double.Parse(v.ToString("f2"));
            double F_DataValue = (double)SensorGroupData.ValuePropertyInfoCache[position].GetValue(real);
            if (F_DataValue == 0 || F_DataValue == double.MinValue) return 0;
            double v = (F_DataValue - ZeroStd) * StdPulse;
            return double.Parse(v.ToString("f2"));
        }

        public static double SpanTimeLast = 0;
        public static void SetSpan(SensorGroupData gds, double span)
        {
            SpanTimeLast = span;
            STD = gds;
        }
        public static void ClearSpan()
        {
            SpanTimeLast = 0;
            STD = null;
        }
        public static void SetSpan(SensorGroupData item)
        {
            if (item.F_AddTime == SpanTimeLast)
            {
                STD = item;
            }
        }


    }
}