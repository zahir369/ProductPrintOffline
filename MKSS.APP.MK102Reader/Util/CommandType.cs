namespace MKSS.APP.MK102Reader
{
    public enum CommandType
    {
        /// <summary>
        ///  未知
        /// </summary>
        None = -1,
        /// <summary>
        /// 探测器状态
        /// </summary>
        Status = 3,
        /// <summary>
        /// 探测器数值
        /// </summary>
        NongDu = 4,
        /// <summary>
        /// 控制器的状态
        /// </summary>
        ControllerStatus = 2,
        /// <summary>
        /// 联动继电器状态（01位是低报，02位是高报）
        /// </summary>
        RelayStatus = 10,
        /// <summary>
        /// 控制器的联动低报继电器动作
        /// </summary>
        RelayControl = 50,
    }
}