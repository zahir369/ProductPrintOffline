using System;
using System.Collections.Generic;
using System.Linq;

namespace MKSS.APP.ConfigTool
{
    public class AddrConfigExtend
    {
        public AddrConfigExtend()
        {
            address_dic = new List<AddrValue>();
        }
        public int Min { get; set; }
        public int Max { get; set; }
        public List<AddrValue> address_dic { get; set; }

        public static bool TryAdd(AddrValue addr, List<AddrConfigExtend> results)
        {

            if (results.Count == 0)
            {
                results.Add(new AddrConfigExtend());
                return false;
            }

            AddrConfigExtend last = results.Last();
            //查询空间过大，单次查询不了，不再加入
            if (last.Max - last.Min > 50)
            {
                results.Add(new AddrConfigExtend());
                return false;
            }

            if (last.address_dic.Count == 0)
            {
                last.address_dic.Add(addr); 
                last.Min = addr.Address;
                last.Max = addr.Address;
                return true;
            }

            //超过最大间距，不能加入
            int max_interval = 16;
            if (addr.Address < last.Min && Math.Abs(addr.Address - last.Min) > max_interval)
            {
                results.Add(new AddrConfigExtend());
                return false;
            }
            if (addr.Address > last.Max && Math.Abs(addr.Address - last.Max) > max_interval)
            {
                results.Add(new AddrConfigExtend());
                return false;
            }

            last.address_dic.Add(addr); 
            if (addr.Address < last.Min) last.Min = addr.Address;
            if (addr.Address > last.Max) last.Max = addr.Address;
            return true;

        }

        public override string ToString()
        {
            return string.Format("{0}-{1}",Min,Max) ;
        }

    }

}
