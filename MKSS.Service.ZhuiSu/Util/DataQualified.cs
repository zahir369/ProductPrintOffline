using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MKSS.Service.ZhuiSu.Util
{
	public class DataQualified
	{

		public List<SensorGroupData> CurrentProductData
		{
			get;
			set;
		}

		public List<List<SensorGroupData>> HistoryProductData
		{
			get;
			set;
		}

		Dictionary<int, Dictionary<int, bool?>> IsQualifiedCache = new Dictionary<int, Dictionary<int, bool?>>();
		public bool? this[int boardNo, int lineIndex]
		{
			get{
				if (!IsQualifiedCache.ContainsKey(boardNo)) return null;
				if (!IsQualifiedCache[boardNo].ContainsKey(lineIndex)) return null;
				return IsQualifiedCache[boardNo][lineIndex];
			}
			set{
				if (!IsQualifiedCache.ContainsKey(boardNo)) IsQualifiedCache.Add(boardNo,new Dictionary<int, bool?>());
				if (!IsQualifiedCache[boardNo].ContainsKey(lineIndex)) IsQualifiedCache[boardNo].Add(lineIndex, value);
				else IsQualifiedCache[boardNo][lineIndex] = value;
			}
		}

		/// <summary>
		///  不合格品
		/// </summary>
		/// <param name="boardNo"></param>
		/// <param name="lineIndex"></param>
		/// <returns></returns>
		public bool NotQualified(int boardNo, int lineIndex) {
			if (!IsQualifiedCache.ContainsKey(boardNo)) return false;
			if (!IsQualifiedCache[boardNo].ContainsKey(lineIndex)) return false;
			return IsQualifiedCache[boardNo][lineIndex]!=null && !IsQualifiedCache[boardNo][lineIndex].Value;
		}

		public int StaQuallified
		{
			get { 
				if (CurrentProductData == null) return 0;
				return StaTotal- StaUnqualified;
			}
		}

		public int StaUnqualified
		{
			get
			{
				if (CurrentProductData == null) return 0;
				int ret = 0;
				///StringBuilder ss = new StringBuilder();
                foreach (SensorGroupData item in CurrentProductData)
                {
                    foreach (var s in item.SingleAddressData)
					{
						if (s.IsEmpData) continue;
						if (s.IsEmpSensor) continue;
						if (s.IsEmp170) continue;
						if (s.IsQualifiedV7 != null && !s.IsQualifiedV7.Value) {
							ret++;
							///ss.Append(string.Format("{0}_{1};"+System.Environment.NewLine, item.Address, s.Position));
						}
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
				return CurrentProductData.Sum(w => w.SingleAddressData.Count(z => 
				!z.IsEmpSensor && !z.IsEmp170));
			}
		}

		public void Clear()
		{
			this.CurrentProductData.Clear();
			this.HistoryProductData.Clear();
		}

		public void CalcAvg(out int v1Avg, out int v7Avg)
		{ 
			List<int> V7_LIST = new List<int>();
			List<int> V1_LIST = new List<int>();
			for (int i = 0; i < this.CurrentProductData.Count; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					int? numV1 = this.CurrentProductData[i].SingleAddressData[j].V01;
					if (numV1.HasValue && numV1.GetValueOrDefault() != 170 )
					{
						V1_LIST.Add(this.CurrentProductData[i].SingleAddressData[j].V01.Value);
						int? numV7 = this.CurrentProductData[i].SingleAddressData[j].V07;
						if (numV7.HasValue && numV7.GetValueOrDefault() != 0)
						{
							V7_LIST.Add(this.CurrentProductData[i].SingleAddressData[j].V07.Value);
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
			 
			for (int i = 0; i < this.CurrentProductData.Count; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					SensorData s = this.CurrentProductData[i].SingleAddressData[j];
					if (s.IsEmpData || s.IsEmpSensor) {
						s.IsQualifiedV1 = null;
						s.IsQualifiedV7 = null; 
						continue;
					}

					if (s.V01.Value >= baseValue01 - OffSideDown01 && s.V01.Value <= baseValue01 + OffSideUp01)
					{
						s.IsQualifiedV1 = true;
					}
					else {
						s.IsQualifiedV1 = false;
					}

					if (s.V07.Value >= baseValue07 - OffSideDown07 && s.V07.Value <= baseValue07 + OffSideUp07)
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

		 

		public DataQualified()
		{
			CurrentProductData = new List<SensorGroupData>();
			HistoryProductData = new List<List<SensorGroupData>>();
		}
	}


}
