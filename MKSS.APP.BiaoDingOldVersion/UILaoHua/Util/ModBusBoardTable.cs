using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System.Collections.Generic;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    public class ModBusBoardTable
	{

		public ModBusBoard this[int boardcase_no, int tunnel_no] {
			get {
				if (!Table.ContainsKey(boardcase_no)) Table.Add(boardcase_no,new Dictionary<int, ModBusBoard>());
				if (!Table[boardcase_no].ContainsKey(tunnel_no)) 
					Table[boardcase_no].Add(tunnel_no, 
						new ModBusBoard(boardcase_no, tunnel_no, UILaoHuaGuanChaModel.READ_SPEED) );
				return Table[boardcase_no][tunnel_no];
			}
		}
		 
		Dictionary<int, Dictionary<int, ModBusBoard>> Table { get; set; }
		public ModBusBoardTable()
		{
			Table = new Dictionary<int, Dictionary<int, ModBusBoard>>();
		}
	}

}
