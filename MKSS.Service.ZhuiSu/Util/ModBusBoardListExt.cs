using System.Collections.Generic;
using MKSS.Model;
using MKSS.Services;
using System.Linq;
using System.ComponentModel;

namespace DeviceDataMonitorWPF.UILaoHua
{

    public class ModBusBoardListExt : INotifyPropertyChanged
	{

		Dictionary<Board, BoardCase> RelCase { get; set; }
		Dictionary<Board, List<DataBus>> RelBus { get; set; }
		Dictionary<Board, List<ModBusBoardConfig>> RelModBusBoard { get; set; }
		Dictionary<string, List<ModBusBoardConfig>> Cache = new Dictionary<string, List<ModBusBoardConfig>>(); 


		public event PropertyChangedEventHandler PropertyChanged;
		public List<ModBusBoardConfig> this[Board board] {
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
					List<ModBusBoardConfig> cxx = RelModBusBoard.ContainsKey(board) ? RelModBusBoard[board] : null;
                    foreach (ModBusBoardConfig c in cxx) if (c != null && !ret.Contains(c)) ret.Add(c);
				}
				return ret;
			}
		}

		public ModBusBoardListExt() {
			RelCase = new Dictionary<Board, BoardCase>();
			RelBus = new Dictionary<Board, List<DataBus>>();
			RelModBusBoard = new Dictionary<Board, List<ModBusBoardConfig>>(); 
		}
		public void Clear(){
			RelCase = new Dictionary<Board, BoardCase>();
			RelBus = new Dictionary<Board, List<DataBus>>();
			RelModBusBoard = new Dictionary<Board, List<ModBusBoardConfig>>(); 
		}
		public void Push(Board _Board, BoardCase _BoardCase, List<DataBus> buss, ModBusBoardTable SerialModBusPool) {

			int case_no = _BoardCase.F_BoardCaseAddress;
			if (!ModBusBoards.ContainsKey(case_no)) 
				ModBusBoards.Add(case_no,new Dictionary<int, ModBusBoardConfig>());
			if (!RelBus.ContainsKey(_Board)) {
				DataBusServices DataBusServices = new DataBusServices();
				DataBus _DataBus2 = DataBusServices.Query(w => w.F_DataBusId == _BoardCase.F_DataBusId).Result.FirstOrDefault(); ;
				if (_DataBus2 == null) throw new System.Exception("缺少数据总线");
				RelBus.Add(_BoardCase, _DataBus2);
			}

			DataBus _DataBus = RelBus[_BoardCase];
			string ip = _DataBus.F_SocketIP;
			int port = _DataBus.F_SocketPort;
			if (!ModBusBoards[case_no].ContainsKey(tunnel_no_addr))
			{
				ModBusBoards[case_no].Add(tunnel_no_addr, ModBusBoardConfig.Instance(Cache,ip, port,case_no, tunnel_no_addr, SerialModBusPool));
			}
			 
			if (!RelModBusBoard.ContainsKey(_Board)) {
				RelModBusBoard.Add(_Board, ModBusBoards[case_no][tunnel_no_addr]);
			} else {
				RelModBusBoard[_Board] = ModBusBoards[case_no][tunnel_no_addr];
			} 
		}
		 
	}

}
