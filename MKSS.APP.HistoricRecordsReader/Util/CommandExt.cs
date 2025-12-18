namespace MKSS.APP.HistoricRecordsReader
{
    public class CommandExt {
        public const int Par1Default = 0XFF;
        public const string ResultEmpString = "/";
        public CommandType CommandType { get; set; }
        public string CommandTypeStr
        {
            get
            {
                return CommandTypeString(CommandType);
            }
        }
        public int Par1 { get; set; }
        public string Result { get; set; }
        public bool Reponsed { get { return !string.IsNullOrEmpty(Result); } }
        public static string CommandTypeString(CommandType CommandType) {
            switch (CommandType)
            {
                case CommandType.None:
                    return "未知";
                case CommandType.Warn:
                    return "报警记录";
                case CommandType.WarnRecover:
                    return "报警恢复记录";
                case CommandType.Error:
                    return "故障记录";
                case CommandType.ErrorRecover:
                    return "故障恢复记录";
                case CommandType.PowerOff:
                    return "掉电记录";
                case CommandType.PowerOffRecover:
                    return "上电记录";
                case CommandType.Overdue:
                    return "传感器失效记录";
                case CommandType.NetData:
                    return "联网数据";
                case CommandType.RecordCount:
                    return "记录总数";
                case CommandType.CurrentRecordCount:
                    return "当前记录数";
                case CommandType.DateTimeNow:
                    return "报警器当前时间";
                case CommandType.SerialNo:
                    return "读取序列号";
                case CommandType.SerialNoWrite:
                    return "写入序列号";
                default:
                    break;
            }
            return "";
        }
        public string Key() {
            return string.Format("{0}_{1}", CommandType, Par1);
        }

        public override string ToString()
        {
            return string.Format(Key()+":"+ Result);
        }
    }

}