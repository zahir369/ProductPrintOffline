//using OfficeOpenXml;
//using OfficeOpenXml.Style;
//using System;
//using System.IO;

//namespace MKSS.APP.UIBiaoDing.Util
//{
//    public class ExcuteToExcel : IDisposable
//	{
//		public string FilePath
//		{
//			get;
//			set;
//		}

//		public ExcelPackage Package
//		{
//			get;
//			set;
//		}

//		public ExcelWorksheet Sheet
//		{
//			get;
//			set;
//		}

//		public ExcuteToExcel()
//		{
//			this.Package = new ExcelPackage();
//			this.Sheet = this.Package.Workbook.Worksheets.Add("历史数据");
//		}

//		public void SetCellValue<T>(int row, int column, T t)
//		{
//			bool flag = t == null;
//			if (flag)
//			{
//				this.Sheet.Cells[row, column].Value = "";
//			}
//			else
//			{
//				this.Sheet.Cells[row, column].Value = t;
//			}
//		}

//		public void AllCellsAlignment(ExcelHorizontalAlignment Alignment)
//		{
//			this.Sheet.Cells.Style.HorizontalAlignment = Alignment;
//		}

//		public void SetCellColor(int row, int column, System.Drawing.Color c)
//		{
//			this.Sheet.Cells[row, column].Style.Font.Color.SetColor(c);
//		}

//		public void SetRowColor(int row, System.Drawing.Color c)
//		{
//			this.Sheet.Row(row).Style.Font.Color.SetColor(c);
//		}

//		public void SetRowStyleOfFontToBold(int rownum)
//		{
//			for (int i = 0; i < rownum; i++)
//			{
//				this.Sheet.Row(rownum).Style.Font.Bold = true;
//			}
//		}

//		public void SetColoumnStyleOfFontToBold(int coloumnnum)
//		{
//			for (int i = 0; i < coloumnnum; i++)
//			{
//				this.Sheet.Column(coloumnnum).Style.Font.Bold = true;
//			}
//		}

//		public void SetCellValue(int row, int column, DateTime value)
//		{
//			this.Sheet.Cells[row, column].Value = value;
//			this.Sheet.Cells[row, column].Style.Font.Bold = true;
//		}

//		public void SetCellBackColor(int row, int column, System.Drawing.Color c)
//		{
//			this.Sheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
//			this.Sheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(c);
//		}

//		public void SetColumnColor(int column, System.Drawing.Color c)
//		{
//			this.Sheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
//			this.Sheet.Column(column).Style.Fill.BackgroundColor.SetColor(c);
//		}

//		public void SetColumnWidth(int column, int Width)
//		{
//			this.Sheet.Column(column).Width = (double)Width;
//		}

//		public void SaveAsExcel()
//		{
//			this.Package.SaveAs(new FileInfo(this.FilePath));
//		}

//		public void Dispose()
//		{
//			this.Package.Dispose();
//		}
//	}


//}
