using MKSS.Model;
using System.Linq;
using System.Collections.Generic;
using MKSS.APP.LaoHuaService.MQTT;

namespace MKSS.APP.LaoHuaService.MQTT
{

    /// <summary>
    ///  老化数据出入口
    /// </summary>
    public interface LaoHuaDataProvider
    {
        static LaoHuaDataProvider _Instance = null;
        public static LaoHuaDataProviderUIMode UIMode { get; set; } = LaoHuaDataProviderUIMode.SingleBoard;
        public static LaoHuaDataProvider Instance {
            get {
                if (_Instance == null) {
                    switch (UIMode)
                    {
                        case LaoHuaDataProviderUIMode.SingleBoard:
                            _Instance = LaoHuaDataProviderSerialPort.Instance;
                            break;
                        case LaoHuaDataProviderUIMode.SerialPort:
                            _Instance = LaoHuaDataProviderSerialPort.Instance;
                            
                            break;
                        case LaoHuaDataProviderUIMode.NetWork:
                            _Instance = LaoHuaDataProviderNetwork.Instance;
                            break;
                        default:
                            break;
                    }
                }
                return _Instance;
            }
        } 
        bool Inited { get;  }
        LaHuaTaskStatus TaskStatus { get { return Status.TaskStatus; } }
        LaoHuaTaskOperate TaskOperate { get { return Status.TaskOperate; } } 
        StatusEntity Status { get; }
        void InitConnection();
        void Query(Batch _Batch, MqttTaskCommand comm = MqttTaskCommand.QueryRealtimeData, string param = null); 
    }

    /// <summary>
    ///  界面样式
    /// </summary>
    public enum LaoHuaDataProviderUIMode
    {
        /// <summary>
        ///  单托盘版
        /// </summary>
        SingleBoard = 0,
        /// <summary>
        /// 串口版
        /// </summary>
        SerialPort = 1,
        /// <summary>
        /// 网络版
        /// </summary>
        NetWork = 2
    }

}
