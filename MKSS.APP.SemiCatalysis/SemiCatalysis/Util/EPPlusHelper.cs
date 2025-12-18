using System;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace MKSS.APP.SemiCatalysis
{
    public class EPPlusHelper
    {
        private static int i;

        /// <summary>
        /// 导入数据到Excel中
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ds"></param>
        public static bool ImportExcel(string fileName, DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
            {
                return false;
            }
            FileInfo file = new FileInfo(fileName);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(fileName);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //在using语句里面我们可以创建多个worksheet，ExcelPackage后面可以传入路径参数
            //命名空间是using OfficeOpenXml
            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (DataTable dt in ds.Tables)
                {
                    //创建工作表worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);
                    //给单元格赋值有两种方式
                    //worksheet.Cells[1, 1].Value = "单元格的值";直接指定行列数进行赋值
                    //worksheet.Cells["A1"].Value = "单元格的值";直接指定单元格进行赋值
                    worksheet.Cells.Style.Font.Name = "微软雅黑";
                    worksheet.Cells.Style.Font.Size = 12;
                    worksheet.Cells.Style.ShrinkToFit = false;//单元格自动适应大小 

                    for (int i = 0; i < dt.Columns.Count; i++)
                    { 
                        worksheet.Cells[1, i + 1].Value = dt.Columns[i].ColumnName;
                        if (dt.Columns[i].DataType == typeof(DateTime))
                        {
                            worksheet.Column(i + 1).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                            worksheet.Column(i + 1).Width = 32;
                        }
                        else {
                            worksheet.Column(i + 1).Width = 16;
                        }
                    }
                    using (var cell = worksheet.Cells[1, 1, 1, dt.Columns.Count])
                    {
                        //设置样式:首行居中加粗背景色
                        cell.Style.Font.Bold = true; //加粗
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center; //水平居中
                        cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;     //垂直居中
                        cell.Style.Font.Size = 14;
                        cell.Style.Fill.PatternType = ExcelFillStyle.Solid;  //背景颜色
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(128, 128, 128));//设置单元格背景色
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1].Value = dt.Rows[i][j];
                            
                        }
                    }
                    
                }
                //保存
                package.Save();
            }
            return true;
        }

        /// <summary>
        /// 读取Excel数据
        /// </summary>
        /// <param name="fileName"></param>
        public static string ReadExcel(string fileName)
        {
            StringBuilder sb = new StringBuilder();
            FileInfo file = new FileInfo(fileName);
            try
            {
                using (ExcelPackage package = new ExcelPackage(file))
                {
                    var count = package.Workbook.Worksheets.Count;
                    for (int k = 1; k <= count; k++)  //worksheet是从1开始的
                    {
                        var workSheet = package.Workbook.Worksheets[k];
                        sb.Append(workSheet.Name);
                        sb.Append(Environment.NewLine);
                        int row = workSheet.Dimension.Rows;
                        int col = workSheet.Dimension.Columns;
                        for (int i = 1; i <= row; i++)
                        {
                            for (int j = 1; j <= col; j++)
                            {
                                sb.Append(workSheet.Cells[i, j].Value.ToString() + "\t");
                            }
                            sb.Append(Environment.NewLine);
                        }
                        sb.Append(Environment.NewLine);
                        sb.Append(Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                return "An error had Happen";
            }
            return sb.ToString();
        }
    }

}
