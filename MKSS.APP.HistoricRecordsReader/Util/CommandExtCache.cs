using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MKSS.APP.HistoricRecordsReader
{
    public class CommandExtCache
    {
        public Dictionary<string, CommandExt> Commands = new Dictionary<string, CommandExt>();
        public CommandExt Add(CommandExt ext) {
            if (ext == null) return null;
            string key = ext.Key();
            if(!Commands.ContainsKey(key)) Commands.Add(key, ext);
            return Commands[key];
        }
        public CommandExt Of(CommandType y,int p)
        {
            string k = string.Format("{0}_{1}", y, p);
            return Commands[k];
        }
        public void Clear() {
            Commands.Clear();  
        }
        public int ReponsedCount { get { return this.Commands.Count(w => w.Value.Reponsed); } }
        public List<CommandExt> OfList(CommandType t)
        {
            if (t == CommandType.None)
            {
                return Commands.Values.Where(w=>
                w.CommandType== CommandType.PowerOff || 
                w.CommandType == CommandType.PowerOffRecover || 
                w.CommandType == CommandType.Warn || 
                w.CommandType == CommandType.WarnRecover ||
                w.CommandType == CommandType.Error ||
                w.CommandType == CommandType.ErrorRecover ||
                w.CommandType == CommandType.Overdue).ToList();
            }
            else {
                return Commands.Values.Where(w => w.CommandType == t).ToList();
            }
        }

        /// <summary>
        ///  优先处理
        /// </summary>
        /// <param name="t"></param>
        public void FirstDealWith(CommandType t)
        {
            lock (Commands) {
                List<CommandExt> a1 = Commands.Values.Where(w => w.CommandType == t).ToList();
                List<CommandExt> a2 = Commands.Values.Where(w => w.CommandType != t).ToList();
                Clear();
                foreach (var item in a1)
                {
                    Add(item);
                }
                foreach (var item in a2)
                {
                    Add(item);
                }
            }
            
        }
    }

}