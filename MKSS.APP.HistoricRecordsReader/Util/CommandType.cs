namespace MKSS.APP.HistoricRecordsReader
{
    public enum CommandType
    {
        /// <summary>
        /// 报警记录
        /// </summary>
        None = -1,
        /// <summary>
        /// 报警记录
        /// </summary>
        Warn = 1,
        /// <summary>
        /// 报警恢复记录
        /// </summary>
        WarnRecover = 2,
        /// <summary>
        /// 故障记录
        /// </summary>
        Error = 3,
        /// <summary>
        /// 故障恢复记录
        /// </summary>
        ErrorRecover = 4,
        /// <summary>
        /// 掉电记录
        /// </summary>
        PowerOff = 5,
        /// <summary>
        /// 上电记录
        /// </summary>
        PowerOffRecover = 6,
        /// <summary>
        /// 传感器失效记录
        /// </summary>
        Overdue = 7,
        /// <summary>
        /// 联网数据
        /// </summary>
        NetData = 8,
        /// <summary>
        /// 记录总数
        /// </summary>
        RecordCount = 9,
        /// <summary>
        /// 当前记录数
        /// </summary>
        CurrentRecordCount = 10,
        /// <summary>
        /// 报警器当前时间
        /// </summary>
        DateTimeNow = 11,
        /// <summary>
        /// 报警器序列号
        /// </summary>
        SerialNo = 12,
        /// <summary>
        /// 报警器序列号
        /// </summary>
        SerialNoWrite = 13,
        /// <summary>
        /// 0x89-读取零点AD、标定点AD、标定点浓度点命令
        /// </summary>
        ZeroSpanNongDu = 16, 

    }

}