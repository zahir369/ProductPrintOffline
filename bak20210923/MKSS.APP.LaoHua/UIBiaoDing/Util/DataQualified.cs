using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MKSS.APP.UIBiaoDing.Util
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
			get
			{
				if (!IsQualifiedCache.ContainsKey(boardNo)) return null;
				if (!IsQualifiedCache[boardNo].ContainsKey(lineIndex)) return null;
				return IsQualifiedCache[boardNo][lineIndex];
			}
			set
			{
				if (!IsQualifiedCache.ContainsKey(boardNo)) IsQualifiedCache.Add(boardNo, new Dictionary<int, bool?>());
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

		public static void CalcAvg(List<DataQualified> qss ,out int v1Avg, out int v7Avg)
		{ 
			List<int> V7_LIST = new List<int>();
			List<int> V1_LIST = new List<int>();

            foreach (var this_q in qss)
            {
				for (int i = 0; i < this_q.CurrentProductData.Count; i++)
				{
					for (int j = 0; j < 15; j++)
					{
						int? numV1 = this_q.CurrentProductData[i].SingleAddressData[j].V01;
						if (numV1.HasValue && numV1.GetValueOrDefault() != 170)
						{
							V1_LIST.Add(this_q.CurrentProductData[i].SingleAddressData[j].V01.Value);
							int? numV7 = this_q.CurrentProductData[i].SingleAddressData[j].V07;
							if (numV7.HasValue && numV7.GetValueOrDefault() != 0)
							{
								V7_LIST.Add(this_q.CurrentProductData[i].SingleAddressData[j].V07.Value);
							}
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
					SensormData s = this.CurrentProductData[i].SingleAddressData[j];
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

		//public void SaveDataToExcel()
		//{
		//	ExcuteToExcel ETE = new ExcuteToExcel();
		//	for (int i = 0; i < this.CurrentProductData.Count; i++)
		//	{
		//		for (int j = 0; j < 15; j++)
		//		{
		//			ETE.SetCellValue<string>(1, i * 241 + 1 + j * 16, "产品位置");
		//			ETE.SetCellValue<string>(1, i * 241 + 2 + j * 16, "C1(84-23)");
		//			ETE.SetCellValue<string>(1, i * 241 + 3 + j * 16, "C2(84-45)");
		//			ETE.SetCellValue<string>(1, i * 241 + 4 + j * 16, "C4(85-23)");
		//			ETE.SetCellValue<string>(1, i * 241 + 5 + j * 16, "C7(86-23)");
		//			ETE.SetCellValue<string>(1, i * 241 + 6 + j * 16, "C8(86-4)");
		//			ETE.SetCellValue<string>(1, i * 241 + 7 + j * 16, "是否合格");
		//		}
		//		ETE.SetCellValue<string>(1, 241 * (i + 1), "接收时间");
		//		for (int k = 0; k < 240; k++)
		//		{
		//			ETE.SetColumnWidth(i * 241 + k + 1, 19);
		//		}
		//		ETE.SetColumnWidth(241 * (i + 1), 60);
		//		bool flag = this.HistoryProductData.Count > i;
		//		if (flag)
		//		{
		//			for (int l = 0; l < this.HistoryProductData[i].Count; l++)
		//			{
		//				for (int m = 0; m < 15; m++)
		//				{
		//					ETE.SetCellValue<string>(l + 2, i * 241 + 1 + m * 16, string.Format("{0}->{1}", this.HistoryProductData[i][l].Address, this.HistoryProductData[i][l].SingleAddressData[m].Position));
		//					ETE.SetCellValue<int>(l + 2, i * 241 + 2 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].V01.Value);
		//					ETE.SetCellValue<int>(l + 2, i * 241 + 3 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].V02.Value);
		//					ETE.SetCellValue<int>(l + 2, i * 241 + 4 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].V04.Value);
		//					ETE.SetCellValue<int>(l + 2, i * 241 + 5 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].V07.Value);
		//					ETE.SetCellValue<int>(l + 2, i * 241 + 6 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].V08.Value);
		//					//ETE.SetCellValue<string>(l + 2, i * 241 + 7 + m * 16, this.HistoryProductData[i][l].SingleAddressData[m].IsQualified.ToString());
		//				}
		//				ETE.SetCellValue<string>(l + 2, 241 * (i + 1), this.HistoryProductData[i][l].Time);
		//			}
		//		}
		//	}
		//	ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
		//	ETE.SetRowStyleOfFontToBold(1);
		//	string Year = DateTime.Now.Year.ToString();
		//	string Month = DateTime.Now.Month.ToString();
		//	string Day = DateTime.Now.Day.ToString();
		//	bool flag2 = !Directory.Exists(string.Concat(new string[]
		//	{
		//		Directory.GetCurrentDirectory(),
		//		"\\历史数据 \\",
		//		Year,
		//		"年\\",
		//		Month,
		//		"月\\",
		//		Day,
		//		"日"
		//	}));
		//	if (flag2)
		//	{
		//		Directory.CreateDirectory(string.Concat(new string[]
		//		{
		//			Directory.GetCurrentDirectory(),
		//			"\\历史数据 \\",
		//			Year,
		//			"年\\",
		//			Month,
		//			"月\\",
		//			Day,
		//			"日"
		//		}));
		//	}
		//	ETE.FilePath = string.Concat(new string[]
		//	{
		//		Directory.GetCurrentDirectory(),
		//		"\\历史数据 \\",
		//		Year,
		//		"年\\",
		//		Month,
		//		"月\\",
		//		Day,
		//		"日\\",
		//		DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
		//		".xlsx"
		//	});
		//	ETE.SaveAsExcel();
		//	ETE.Dispose();
		//}

		//public void SaveCheckedDataToExcel()
		//{
		//	ExcuteToExcel ETE = new ExcuteToExcel();
		//	ETE.SetCellValue<string>(1, 1, "产品位置");
		//	ETE.SetCellValue<string>(1, 2, "C1(84-23)");
		//	ETE.SetCellValue<string>(1, 3, "C2(84-45)");
		//	ETE.SetCellValue<string>(1, 4, "C4(85-23)");
		//	ETE.SetCellValue<string>(1, 5, "C7(86-23)");
		//	ETE.SetCellValue<string>(1, 6, "C8（86-5）");
		//	ETE.SetCellValue<string>(1, 7, "是否合格");
		//	for (int i = 0; i < this.CurrentProductData.Count; i++)
		//	{
		//		for (int j = 0; j < 15; j++)
		//		{
		//			ETE.SetCellValue<string>(i * 15 + j + 2, 1, string.Format("{0}->{1}", this.CurrentProductData[i].Address, this.CurrentProductData[i].SingleAddressData[j].Position));
		//			ETE.SetCellValue<int>(i * 15 + j + 2, 2, this.CurrentProductData[i].SingleAddressData[j].V01.Value);
		//			ETE.SetCellValue<int>(i * 15 + j + 2, 3, this.CurrentProductData[i].SingleAddressData[j].V02.Value);
		//			ETE.SetCellValue<int>(i * 15 + j + 2, 4, this.CurrentProductData[i].SingleAddressData[j].V04.Value);
		//			ETE.SetCellValue<int>(i * 15 + j + 2, 5, this.CurrentProductData[i].SingleAddressData[j].V07.Value);
		//			ETE.SetCellValue<int>(i * 15 + j + 2, 6, this.CurrentProductData[i].SingleAddressData[j].V08.Value);
		//			//ETE.SetCellValue<string>(i * 15 + j + 2, 7, this.CurrentProductData[i].SingleAddressData[j].IsQualified.ToString());
		//			//if (this.CurrentProductData[i].SingleAddressData[j].IsQualified != null
		//			//	&&
		//			//	!this.CurrentProductData[i].SingleAddressData[j].IsQualified.Value)
		//			//{
		//			//	ETE.SetCellBackColor(i * 15 + j + 2, 7, System.Drawing.Color.LightPink);
		//			//}
		//		}
		//	}
		//	ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
		//	ETE.SetRowStyleOfFontToBold(1);
		//	string Year = DateTime.Now.Year.ToString();
		//	string Month = DateTime.Now.Month.ToString();
		//	string Day = DateTime.Now.Day.ToString();
		//	bool flag2 = !Directory.Exists(string.Concat(new string[]
		//	{
		//		Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
		//		"\\判定结果 \\",
		//		Year,
		//		"年\\",
		//		Month,
		//		"月\\",
		//		Day,
		//		"日"
		//	}));
		//	if (flag2)
		//	{
		//		Directory.CreateDirectory(string.Concat(new string[]
		//		{
		//			Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
		//			"\\判定结果 \\",
		//			Year,
		//			"年\\",
		//			Month,
		//			"月\\",
		//			Day,
		//			"日"
		//		}));
		//	}
		//	ETE.FilePath = string.Concat(new string[]
		//	{
		//		Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
		//		"\\判定结果 \\",
		//		Year,
		//		"年\\",
		//		Month,
		//		"月\\",
		//		Day,
		//		"日\\",
		//		DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
		//		".xlsx"
		//	});
		//	ETE.SaveAsExcel();
		//	ETE.Dispose();
		//}

		public DataQualified()
		{
			CurrentProductData = new List<SensorGroupData>();
			HistoryProductData = new List<List<SensorGroupData>>();
		}
	}


}
