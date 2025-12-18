using MKSS.Service.ZhuiSu.Util;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;

namespace MKSS.Service.ZhuiSu
{

	[LogTagClass(Title = "实验柜连接")]
	public class ModBusBoardConnection : ULogDataFilter
	{

		/// <summary>
		///  主类别
		/// </summary>
		public virtual string Main { get { return "实验柜连接"; } }
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
                        {
							if (_Board.DataBusInt == this.DataBus.F_DataBusId) {
								//一个板子有多个地址，只添加，这个数据总线 ip 的板子地址  
								rt.Add(_Board.AddrByte);
							}
                        }
						
					}
				}
				return rt;
			}
		}
		 
		public Dictionary<Board,Batch> BoardList { get; internal set; }
		public DataBus DataBus { get; internal set; }
		public DateTime Start { get; internal set; }
        public string IP { get { return DataBus.F_SocketIP; } }
		public int PORT { get { return DataBus.F_SocketPort; } }
		public ModBusBoard ModBusBoard { get; private set; }
		 
		/// <summary>
		/// 上次网络测试失败时间
		/// </summary>
		public DateTime LastPingFailer { get; set; }
		public bool IsOpen { get { return ModBusBoard != null && ModBusBoard.IsOpen; } }
		public event PropertyChangedEventHandler PropertyChanged;

		public ModBusBoardConnection(DataBus _DataBus) {
			this.DataBus = _DataBus;
			this.BoardList = new Dictionary<Board, Batch>();
			this.ModBusBoard = new ModBusBoard(this, ZhuiSuService.READ_SPEED);
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.IsOpen);
		}

	}

}
