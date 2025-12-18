using System;
using System.Collections.Generic;

namespace WpfChartTest.Model
{
    public class SensorGroupData
    {

        public SensorGroupData(ushort[] arr,double time){
            int i = 0; F_AddTime = time;
             Data = new Dictionary<PosEnum, SensorDataItem>();
            foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
            {
                SensorDataItem d = new SensorDataItem() { 
                 PosEnum = item, F_AddTime= time , F_LoadDataValue= (double)arr[i]
                };
                Data.Add(d.PosEnum, d);
                i++;
            }
        }

        public SensorGroupData(float arr, double time)
        {
            int i = 0; F_AddTime = time;
            Data = new Dictionary<PosEnum, SensorDataItem>();
            foreach (PosEnum item in Enum.GetValues(typeof(PosEnum)))
            {
                SensorDataItem d = new SensorDataItem()
                {
                    PosEnum = item,
                    F_AddTime = time,
                    F_LoadDataValue = (double)arr 
                };
                Data.Add(d.PosEnum, d);
                i++;
            }
        }


        public double F_AddTime { get; set; }
        public Dictionary<PosEnum, SensorDataItem> Data { get; set; }
         


        public static ShowModeEnum ShowMode { get; internal set; }
        public static PosEnum PosEnum(string pos)
        {
            return (PosEnum)Enum.Parse(typeof(PosEnum), pos);
        }

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

         

    }

}
