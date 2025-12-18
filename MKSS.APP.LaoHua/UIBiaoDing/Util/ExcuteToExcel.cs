using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class ExcuteToExcel : IDisposable
    {
        public string FilePath
        {
            get;
            set;
        }

        public ExcelPackage Package
        {
            get;
            set;
        }

        public ExcelWorksheet Sheet
        {
            get;
            set;
        }

        public ExcuteToExcel( )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.Package = new ExcelPackage(); 
        }
        public ExcuteToExcel(string sheetname)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.Package = new ExcelPackage();
            CreateSheet(sheetname);
        }

        public void CreateSheet(string sheetname) {

            this.Sheet = this.Package.Workbook.Worksheets.Add(sheetname);
        }



        public void SetCellValue<T>(int row, int column, T t, string formate = null, int ToRow = -1, int ToCol = -1)
        {
            bool flag = t == null;
            bool merge = ToRow > 0 && ToCol > 0;
            if (merge)
            {
                this.Sheet.Cells[row, column, ToRow, ToCol].Merge = true; 
            } 
            if (flag)
            {
                if (merge)
                {
                    this.Sheet.Cells[row, column, ToRow, ToCol].Value = "";
                }
                else {

                    this.Sheet.Cells[row, column].Value = "";
                }
            }
            else
            {
                if (merge)
                {
                    this.Sheet.Cells[row, column, ToRow, ToCol].Value = t;
                }
                else
                {

                    this.Sheet.Cells[row, column].Value = t;
                }
                this.Sheet.Cells[row, column].Value = t;
            }
            if (!string.IsNullOrEmpty(formate))
            {
                if (merge)
                {
                    this.Sheet.Cells[row, column, ToRow, ToCol].Style.Numberformat.Format = formate;//or m/d/yy h:mm
                }
                else
                {
                    this.Sheet.Cells[row, column].Style.Numberformat.Format = formate;//or m/d/yy h:mm
                }
            }
        }

        public void AllCellsAlignment(ExcelHorizontalAlignment Alignment)
        {
            this.Sheet.Cells.Style.HorizontalAlignment = Alignment;
        }

        public void SetCellColor(int row, int column, System.Drawing.Color c)
        {
            this.Sheet.Cells[row, column].Style.Font.Color.SetColor(c);
        }

        public void SetRowColor(int row, System.Drawing.Color c)
        {
            this.Sheet.Row(row).Style.Font.Color.SetColor(c);
        }

        public void SetRowStyleOfFontToBold(int rownum)
        {
            for (int i = 0; i < rownum; i++)
            {
                this.Sheet.Row(rownum).Style.Font.Bold = true;
            }
        }

        public void SetColoumnStyleOfFontToBold(int coloumnnum)
        {
            for (int i = 0; i < coloumnnum; i++)
            {
                this.Sheet.Column(coloumnnum).Style.Font.Bold = true;
            }
        }

        public void SetCellValue(int row, int column, DateTime value)
        {
            this.Sheet.Cells[row, column].Value = value;
            this.Sheet.Cells[row, column].Style.Font.Bold = true;
        }

        public void SetCellBackColor(int row, int column, System.Drawing.Color c)
        {
            this.Sheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
            this.Sheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(c);
        }

        public void SetColumnColor(int column, System.Drawing.Color c)
        {
            this.Sheet.Column(column).Style.Fill.PatternType = ExcelFillStyle.Solid;
            this.Sheet.Column(column).Style.Fill.BackgroundColor.SetColor(c);
        }

        public void SetColumnWidth(int column, int Width)
        {
            this.Sheet.Column(column).Width = (double)Width;
        }

        public void SaveAsExcel()
        {
            this.Package.SaveAs(new FileInfo(this.FilePath));
        }

        public void Dispose()
        {
            this.Package.Dispose();
        }
    }


}
