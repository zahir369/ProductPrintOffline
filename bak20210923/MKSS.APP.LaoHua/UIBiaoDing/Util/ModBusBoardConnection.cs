using MKSS.Service.LaoHua.Util;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;

namespace MKSS.APP.UIBiaoDing.Util
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

		public List<byte> Address {
			get {
				List<byte> rt = new List<byte>();
				if (BoardList != null) {
                    foreach (var _Board in BoardList.Keys.ToList())
                    {

						List<byte> addr = _Board.AddrList;
						List<long> databus = _Board.DataBusList;
						if (addr.Count != databus.Count)
						{
							ULogger.Error(string.Format("地址，链路不匹配：{0},{1}", addr.Count, databus.Count));
							continue;
						}

                        
                        for (int i = 0; i < databus.Count; i++)
                        {
							if (databus[i] == this.DataBus.F_DataBusId) {
								//一个板子有多个地址，只添加，这个数据总线 ip 的板子地址  
								rt.Add(addr[i]);
							}
                        }
						
					}
				}
				return rt;
			}
		}
		 
		public Dictionary<Board,object> BoardList { get; internal set; }
		public DataBus DataBus { get; internal set; }
		public DateTime Start { get; internal set; }
        public string IP { get { return DataBus.F_SocketIP; } }
		public int PORT { get { return DataBus.F_SocketPort; } }
		public ModBusBoard ModBusBoard { get; private set; }
		 
		public bool IsOpen { get { return ModBusBoard != null && ModBusBoard.IsOpen; } }
		public event PropertyChangedEventHandler PropertyChanged;

		public ModBusBoardConnection(DataBus _DataBus) {
			this.DataBus = _DataBus;
			this.BoardList = new Dictionary<Board, object>();
			this.ModBusBoard = new ModBusBoard(this, UISheBeiBiaoDingViewModel.READ_SPEED);
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.IsOpen);
		}

	}

}
