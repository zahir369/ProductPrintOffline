using System;

namespace MKSS.Service.LaoHua.Util
{
    public static class StringToByteArray
	{
		public static byte[] AddArray(string stringtem)
		{
			int len = 0;
			int i = 0;
			bool flag = stringtem.Contains(",");
			byte[] by;
			if (flag)
			{
				string[] str = new string[stringtem.Split(new char[]
				{
					','
				}).Length];
				str = stringtem.Trim().Split(new char[]
				{
					','
				});
				for (int j = 0; j < str.Length; j++)
				{
					bool flag2 = !str[j].Contains("-");
					if (flag2)
					{
						len++;
					}
					else
					{
						string[] @string = new string[str[j].Split(new char[]
						{
							'-'
						}).Length];
						@string = str[j].Split(new char[]
						{
							'-'
						});
						len = len + Convert.ToInt32(@string[1]) - Convert.ToInt32(@string[0]) + 1;
					}
				}
				by = new byte[len];
				for (int k = 0; k < str.Length; k++)
				{
					bool flag3 = !str[k].Contains("-");
					if (flag3)
					{
						by[i] = Convert.ToByte(str[k]);
						i++;
					}
					else
					{
						for (byte w = Convert.ToByte(str[k].Split(new char[]
						{
							'-'
						})[0]); w <= Convert.ToByte(str[k].Split(new char[]
						{
							'-'
						})[1]); w += 1)
						{
							by[i] = w;
							i++;
						}
					}
				}
			}
			else
			{
				bool flag4 = stringtem.Contains("-");
				if (flag4)
				{
					string[] str = new string[stringtem.Trim().Split(new char[]
					{
						'-'
					}).Length];
					str = stringtem.Trim().Split(new char[]
					{
						'-'
					});
					by = new byte[(int)(Convert.ToByte(str[1]) - Convert.ToByte(str[0]) + 1)];
					for (int w2 = (int)Convert.ToByte(str[0]); w2 <= (int)Convert.ToByte(str[1]); w2++)
					{
						by[i] = (byte)w2;
						i++;
					}
				}
				else
				{
					by = new byte[]
					{
						Convert.ToByte(stringtem)
					};
				}
			}
			Array.Sort<byte>(by);
			return by;
		}
	}
}
