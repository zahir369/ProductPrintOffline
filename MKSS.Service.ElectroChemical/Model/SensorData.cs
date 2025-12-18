//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Data;
//using SqlSugar;

//namespace MKSS.Model
//{


//    [SugarTable("pd_sensordata")]
//    public class SensorData1
//    {


//        /// <summary>
//        /// F_DataId
//        /// </summary>		

//        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
//        public string F_DataId { get; set; }



//        /// <summary>
//        /// F_SensorId
//        /// </summary>		
//        public string F_SensorId { get; set; }



//        /// <summary>
//        /// F_DataValue
//        /// </summary>		
//        public double F_DataValue { get; set; }

//        /// <summary>
//        /// F_AddTime
//        /// </summary>		
//        public double F_AddTime { get; set; }

//        [SugarColumn(IsIgnore = true)]
//        public int F_DataValueEmpCount { get; set; }
//        /// <summary>
//        ///  空数据计算，超过 五次空数据，那么认可空数据值
//        /// </summary>
//        [SugarColumn(IsIgnore = true)]
//        public Sensor Sensor { get; set; }

//        [SugarColumn(IsIgnore = true)]
//        public bool F_DataValueEmp { get { return F_DataValue <= 0 || F_DataValue == 170; } }

//        public override string ToString()
//        {
//            return string.Format("{0}:{1},{2}", this.F_SensorId,this.F_AddTime,this.F_DataValue);
//        }

//    }
//}