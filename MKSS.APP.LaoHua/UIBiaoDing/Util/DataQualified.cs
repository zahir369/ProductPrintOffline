using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MKSS.APP.UIBiaoDing.Util
{
	public class DataQualified
	{

		public DictionaryExt<Address, SensorGroupData> CurrentProductData
		{
			get;
			set;
		}

		public string KeyOf(int append, int boardNo, int lineIndex) {
			return string.Format("{0}_{1}_{2}", append, boardNo, lineIndex);
		}
		Dictionary<string, bool?> IsQualifiedCache = new Dictionary<string, bool?>();
		public bool? this[int append,int boardNo, int lineIndex]
		{
			get{
				string key = KeyOf(append, boardNo, lineIndex);
				if (!IsQualifiedCache.ContainsKey(key)) return null;
				return IsQualifiedCache[key];
			}
			set
			{
				string key = KeyOf(append, boardNo, lineIndex);
				if (!IsQualifiedCache.ContainsKey(key)) IsQualifiedCache.Add(key, value);
				else IsQualifiedCache[key] = value;
			}
		}

		/// <summary>
		///  不合格品
		/// </summary>
		/// <param name="boardNo"></param>
		/// <param name="lineIndex"></param>
		/// <returns></returns>
		public bool NotQualified(int append, int boardNo, int lineIndex)
		{
			string key = KeyOf(append, boardNo, lineIndex);
			if (!IsQualifiedCache.ContainsKey(key)) return false;
			return IsQualifiedCache[key] !=null && !IsQualifiedCache[key].Value;
		}

		public int StaQuallified
		{
			get { 
				if (CurrentProductData == null) return 0;
				return StaTotal - StaUnqualified;
			}
		}

		public int StaUnqualified
		{
			get
			{
				if (CurrentProductData == null) return 0;
				int ret = 0;
				///StringBuilder ss = new StringBuilder();
                foreach (SensorGroupData item in CurrentProductData.Values.ToArray())
                {
					if (item == null) continue;
                    foreach (var s in item.SingleAddressData.ToArray())
					{
						if (s.IsEmpData) continue;
						if (s.IsEmpSensor) continue;
						if (s.IsEmp170) continue;
						bool? HEGE = this[item.Address.APP, item.Address.V, s.Position];
						if(HEGE!=null && !HEGE.Value) ret++;
						//if (s.IsQualifiedV7 != null && !s.IsQualifiedV7.Value) {
						//	ret++;
						//	///ss.Append(string.Format("{0}_{1};"+System.Environment.NewLine, item.Address, s.Position));
						//}
					}
                }
				return ret;
			}
		}

		public double StaRate
		{
			get {
				double _HeGeRatio = (double)this.StaQuallified / (double)StaTotal;
				return _HeGeRatio;
			}
		}


		public int StaTotal
		{
			get
			{
				if (CurrentProductData == null) return 0;
				int t = 0;
                foreach (var c in CurrentProductData.Values.ToArray())
                {
					if (c == null) continue;
                    foreach (SensorDataX d in c.SingleAddressData.ToArray())
                    {
						int? v = d.V01;
						if (v != null && v.Value > 0 && v.Value != 170) {
							t++;
						}
                    }
                }
				return t;
			}
		}

		public void Clear()
		{
			IsQualifiedCache.Clear();
			this.CurrentProductData.Clear(); 
		}

		public void CalcAvg(out int v1Avg, out int v7Avg)
		{ 
			List<int> V7_LIST = new List<int>();
			List<int> V1_LIST = new List<int>();
 
			foreach (var groupdata in this.CurrentProductData.Values.ToArray())
			{
				if (groupdata == null) continue;
				for (int j = 0; j < 15; j++)
				{
					int? numV1 = groupdata.SingleAddressData[j].V01;
					if (numV1.HasValue && numV1.GetValueOrDefault() != 170 )
					{
						V1_LIST.Add(groupdata.SingleAddressData[j].V01.Value);
						int? numV7 = groupdata.SingleAddressData[j].V07;
						if (numV7.HasValue && numV7.GetValueOrDefault() != 0)
						{
							V7_LIST.Add(groupdata.SingleAddressData[j].V07.Value);
						}
					}
				}
			}
			V1_LIST.Sort();
			V7_LIST.Sort();


			int PaiChuV7 = V7_LIST.Count / 10;
			if (V7_LIST.Count <= 2)
			{
				PaiChuV7 = 0;
			}
			else
			{
				if (PaiChuV7 == 0)
				{
					PaiChuV7 = 1;
				}
			}

			if (V7_LIST.Count > 2)
			{
				V7_LIST.RemoveRange(V7_LIST.Count - PaiChuV7, PaiChuV7);
				V7_LIST.RemoveRange(0, PaiChuV7);
			}

			if (V7_LIST.Count > 0)
			{
				v7Avg = V7_LIST.Sum() / V7_LIST.Count;
			}
			else {
				v7Avg = 0;
			}

			int PaiChuV1 = V1_LIST.Count / 10;
			if (V1_LIST.Count <= 2)
			{
				PaiChuV1 = 0;
			}
			else
			{
				if (PaiChuV1 == 0)
				{
					PaiChuV1 = 1;
				}
			}

			if (V1_LIST.Count > 2)
			{
				V1_LIST.RemoveRange(V1_LIST.Count - PaiChuV1, PaiChuV1);
				V1_LIST.RemoveRange(0, PaiChuV1);
			}

			if (V1_LIST.Count > 0)
			{
				v1Avg = V1_LIST.Sum() / V1_LIST.Count;
			}
			else {
				v1Avg = 0;
			}

		}

		public void SetQualifiedV1V7(
			int baseValue07,int OffSideDown07, int OffSideUp07,
			int baseValue01, int OffSideDown01, int OffSideUp01)
		{


            try
            {
				foreach (var groupdata in this.CurrentProductData.Values.ToArray())
				{
					if (groupdata == null) continue;
					for (int j = 0; j < 15; j++)
					{
						if (groupdata == null) continue;
						SensorDataX s = groupdata.SingleAddressData[j];
						if (s.IsEmpData || s.IsEmpSensor)
						{
							s.IsQualifiedV1 = null;
							s.IsQualifiedV7 = null;
							continue;
						}

						if (baseValue01 == 0)
						{
							s.IsQualifiedV1 = null;
						}
						else
						{
							if (s.V01.Value >= baseValue01 - OffSideDown01 && s.V01.Value <= baseValue01 + OffSideUp01)
							{
								s.IsQualifiedV1 = true;
							}
							else
							{
								if (s.V01.Value == 0 || s.V01.Value == 170)
								{
									s.IsQualifiedV1 = true;
								}
								else
								{
									s.IsQualifiedV1 = false;
								}

							}
						}

						if (baseValue07 == 0)
						{
							s.IsQualifiedV7 = null;
						}
						else
						{
							if (s.V07.Value >= baseValue07 - OffSideDown07 && s.V07.Value <= baseValue07 + OffSideUp07)
							{
								s.IsQualifiedV7 = true;
							}
							else
							{
								if (s.V07.Value == 0)
								{
									s.IsQualifiedV7 = true;
								}
								else
								{
									s.IsQualifiedV7 = false;
								}

							}
						}





					}
				}
			}
            catch (Exception)
            {
				 
            }
			

		}
		 

        public DataQualified()
		{
			CurrentProductData = new DictionaryExt<Address, SensorGroupData>(); 
		}

	}

	public class DictionaryExt<TKey, TValue> : Dictionary<TKey, TValue> {
		public void TryAdd(TKey k, TValue v) {
			if (this.ContainsKey(k)) this[k] = v;
			else this.Add(k, v);
		}
		public TValue TryGet(TKey k)
		{
			if (this.ContainsKey(k)) return this[k];
			else return default(TValue);
		}
	}

	public struct Address
	{ 
		public Address(int v,int app)
		{
			V = v;
			APP = app;
		}
		public int V { get; set; }
		public int APP { get; set; }
		public byte Vb { get { return (byte)V; } set { V = (byte)value; } }

		//重载加法运算符+  
		public static bool operator ==(Address f, Address g)
		{
			return f.V == g.V && f.APP == g.APP;
		}

		public static bool operator !=(Address f, Address g)
		{
			return f.V != g.APP || f.V != g.APP; 
		}

		public override string ToString()
		{
			if (APP > 0) return string.Format("{0}#{1}{2}", APP, ((V + 1) / 2).ToString(), V % 2 == 0 ? "右" : "左");
			return V == 0 ? "" : V.ToString("00");
        }

    }

}
