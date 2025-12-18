using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing.Util
{

	/// <summary>
	///  老化
	/// </summary>
	public class DataAging
	{
		
		public int TunnelNo { get; set; }
		public int BoardCaseNo { get; set;}
		Dictionary<int, Dictionary<int, DataAgingSensor>> DataAgingSensorCache = new Dictionary<int, Dictionary<int, DataAgingSensor>>();
		public DataAgingSensor this[int boardNo, int lineIndex]
		{
			get{
				if (!DataAgingSensorCache.ContainsKey(boardNo)) {
					DataAgingSensorCache.Add(boardNo, new Dictionary<int, DataAgingSensor>());
				}
				if (!DataAgingSensorCache[boardNo].ContainsKey(lineIndex)) {
					DataAgingSensorCache[boardNo].Add(lineIndex, new DataAgingSensor(boardNo, lineIndex));
				}
				return DataAgingSensorCache[boardNo][lineIndex];
			} 
		}

		public void Reset(List<byte> address)
		{
            foreach (var item in address)
            {
				int boardNo = (int)item;
				if (DataAgingSensorCache.ContainsKey(boardNo))
				{
                    foreach (DataAgingSensor sen in DataAgingSensorCache[boardNo].Values)
                    {
						sen.Reset();

					}
				}
			}
		}

		///// <summary>
		/////  老化程度
		///// </summary>
		///// <param name="boardNo"></param>
		///// <param name="lineIndex"></param>
		///// <returns></returns>
		//public bool NotQualified(int boardNo, int lineIndex) {
		//	if (!DataAgingSensorCache.ContainsKey(boardNo)) return false;
		//	if (!DataAgingSensorCache[boardNo].ContainsKey(lineIndex)) return false;
		//	return DataAgingSensorCache[boardNo][lineIndex].Value != null && !DataAgingSensorCache[boardNo][lineIndex].Value.Value;
		//}

		//public int StaQuallified
		//{
		//	get { 
		//		if (CurrentProductData == null) return 0;
		//		return StaTotal- StaUnqualified;
		//	}
		//}

		//public int StaUnqualified
		//{
		//	get
		//	{
		//		if (CurrentProductData == null) return 0;
		//		return CurrentProductData.Sum(w => w.SingleAddressData.Count(z => !z.IsEmpSensor && (this[w.AddressInt, z.Position] != null && !this[w.AddressInt, z.Position].Value)));
		//	}
		//}

		//public double StaRate
		//{
		//	get {
		//		double _HeGeRatio = (double)this.StaQuallified / (double)StaTotal;
		//		return _HeGeRatio;
		//	}
		//}


		//public int StaTotal
		//{
		//	get
		//	{
		//		if (CurrentProductData == null) return 0;
		//		return CurrentProductData.Sum(w => w.SingleAddressData.Count(z => !z.IsEmpSensor));
		//	}
		//}

		//public void Clear()
		//{
		//	this.CurrentProductData.Clear();
		//	this.HistoryProductData.Clear();
		//}

		public void CalcAvg(out int v1Avg )
		{
			v1Avg = 0;
			//List<int> V7_LIST = new List<int>();
			//List<int> V1_LIST = new List<int>();
			//for (int i = 0; i < this.CurrentProductData.Count; i++)
			//{
			//	for (int j = 0; j < 15; j++)
			//	{
			//		int? numV1 = this.CurrentProductData[i].SingleAddressData[j].V01;
			//		if (numV1.HasValue && numV1.GetValueOrDefault() != 170 )
			//		{
			//			V1_LIST.Add(this.CurrentProductData[i].SingleAddressData[j].V01.Value);
			//			int? numV7 = this.CurrentProductData[i].SingleAddressData[j].V07;
			//			if (numV7.HasValue && numV7.GetValueOrDefault() != 0)
			//			{
			//				V7_LIST.Add(this.CurrentProductData[i].SingleAddressData[j].V07.Value);
			//			}
			//		}
			//	}
			//}
			//V1_LIST.Sort();
			//V7_LIST.Sort();


			//int PaiChuV7 = V7_LIST.Count / 10;
			//if (V7_LIST.Count <= 2)
			//{
			//	PaiChuV7 = 0;
			//}
			//else
			//{
			//	if (PaiChuV7 == 0)
			//	{
			//		PaiChuV7 = 1;
			//	}
			//}

			//if (V7_LIST.Count > 2)
			//{
			//	V7_LIST.RemoveRange(V7_LIST.Count - PaiChuV7, PaiChuV7);
			//	V7_LIST.RemoveRange(0, PaiChuV7);
			//}

			//if (V7_LIST.Count > 0)
			//{
			//	v7Avg = V7_LIST.Sum() / V7_LIST.Count;
			//}
			//else {
			//	v7Avg = 0;
			//}

			//int PaiChuV1 = V1_LIST.Count / 10;
			//if (V1_LIST.Count <= 2)
			//{
			//	PaiChuV1 = 0;
			//}
			//else
			//{
			//	if (PaiChuV1 == 0)
			//	{
			//		PaiChuV1 = 1;
			//	}
			//}

			//if (V1_LIST.Count > 2)
			//{
			//	V1_LIST.RemoveRange(V1_LIST.Count - PaiChuV1, PaiChuV1);
			//	V1_LIST.RemoveRange(0, PaiChuV1);
			//}

			//if (V1_LIST.Count > 0)
			//{
			//	v1Avg = V1_LIST.Sum() / V1_LIST.Count;
			//}
			//else {
			//	v1Avg = 0;
			//}

		}

		public void SetQualifiedV1V7( 
			int baseValue01, int OffSideDown01, int OffSideUp01)
		{
			 
			//for (int i = 0; i < this.CurrentProductData.Count; i++)
			//{
			//	for (int j = 0; j < 15; j++)
			//	{
			//		SensorData s = this.CurrentProductData[i].SingleAddressData[j];
			//		if (s.IsEmpData || s.IsEmpSensor) {
			//			s.IsQualifiedV1 = null;
			//			s.IsQualifiedV7 = null; 
			//			continue;
			//		}

			//		if (s.V01.Value >= baseValue01 - OffSideDown01 && s.V01.Value <= baseValue01 + OffSideUp01)
			//		{
			//			s.IsQualifiedV1 = true;
			//		}
			//		else {
			//			s.IsQualifiedV1 = false;
			//		}

			//		if (s.V07.Value >= baseValue07 - OffSideDown07 && s.V07.Value <= baseValue07 + OffSideUp07)
			//		{
			//			s.IsQualifiedV7 = true;
			//		}
			//		else
			//		{
			//			s.IsQualifiedV7 = false;
			//		}

			//	}
			//}

		}
		 
		public DataAging(int no,int tunnel)
		{
			BoardCaseNo = no;
			TunnelNo = no;
		}

    }


}
