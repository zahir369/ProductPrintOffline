using MKSS.Service.ZhuiSu.Util;
using System.Collections.Generic;

namespace MKSS.Service.ZhuiSu
{
    public class ModBusBoardTable
	{

		public ModBusBoard this[ModBusBoardConnection config] {
			get {
				string ip = config.IP;
				int port = config.PORT;
				if (!Table.ContainsKey(ip)) Table.Add(ip,new Dictionary<int, ModBusBoard>());
				if (!Table[ip].ContainsKey(port)) 
					Table[ip].Add(port, 
						new ModBusBoard(config, ZhuiSuService.READ_SPEED) );
				return Table[ip][port];
			}
		}
		 
		Dictionary<string, Dictionary<int, ModBusBoard>> Table { get; set; }
		public ModBusBoardTable()
		{
			Table = new Dictionary<string, Dictionary<int, ModBusBoard>>();
		}
	}

}
