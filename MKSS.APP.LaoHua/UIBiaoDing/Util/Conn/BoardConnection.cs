using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using MKSS.APP.UIBiaoDing.Util;
using NModbus;

namespace MKSS.Service.LaoHua
{


    [LogTagClass(Title = "硬件连接")]
	public class BoardConnection : ULogDataFilter
	{

		/// <summary>
		///  唯一标记
		/// </summary>
		public virtual string ID { get { return "未定义"; } }
		/// <summary>
		///  主类别
		/// </summary>
		public virtual string Main { get { return "硬件连接"; } }
		/// <summary>
		///  分类名称
		/// </summary>
		public virtual string DataTitle { get { return "某地址"; } }
		/// <summary>
		///  用户对象
		/// </summary>
		public virtual object Tag { get { return null; } set { } }
		/// <summary>
		///  是否已打开连接
		/// </summary>
		public virtual bool IsOpen { get { return false; } }
		/// <summary>
		///  检查连接 
		/// </summary>
		/// <returns></returns>
		public virtual bool CheckConnection()
		{
			return false;
		}
		public virtual void Open() { }
		public virtual void Close() { }
		public AllConnection Pool { get;   set; }
		public DateTime Start { get; internal set; }
		public CommFactory CommFactory { get; private set; }
		public List<Address> Address { get; set; }
		public ModbusFactory modbusFactory;
		public event PropertyChangedEventHandler PropertyChanged;

		public BoardConnection( int read_speed) {
			this.CommFactory = new CommFactory(this,   read_speed);
			Address = new List<Address>();
			modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace,this));
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.DataTitle,  this.IsOpen?"T":"F");
		}

	}

}
