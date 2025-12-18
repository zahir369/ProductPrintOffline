using System; 
using System.Text;
using System.Collections.Generic; 
using System.Data;
using SqlSugar;

namespace MKSS.Model
{


	[SugarTable("pd_databus")]	
	public class DataBus
	{


        /// <summary>
        /// 总线自动编号
        /// </summary>		

        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public long F_DataBusId { get; set; }  
        
        
				
		/// <summary>
		/// 数据总线用途分类(标定、检测、风机、开门、充气、搅拌、摄像头 。。。。等等）
        /// </summary>		
        
	 
        public long F_DataBusUseTypeId { get; set; }  
        
        
				
		/// <summary>
		/// 用途名称
        /// </summary>		
        
	 
        public string F_DataBusUseTypeName { get; set; }  
        
        
				
		/// <summary>
		/// 总线传输类型:Net;485
        /// </summary>		
        
	 
        public long F_DataBusTransTypeId { get; set; }  
        
        
				
		/// <summary>
		/// 传输类别名称
        /// </summary>		
        
	 
        public string F_DataBusTransTypeName { get; set; }  
        
        
				
		/// <summary>
		/// 数据通信线路名称
        /// </summary>		
        
	 
        public string F_DataBusName { get; set; }  
        
        
				
		/// <summary>
		/// 数据请求方式( 0设备被动 1设备主动) 上传数据
        /// </summary>		
        
	 
        public int F_DataAction { get; set; }  
        
        
				
		/// <summary>
		/// 摄像头登录
        /// </summary>		
        
	 
        public string F_LoginName { get; set; }  
        
        
				
		/// <summary>
		/// 摄像头密码
        /// </summary>		
        
	 
        public string F_LoginPwd { get; set; }  
        
        
				
		/// <summary>
		/// 监听的 串口号COM1,COM2,COM3
        /// </summary>		
        
	 
        public string F_ComName { get; set; }  
        
        
				
		/// <summary>
		/// 波特率
        /// </summary>		
        
	 
        public int F_ComBaudRate { get; set; }  
        
        
				
		/// <summary>
		/// 奇偶校验
        /// </summary>		
        
	 
        public int F_ComParity { get; set; }  
        
        
				
		/// <summary>
		/// 停止位
        /// </summary>		
        
	 
        public int F_ComStopBits { get; set; }  
        
        
				
		/// <summary>
		/// 数据位
        /// </summary>		
        public int F_ComDataBits { get; set; }  
        
        
				
		/// <summary>
		/// 监听的 IP地址
        /// </summary>		
        public string F_SocketIP { get; set; }  
        
        
				
		/// <summary>
		/// 监听的 端口
        /// </summary>		
        public int F_SocketPort { get; set; }  
        
        
				
		/// <summary>
		/// 类型为：TCP、UDP
        /// </summary>		
        public long F_SocketType { get; set; }  
				
		/// <summary>
		/// F_SocketTypeName
        /// </summary>		
        public string F_SocketTypeName { get; set; }  
        

        /// <summary>
        ///  使用状态, 0 未使用 , 1 检测使用中，2 标定使用中
        /// </summary>
        public int F_UseStatus { get; set; }


        /// <summary>
        ///  
        /// </summary>		
        public DateTime F_UseStartTime { get; set; }

        /// <summary>
        ///  
        /// </summary>		
        public DateTime F_UseEndTime { get; set; }


        [SugarColumn(IsIgnore = true)]
        public DataBusUseStatus EnumUseStatus { get { return (DataBusUseStatus)F_UseStatus; } set { F_UseStatus = (int)value; } }

    }

    public enum DataBusUseStatus
    {
        None = 0, ZhuiSu = 1, BiaoDing = 2
    }

}