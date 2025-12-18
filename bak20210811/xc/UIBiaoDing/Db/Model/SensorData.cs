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

        [SugarColumn(IsNullable = false, IsPrimaryKey = true )]
        public string F_DataId { get; set; }


        /// <summary>
        /// 所在数据板编号 
        /// </summary>		
        public int F_BoardId { get; set; }
        /// <summary>
        /// F_SensorId
        /// </summary>		
        public string F_SensorId { get; set; }

         
        public int V1 { get; set; }
        public int V2 { get; set; }
        public int V3 { get; set; }
        public int V4 { get; set; }
        public int V5 { get; set; }
        public int V6 { get; set; }
        public int V7 { get; set; }
        public int V8 { get; set; }
        public int V9 { get; set; }
        public int V10 { get; set; } 

        /// <summary>
        /// F_AddTime
        /// </summary>		
        public DateTime F_AddTime { get; set; }

        [SugarColumn(IsIgnore = true)]
        public int F_DataValueEmpCount { get; set; }
        /// <summary>
        ///  空数据计算，超过 五次空数据，那么认可空数据值
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public Sensor Sensor { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool F_DataValueEmp { get { return V1 <= 0 || V1 == 170; } }

        public override string ToString()
        {
            return string.Format("{0}:{1},{2}", this.F_SensorId,this.F_AddTime,this.V1);
        }

    }
}