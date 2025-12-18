using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.Service.UIBiaoDing
{


	public class X01
	{
		public const string N1 = "0123456789"; 
		public static string X0(string value, string charSet)
		{
			if (String.IsNullOrEmpty(value))
				return null;

			value = value.ToUpper();
			int radix = charSet.Length;
			int pos = radix;

			foreach (char c in value)
			{
				int i = charSet.IndexOf(c);

				if (i == -1)
					return null;

				pos += i;

				if (pos > radix)
					pos -= radix;

				pos *= 2;

				if (pos >= radix + 1)
					pos -= radix + 1;
			}

			pos = radix + 1 - pos;

			if (pos == radix)
				pos = 0;

			return value + charSet[pos];
		}
		 
	}

	 
}
