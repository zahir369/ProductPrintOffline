using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.WinNBOnlinePrinterTest
{

    /// <summary>
    ///  状态，0 仅添加, 1 已在WING平台登记 ， -1 交付后从WING平台注销，2 有数据但没有F_DeviceId或F_SerialNO信息，3 产品上线成功
    /// </summary>
    public enum DeviceStatus
    {
        None = 0,
        Register = 1,
        RegisterFaild = -11,
        UnRegister = -1,
        UnRegisterFaild = -10,
        OnlineNoData = 2,
        OnlineSucess = 3,
        OnlineSucessTestAlarm = 4,
        OnlineSucessAlarm = 5,
        OnlineSucessDeviceError = 6,
        RegisterSoldDevice = 99,
        RegisterSoldDeviceError = 991
    }


    /// <summary>
    ///  发货状态，0 未打印，1 已打印 ，2 已装箱，3 已发货
    /// </summary>
    public enum DeliverStatus
    {
        /// <summary>
        /// 未打印
        /// </summary>
        NoPrint = 0,
        /// <summary>
        ///  已打印
        /// </summary>
        Printed = 1,
        /// <summary>
        ///  已装箱
        /// </summary>
        Packaged = 2,
        /// <summary>
        ///  已发货
        /// </summary>
        Deliverd = 3
    }

}
