using System.Collections.Generic;
using MKSS.Model;
using MKSS.Services;
using System.Linq;
using System.ComponentModel;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{

    public class ModBusBoardListExt : INotifyPropertyChanged
	{
		Dictionary<Board, BoardCase> RelCase { get; set; }
		Dictionary<BoardCase, DataBus> RelBus { get; set; }
		Dictionary<Board, ModBusBoardConfig> RelModBusBoard { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		public ModBusBoardConfig this[Board board] {
			get {
				return RelModBusBoard.ContainsKey(board) ? RelModBusBoard[board] : null;
			}
		}

		public List<ModBusBoardConfig> this[List<Board> boards]
		{
			get
			{
				List<ModBusBoardConfig> ret = new List<ModBusBoardConfig>();
                foreach (var board in boards)
                {
					ModBusBoardConfig c = RelModBusBoard.ContainsKey(board) ? RelModBusBoard[board] : null;
					if (c != null && !ret.Contains(c)) ret.Add(c);
				}
				return ret;
			}
		}

		public Dictionary<int, Dictionary<int, ModBusBoardConfig>> ModBusBoards { get; set; }
		public ModBusBoardListExt() {
			RelCase = new Dictionary<Board, BoardCase>();
			RelBus = new Dictionary<BoardCase, DataBus>();
			RelModBusBoard = new Dictionary<Board, ModBusBoardConfig>();
			ModBusBoards = new Dictionary<int, Dictionary<int, ModBusBoardConfig>>();
		}

		public void Clear(){
			RelCase = new Dictionary<Board, BoardCase>();
			RelBus = new Dictionary<BoardCase, DataBus>();
			RelModBusBoard = new Dictionary<Board, ModBusBoardConfig>();
			ModBusBoards = new Dictionary<int, Dictionary<int, ModBusBoardConfig>>();
		}

		public void Push(Board _Board, BoardCase _BoardCase, ModBusBoardTable SerialModBusPool) {

			int case_no = _BoardCase.F_BoardCaseAddress;
			int tunnel_no_addr = _Board.TunnelNO;//每两层一个通道
			if (!ModBusBoards.ContainsKey(case_no)) 
				ModBusBoards.Add(case_no,new Dictionary<int, ModBusBoardConfig>());
			if (!RelBus.ContainsKey(_BoardCase)) {
				DataBusServices DataBusServices = new DataBusServices();
				DataBus _DataBus2 = DataBusServices.Query(w => w.F_DataBusId == _BoardCase.F_DataBusId).Result.FirstOrDefault(); ;
				if (_DataBus2 == null) throw new System.Exception("缺少数据总线");
				RelBus.Add(_BoardCase, _DataBus2);
			}

			DataBus _DataBus = RelBus[_BoardCase];
			string ip = _DataBus.F_SocketIP;
			int portadd = (tunnel_no_addr - 1) * 100;
			int port = _DataBus.F_SocketPort + portadd;
			if (!ModBusBoards[case_no].ContainsKey(tunnel_no_addr))
			{
				ModBusBoards[case_no].Add(tunnel_no_addr, ModBusBoardConfig.Instance(ip, port,case_no, tunnel_no_addr, SerialModBusPool));
			}
			 
			if (!RelModBusBoard.ContainsKey(_Board)) {
				RelModBusBoard.Add(_Board, ModBusBoards[case_no][tunnel_no_addr]);
			} else {
				RelModBusBoard[_Board] = ModBusBoards[case_no][tunnel_no_addr];
			} 
		}
		 
	}

}
