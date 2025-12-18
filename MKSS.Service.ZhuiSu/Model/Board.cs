using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using SqlSugar;

namespace MKSS.Model
{

    [SugarTable("pd_board")]	
	public class Board
	{


        /// <summary>
        /// 板子编号
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_BoardId { get; set; }


        /// <summary>
        /// 板子物理地址码，多个地址逗号隔开
        /// </summary>		
        public string F_BoardAddress { get; set; }

        /// <summary>
        /// 板子名称
        /// </summary>		
        public string F_BoardName { get; set; }


        /// <summary>
        /// 所在层号
        /// </summary>		
        public int F_FloorNO { get; set; }



        /// <summary>
        /// 板子传感器容量，固定值15
        /// </summary>		
        public int F_SensorCount { get; set; }



        /// <summary>
        /// 所属数据总线编号，多个地址逗号隔开
        /// </summary>		
        public string F_DataBusId { get; set; }



        /// <summary>
        /// 所在架、柜编号
        /// </summary>		
        public long F_BoarCaseId { get; set; }

        /// <summary>
        /// 最后使用开始时间
        /// </summary>		
        public DateTime F_LastUseStart { get; set; }

        /// <summary>
        /// 最后使用时间
        /// </summary>		
        public DateTime F_LastUseUpdate { get; set; }

        /// <summary>
        /// 最后使用板子插入传感器数量
        /// </summary>		
        public int F_LastUseSensorCount { get; set; }


        /// <summary>
        ///  最后使用类别，老化、标定，1 老化，2标定
        /// </summary>		
        public int F_LastUseType { get; set; }

        /// <summary>
        ///  使用状态，1 使用中，2 空闲
        /// </summary>		
        public int F_UseStatus { get; set; }

        [SugarColumn(IsIgnore = true)]
        public EnumUseType EnumUseType { get { return (EnumUseType)F_LastUseType; } set { F_LastUseType = (int)value; } }
        [SugarColumn(IsIgnore = true)]
        public EnumUseInFree EnumUseInFree { get { return (EnumUseInFree)F_UseStatus; } set { F_UseStatus = (int)value; } }
        [SugarColumn(IsIgnore = true)]
        public int TunnelNO { get { return (this.F_FloorNO+1)/2; } }
        [SugarColumn(IsIgnore = true)]
        public byte AddrByte { 
            get {
                if (string.IsNullOrEmpty(F_BoardAddress)) return 0;
                int i = 0;
                if (int.TryParse(F_BoardAddress, out i))
                {
                    if (i <= 255 && i >= 0) return ((byte)i);
                }
                return 0; 
            } 
        }
        [SugarColumn(IsIgnore = true)]
        public int DataBusInt
        {
            get
            {
                if (string.IsNullOrEmpty(F_DataBusId)) return 0;
                int i = 0;
                if (int.TryParse(F_DataBusId, out i))
                {
                     return i;
                }
                return 0;
            }
        }



        public override string ToString()
        {
            return string.Format("{0}",this.F_FloorNO);
        }


    }
}