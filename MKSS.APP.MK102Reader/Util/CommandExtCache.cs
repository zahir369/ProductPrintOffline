using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MKSS.APP.MK102Reader
{
    public class CommandExtCache
    {
        public Dictionary<string, CommandAbs> Commands = new Dictionary<string, CommandAbs>();
        public CommandAbs Add(CommandAbs ext) {
            if (ext == null) return null;
            string key = ext.Key();
            if(!Commands.ContainsKey(key)) Commands.Add(key, ext);
            return Commands[key];
        }
        public CommandAbs Of(SensorType y, CommandType p)
        {
            string k = string.Format("{0}_{1}", y, p);
            return Commands[k];
        }
        public void Clear() {
            Commands.Clear();  
        }
        public int ReponsedCount { get { return this.Commands.Count(w => w.Value.Reponsed); } }
        public List<CommandAbs> OfList(SensorType t)
        {
            if (t == SensorType.None)
            {
                return Commands.Values.ToList();
            }
            else {
                return Commands.Values.Where(w => w.Sensor == t).ToList();
            }
        }
         
    }

}