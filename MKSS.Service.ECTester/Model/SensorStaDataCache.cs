using System;
using System.Collections.Generic;

namespace MKSS.Model
{
    /// <summary>
    ///  存放 T90 等信息
    /// </summary>
    public class SensorStaDataCache : Dictionary<PosEnum, SensorStaData> {

        static SensorStaDataCache _Instance = null;
        public static SensorStaDataCache Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SensorStaDataCache();
                    _Instance.Clear();
                    foreach (var item in Enum.GetValues<PosEnum>())
                    {
                        _Instance.Add(item, new SensorStaData(item));
                    }
                }
                return _Instance;
            }
        }

        /// <summary>
        ///  开始计算
        /// </summary>
        public void CalculateStart()
        {
            foreach (var item in this.Values)
            {
                item.CalculateStart();
            } 
        }

        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        /// </summary>
        /// <param name="data"></param>
        public void CalculateAutoStepByStep(SensorGroupData data) {
            foreach (var item in this.Values)
            {
                item.CalculateAutoStepByStep(data);
            }
        }

        /// <summary>
        ///  自动计算 T90 T10 
        ///  插盘.....信号稳定....注气（10%）浓度（检测到AD增强信号，检测到AD最小值）......信号稳定...........散气（检测到AD减弱信号，检测到AD最大值）......信号稳定（检测到AD最小值）........
        /// </summary>
        /// <param name="data"></param>
        public void CalculateAuto(List<SensorGroupData> data)
        {
            foreach (var item in this.Values)
            {
                item.CalculateAuto(data);
            }
        }
    }
}