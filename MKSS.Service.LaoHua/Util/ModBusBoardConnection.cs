using MKSS.Service.LaoHua.Util;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;

namespace MKSS.Service.LaoHua
{

	[LogTagClass(Title = "老化柜连接")]
	public class ModBusBoardConnection : ULogDataFilter
	{

		/// <summary>
		///  主类别
		/// </summary>
		public virtual string Main { get { return "老化柜连接"; } }
		/// <summary>
		///  分类名称
		/// </summary>
		public virtual string DataTitle { get { return DataBus == null ? "默认" : DataBus.F_DataBusId + ""; } }
		/// <summary>
		///  用户对象
		/// </summary>
		public virtual object Tag { get { return this.DataBus; } set { } }


		public List<TaskInfo> TaskList { get; private set; }
		public void SetAddr(List<TaskInfo> tasks)
		{
			TaskList = tasks;
		}

		public List<TaskAddress> Address {
			get {
				List<TaskAddress> rt = new List<TaskAddress>();
				if (TaskList != null)
				{
					foreach (TaskInfo task in TaskList)
					{
						var _Board = task.Board;
						List<byte> addr = _Board.AddrList;
						List<long> databus = _Board.DataBusList;
						if (addr.Count != databus.Count)
						{
							ULogger.Error(string.Format("地址，链路不匹配：{0},{1}", addr.Count, databus.Count));
							continue;
						}
						for (int i = 0; i < databus.Count; i++)
						{
							if (databus[i] == this.DataBus.F_DataBusId)
							{
								//一个板子有多个地址，只添加，这个数据总线 ip 的板子地址  
								rt.Add(new TaskAddress() { Address = addr[i], TaskInfo = task });  
							}
						}
					}
				}
				return rt;
			}
		}


		Dictionary<string, DateTime> ZBoardListSM_DataEventSaveDbLast = new Dictionary<string, DateTime>();
		public void SetSM_DataEventSaveDbLast(Board b, byte addr, DateTime lasr) {
			if (!ZBoardListSM_DataEventSaveDbLast.ContainsKey(b.F_BoardId + "#" + addr)) ZBoardListSM_DataEventSaveDbLast.Add(b.F_BoardId + "#" + addr, lasr);
			else ZBoardListSM_DataEventSaveDbLast[b.F_BoardId + "#" + addr] = lasr;
		}

		public DateTime GetSM_DataEventSaveDbLast(Board b,byte addr) {
			if (!ZBoardListSM_DataEventSaveDbLast.ContainsKey(b.F_BoardId+"#"+ addr)) return DateTime.MinValue;
			else return ZBoardListSM_DataEventSaveDbLast[b.F_BoardId + "#" + addr];
		}

		public DataBus DataBus { get; internal set; }
		public DateTime Start { get; internal set; }
        public string IP { get { return DataBus.F_SocketIP; } }
		public int PORT { get { return DataBus.F_SocketPort; } }
		public CommFactory ModBusBoard { get; private set; }
		 
		public bool IsOpen { get { return ModBusBoard != null && ModBusBoard.IsOpen; } }
		public event PropertyChangedEventHandler PropertyChanged;

		public ModBusBoardConnection(DataBus _DataBus) {
			this.DataBus = _DataBus; 
			this.ModBusBoard = new CommFactory(this, LaohuaService.READ_SPEED);
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.IsOpen);
		}

	}

}
