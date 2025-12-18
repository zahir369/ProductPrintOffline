using System;
using System.Collections.Generic;

namespace MKSS.APP.MK102Reader
{
    public class CommandAbs { 
        public const string ResultEmpString = "/";
        private CommandAbs() { 
        
        }
        public static List<SensorEntity> Sensors = new List<SensorEntity>();
        public static Dictionary<CommandType, CommandAbs> Cache = new Dictionary<CommandType, CommandAbs>();
        public static CommandAbs Create(SensorType s, CommandType c , int l) {
            
            if (!Cache.ContainsKey(c)) {
                Cache.Add(c,new CommandAbs(){ Command=c, Length=l, Sensor=s });
            }
            return Cache[c];
        }
        public static CommandAbs Of(CommandType c)
        {
            if (!Cache.ContainsKey(c))
            {
                return null;
            }
            return Cache[c];
        }

        public SensorType Sensor {  get; set; }
        public CommandType Command {   get; set; }
        public int Length {   get; set; } = 1;
        public byte[] Response { get; set; }
        public bool Reponsed { get { return Response != null && Response.Length > 0; } }

        /// <summary>
        ///  探测器状态
        /// </summary>
        /// <returns></returns>
        public SensorStatus[] ToSensorStatus() {
            /**
                1.读取1号控制器的1号探测器状态：21
                发→◇01 03 00 01 00 08 15 CC 
                收←◆01 03 10 00 0A 00 01 00 01 00 01 00 01 00 01 00 01 00 01 D8 B3
             ***/
            SensorStatus[] ret = new SensorStatus[8];
            for (int i = 1; i <= 8; i++)
            {
                if (Response != null && Response.Length >= 3 + i * 2)
                {
                    ret[i - 1] = (SensorStatus)Response[3 + i * 2 - 1];
                }
                else {
                    ret[i - 1] = SensorStatus.未知;
                }
            }
            return ret;
        }


        /// <summary>
        ///  控制器的状态
        /// </summary>
        /// <returns></returns>
        public List<ControllerStatus> ToControllerStatus()
        {
            /**
                3.读取1号控制器的状态 6
                发→◇01 02 00 01 00 04 28 09 □
                收←◆01 02 01 00 A1 88
                bit0就是代表备电，bit1代表主电
             ***/
            List<ControllerStatus> ret = new List<ControllerStatus>();
            if (Response != null && Response.Length >= 4)
            {
                string strTemp = System.Convert.ToString(Response[3], 2);
                strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                if (strTemp.Substring(4, 1) == "1")   ret.Add(ControllerStatus.S1对地短路);
                if (strTemp.Substring(5, 1) == "1")   ret.Add(ControllerStatus.总线电源短路);
                if (strTemp.Substring(6, 1) == "1")   ret.Add(ControllerStatus.主电故障);
                if (strTemp.Substring(7, 1) == "1")   ret.Add(ControllerStatus.备电故障);
            }
            if (ret.Count == 0) ret.Add(ControllerStatus.正常);
            return ret;
        }

        /// <summary>
        ///  探测器数值
        /// </summary>
        /// <returns></returns>
        public double[] ToSensorValue()
        {
            /**
                    2.读取1号控制器的1号探测器数值：21
                    发→◇01 04 00 01 00 08 A0 0C 
                    收←◆01 04 10 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 55 2C
                    注：bit15表示指数的符号位；bit14~13表示指数位数；bit12~0表示数值。
                    如：0xA019，表示25 * 10^（-1）= 2.5
             ***/
            double[] ret = new double[8];
            for (int i = 1; i <= 8; i++)
            {
                if (Response != null && Response.Length >= 3 + i * 2)
                {
                    byte[] bytesTest = new byte[] { Response[3 + i * 2 - 2], Response[3 + i * 2 - 1] };
                    string strResult = "";
                    for (int b = 0; b < bytesTest.Length; b++)
                    {
                        string strTemp = System.Convert.ToString(bytesTest[b], 2);
                        strTemp = strTemp.Insert(0, new string('0', 8 - strTemp.Length));
                        strResult += strTemp;
                    }
                    int v1 = System.Convert.ToInt32(strResult.Substring(3,13), 2);
                    int v2 = System.Convert.ToInt32(strResult.Substring(1, 2), 2);
                    bool n = strResult.Substring(0, 1)=="1";
                    ret[i - 1] = v1 * Math.Pow(10, (n?(-1):1)* v2);
                }
                else
                {
                    ret[i - 1] = double.NaN;
                }
            }
            return ret;
        }
        public string CommandTypeStr
        {
            get
            {
                return CommandTypeString(Sensor, Command,Length);
            }
        }
        public string CommandTypeString(SensorType Sensor, CommandType p,int Lenght) {

            if (Length == 1) {
                string str = string.Format("{0}{1}",
                    "探测器" + (int)Sensor, CommandTypeSubString(p));
                return str;
            }
            else {
                string str = string.Format("{0}{1}",
                                  "探测器" + Lenght+"个", CommandTypeSubString(p) );
                return str;
            }
        }
        public static string SensorTypeString(SensorType s) {
            switch (s)
            {
                case SensorType.None:
                    return "未知";
                case SensorType.Sensor1:
                    return "探测器一";
                case SensorType.Sensor2:
                    return "探测器二";
                case SensorType.Sensor3:
                    return "探测器三";
                case SensorType.Sensor4:
                    return "探测器四";
                case SensorType.Sensor5:
                    return "探测器五";
                case SensorType.Sensor6:
                    return "探测器六";
                case SensorType.Sensor7:
                    return "探测器七";
                case SensorType.Sensor8:
                    return "探测器八";
                default:
                    break;
            }
            return "未知";
        }
        static string CommandTypeSubString(CommandType s) {
            switch (s)
            {
                case MK102Reader.CommandType.None:
                    return "未知";
                case MK102Reader.CommandType.Status:
                    return "探测器状态";
                case MK102Reader.CommandType.NongDu:
                    return "探测器数值";
                case MK102Reader.CommandType.ControllerStatus:
                    return "控制器的状态";
                case MK102Reader.CommandType.RelayStatus:
                    return "联动继电器状态";
                case MK102Reader.CommandType.RelayControl:
                    return "控制器的联动低报继电器动作";
                default:
                    break;
            }
            return "未知";
        } 
        public string Key() {
            return string.Format("{0}_{1}", Sensor, Command);
        }

        public override string ToString()
        {
            return string.Format(Key()+":"+ Response);
        }
    }

    public class SensorEntity { 
        public string Name { get { return MK102Reader.CommandAbs.SensorTypeString(Enum); } }
        public SensorType Enum { get; set; }
        public SensorStatus Status { get; set; }
        public string NongDu { get; set; }
        public DateTime DateTime { get; set; }
    }

    public enum SensorStatus
    {
        未知 = 0,
        高限报警 = 15, 低限报警 = 14, 预警 = 13, 正常 = 10, 无此节点 = 7, 通讯故障 = 1, 传感器故障 = 2,
    }

    public enum ControllerStatus
    {
        正常 = 0, 
        S1对地短路 = 16,
        总线电源短路 = 8,
        主电故障 = 4,
        备电故障 = 2,
    }

}