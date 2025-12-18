using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


    [SugarTable("pd_sensordata")]
    public class SensorData
    {


        /// <summary>
        /// F_DataId
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public string F_DataId { get; set; }



        /// <summary>
        /// F_SensorId
        /// </summary>		
        public string F_SensorId { get; set; }



        /// <summary>
        /// F_DataValue
        /// </summary>		
        public decimal F_DataValue { get; set; }

        /// <summary>
        /// F_AddTime
        /// </summary>		
        public DateTime F_AddTime { get; set; }

        /// <summary>
        ///  空数据计算，超过 五次空数据，那么认可空数据值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public int F_DataValueEmpCount { get; set; }
        [SugarColumn(IsIgnore = true)]
        public bool F_DataValueEmp { get { return F_DataValue <= 0 || F_DataValue == 170; } }
        [SugarColumn(IsIgnore = true)]
        public bool F_DataValueEmp170 { get { return F_DataValue == 170; } }
        /// <summary>
        /// F_DataValue 校验值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public decimal F_DataValueValid { get; set; }
        /// <summary>
        ///  缓存最近几条数据
        ///  滤波算法使用
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public List<MKSS.Model.SensorData> Cache { get; set; }

        public SensorData() {
            Cache = new List<SensorData>();
        }
        public override string ToString()
        {
            return string.Format("{0}:{1},{2}", this.F_SensorId,this.F_AddTime,this.F_DataValue);
        }

    }
}