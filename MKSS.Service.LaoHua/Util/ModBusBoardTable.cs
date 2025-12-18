using MKSS.Service.LaoHua.Util;
using System.Collections.Generic;

namespace MKSS.Service.LaoHua
{
    public class ModBusBoardTable
	{

		public CommFactory this[ModBusBoardConnection config] {
			get {
				string ip = config.IP;
				int port = config.PORT;
				if (!Table.ContainsKey(ip)) Table.Add(ip,new Dictionary<int, CommFactory>());
				if (!Table[ip].ContainsKey(port)) 
					Table[ip].Add(port, 
						new CommFactory(config, LaohuaService.READ_SPEED) );
				return Table[ip][port];
			}
		}
		 
		Dictionary<string, Dictionary<int, CommFactory>> Table { get; set; }
		public ModBusBoardTable()
		{
			Table = new Dictionary<string, Dictionary<int, CommFactory>>();
		}
	}

}
