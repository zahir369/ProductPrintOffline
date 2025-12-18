using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using SqlSugar;
using System.Reflection;

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
            foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
            {
                Data.Add(item, new SensorDataItem(this, item));
            }
        }

        static Dictionary<PosEnum, PropertyInfo> ValuePropertyInfoCache = new Dictionary<PosEnum, PropertyInfo>();//缓存 PropertyInfo
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
        }


        /// <summary>
        ///  某位置值
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public SensorDataItem F_DataValue(PosEnum position) {
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
            double d = (double)ValuePropertyInfoCache[position].GetValue(this);
            if (ShowV) {
                return CalcDataValueByLoad(ToV(d));
            }
            return CalcDataValueByLoad(d);
        }


        /// <summary>
        ///   不同负载时表现出来的电压值
        /// </summary>		
        public string F_LoadDataValueString(PosEnum position)
        {
            double d = (double)ValuePropertyInfoCache[position].GetValue(this);
            if (ShowV)
            {
                return CalcDataValueByLoad(ToV(d)).ToString("0.000");
            }
            return CalcDataValueByLoad(d).ToString("0");
        }

        public double ToV(double d) {
            return double.Parse((d * 3.3 / 4096).ToString("f3")) - 0.3;
        }

        /// <summary>
        ///  根据负载计算测量电压
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueByLoad(double F_DataValue) { 
            if (F_DataValue == 0 || F_DataValue == double.MinValue) return 0;
            if (Resistance == ResistanceDedault) return F_DataValue;//默认负载，直接返回
            double I = F_DataValue / ResistanceDouble(ResistanceDedault);//回路电流
            double Rs = (5 / I) - ResistanceDouble(ResistanceDedault);//传感器电阻
            double Rv = ResistanceDouble(Resistance);//虚拟电阻
            double Ix = 5 / (Rs + Rv);//虚拟回路电流
            double Vx = Ix * Rv;
            return double.Parse(Vx.ToString("f4"));
        }

        /// <summary>
        ///  显示电压还是原始AD
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public static bool ShowV { get; set; }
        /// <summary>
        ///  负载，默认 4.7 K
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public static ResistanceEnum Resistance { get; set; } = ResistanceEnum.R4_700K;
        [SugarColumn(IsIgnore = true)]
        public static ResistanceEnum ResistanceDedault { get; set; } = ResistanceEnum.R4_700K;

        public static string ResistanceString(ResistanceEnum e)
        {
            double i = ((double)(int)e) / 1000;
            return i + "K";
        }
        public static double ResistanceDouble(ResistanceEnum e)
        {
            return ((double)(int)e);
        }


        public static double Delta(SensorGroupData start, SensorGroupData end, PosEnum position)
        {
            if (end == null || start == null) return 0;
            return (start.F_LoadDataValue(position)) - (end.F_LoadDataValue(position));
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
        public SensorDataItem(SensorGroupData g, PosEnum p ) {
            SensorGroupData = g;
            PosEnum = p; 
        }
        public SensorGroupData SensorGroupData { get; set; }
        public PosEnum PosEnum { get; set; }
        public double F_AddTime { get { return SensorGroupData.F_AddTime; } }
        public double F_DataValue { get { return SensorGroupData.F_LoadDataValue(PosEnum); } }
        public bool F_DataValueEmp { get { return SensorGroupData.F_DataValueEmp(PosEnum); } }
        public string F_SensorId { get { return Sensor.CreateCensorId(SensorGroupData.F_BatchId, PosEnum); } }
        public double F_LoadDataValue { get { return F_DataValue; } }
        public string PosString { get { return PosEnum.ToString(); } }
    }

    /// <summary>
    ///  电阻值
    /// </summary>
    public enum ResistanceEnum
    {
        R1_500K = 500, R1_000K = 1000, R2_000K = 2000,
        R3_900K = 3900, R4_700K = 4700,
        R10_000K = 10000, R20_000K = 20000, R47_000K = 47000,
        R100_000K = 100000, R200_000K = 200000, R470_000K = 470000,
    }
    public enum RegionEnum { A = 100, B = 200, C = 300, D = 400 }
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
}