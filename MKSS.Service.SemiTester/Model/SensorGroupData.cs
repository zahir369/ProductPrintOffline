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
        public double A17 { get; set; }
        public double A18 { get; set; }
        public double A19 { get; set; }
        public double A20 { get; set; }
        public double A21 { get; set; }
        public double A22 { get; set; }
        public double A23 { get; set; }
        public double A24 { get; set; }
        public double A25 { get; set; }
        public double A26 { get; set; }
        public double A27 { get; set; }
        public double A28 { get; set; }
        public double A29 { get; set; }
        public double A30 { get; set; }
        public double A31 { get; set; }
        public double A32 { get; set; }
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
        public double B17 { get; set; }
        public double B18 { get; set; }
        public double B19 { get; set; }
        public double B20 { get; set; }
        public double B21 { get; set; }
        public double B22 { get; set; }
        public double B23 { get; set; }
        public double B24 { get; set; }
        public double B25 { get; set; }
        public double B26 { get; set; }
        public double B27 { get; set; }
        public double B28 { get; set; }
        public double B29 { get; set; }
        public double B30 { get; set; }
        public double B31 { get; set; }
        public double B32 { get; set; }


        public SensorGroupData()
        {
            InitPropertyInfos();//缓存 PropertyInfo
            Data = new Dictionary<PosEnum, SensorDataItem>();
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


        public static string ShowDesc(SensorGroupData g, PosEnum p)
        { 
            return String.Format("{0}负载：{1} V；",
                ResistanceString(Resistance),
                g.F_LoadDataValue(p)
            );
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
            return CalcDataValueByLoad(F_DataValue(position).F_DataValue);
        }

        /// <summary>
        ///  根据负载计算测量电压
        /// </summary>
        /// <param name="F_DataValue"></param>
        /// <returns></returns>
        public static double CalcDataValueByLoad(double F_DataValue) { 
            if (F_DataValue == 0 || F_DataValue == double.MinValue) return 0;
            if (Resistance == ResistanceDedault) return F_DataValue;//默认负载，直接返回

            double v_total = Service.SemiTester.SemiTesterService.VersionVoltage;
            double I = F_DataValue / ResistanceDouble(ResistanceDedault);//回路电流
            double Rs = (v_total / I) - ResistanceDouble(ResistanceDedault);//传感器电阻
            double Rv = ResistanceDouble(Resistance);//虚拟电阻
            double Ix = v_total / (Rs + Rv);//虚拟回路电流
            double Vx = Ix * Rv;
            return double.Parse(Vx.ToString("f4"));
        }

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
        public SensorDataItem(SensorGroupData g, PosEnum p, double time) {
            SensorGroupData = g;
            PosEnum = p;
            F_AddTime = time;
        }
        public SensorGroupData SensorGroupData { get; set; }
        public PosEnum PosEnum { get; set; }
        public double F_AddTime { get { return SensorGroupData.F_AddTime; } set { } }
        public double F_DataValue { get; set; }
        public bool F_DataValueEmp { get { return SensorGroupData.F_DataValueEmp(PosEnum); } }
        public string F_SensorId { get { return Sensor.CreateCensorId(SensorGroupData.F_BatchId, PosEnum); } }
        public double F_LoadDataValue { get { return SensorGroupData.CalcDataValueByLoad(F_DataValue); } }
        public string PosString { get { return PosEnum.ToString(); } }
    }

    /// <summary>
    ///  电阻值
    /// </summary>
    public enum ResistanceEnum
    {
        R1_500K = 500, R1_000K = 1000, R2_000K = 2000, R2_200K = 2200,
        R3_900K = 3900, R4_700K = 4700, R6_800K = 6800,
        R10_000K = 10000, R20_000K = 20000, R47_000K = 47000,
        R100_000K = 100000, R200_000K = 200000, R470_000K = 470000,
    }
    public enum RegionEnum { A  =100,B=200 }
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
        A17 = 117,
        A18 = 118,
        A19 = 119,
        A20 = 120,
        A21 = 121,
        A22 = 122,
        A23 = 123,
        A24 = 124,
        A25 = 125,
        A26 = 126,
        A27 = 127,
        A28 = 128,
        A29 = 129,
        A30 = 130,
        A31 = 131,
        A32 = 132,
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
        B17 = 217,
        B18 = 218,
        B19 = 219,
        B20 = 220,
        B21 = 221,
        B22 = 222,
        B23 = 223,
        B24 = 224,
        B25 = 225,
        B26 = 226,
        B27 = 227,
        B28 = 228,
        B29 = 229,
        B30 = 230,
        B31 = 231,
        B32 = 232,

    }

}