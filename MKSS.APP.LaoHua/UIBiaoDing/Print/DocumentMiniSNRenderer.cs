using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Common;

namespace MKSS.APP.LaoHua.Print
{
    public enum BarCodeMode { 
        SerialCode, SerialCodeWithValidCode
    }
    class DocumentMiniSNRenderer : IDocumentRenderer
    {

        public BarCodeMode PrintMode = BarCodeMode.SerialCode;
        public DocumentMiniSNRenderer(BarCodeMode v) {
            PrintMode = v;
        }
        public void Render(FlowDocument doc, object data)
        {
            //if (1 == 1) return;
            DocumentMiniSNData docData = (DocumentMiniSNData)data;

            TableRowGroup group = doc.FindName("rowsDetails") as TableRowGroup;
            Style styleCell = doc.Resources["BorderedCell"] as Style;
            Style ParagraphImage = doc.Resources["ParagraphImage"] as Style;
            //Style ParagraphCellImage = doc.Resources["ParagraphCellImage"] as Style;
            //Style ParagraphCellTextL1 = doc.Resources["ParagraphCellTextL1"] as Style;
            //Style ParagraphCellTextL2 = doc.Resources["ParagraphCellTextL2"] as Style;

            group.Rows.Clear();
            foreach (SN_ITEM sn in docData.SN_LIST)
            {

                //成图
                System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                image.Style = ParagraphImage;
                BarcodeWriter writer = new BarcodeWriter();
                writer.Format = BarcodeFormat.QR_CODE;
                EncodingOptions options = new EncodingOptions()
                {
                    PureBarcode = true,
                    Width = 450,
                    Height = 450,
                    Margin = 0
                };
                writer.Options = options;
                string code = string.Format("https://xiaofang.mixsense-iot.com?sn={0}", sn.SN + ";" + sn.F_SerialValidCode);
                if (PrintMode == BarCodeMode.SerialCode) {
                    code = sn.SN;
                }
                if (PrintMode == BarCodeMode.SerialCodeWithValidCode)
                {
                    code = sn.SN + ";" + sn.F_SerialValidCode;
                }
                Bitmap map = writer.Write(code);
                image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(map.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());//  BitmapToBitmapImage(map);
                  
                //图布局
                Figure myFigureLeft = new Figure();
                myFigureLeft.Width = new FigureLength(0.4, FigureUnitType.Page);
                myFigureLeft.Height = new FigureLength(1, FigureUnitType.Page);
                //myFigure.Background = System.Windows.Media.Brushes.Red;
                myFigureLeft.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
                myFigureLeft.BorderThickness = new Thickness(0);
                myFigureLeft.Padding = new Thickness(0);
                Paragraph myFigureLeftParagraph = new Paragraph();
                myFigureLeftParagraph.Inlines.Add(image);
                myFigureLeftParagraph.Padding = new Thickness(0);
                myFigureLeftParagraph.Margin = new Thickness(0);
                myFigureLeftParagraph.BorderThickness = new Thickness(0);
                //myFigureParagraph.Background = System.Windows.Media.Brushes.Pink;
                myFigureLeft.Blocks.Add(myFigureLeftParagraph);

                //Figure myFigureRight = new Figure();
                //myFigureRight.Width = new FigureLength(0.4, FigureUnitType.Page);
                //myFigureRight.Height = new FigureLength(1, FigureUnitType.Page);
                //myFigureRight.Background = System.Windows.Media.Brushes.BlueViolet;
                //myFigureRight.HorizontalAnchor = FigureHorizontalAnchor.PageRight;
                //myFigureRight.BorderThickness = new Thickness(0);
                //myFigureRight.Padding = new Thickness(0);
                //Paragraph myFigureRightParagraph = new Paragraph();
                //myFigureRightParagraph.Inlines.Add(new Run(sn.SN.Substring(0, 6) + System.Environment.NewLine));
                //myFigureRightParagraph.Inlines.Add(new Run(sn.SN.Substring(6, 6) + System.Environment.NewLine));
                //myFigureRightParagraph.Inlines.Add(new Run(string.Format("{0}#{1}" + (sn.APP > 0 ? "@" : ""), sn.ADDR, sn.POS)));
                //myFigureRightParagraph.Padding = new Thickness(0);
                //myFigureRightParagraph.Margin = new Thickness(0);
                //myFigureRightParagraph.BorderThickness = new Thickness(0);
                ////myFigureParagraph.Background = System.Windows.Media.Brushes.Pink;
                //myFigureRight.Blocks.Add(myFigureRightParagraph);

                //图布局到文字区
                Paragraph myParagraph = new Paragraph();
                myParagraph.Padding = new Thickness(0);
                myParagraph.Margin = new Thickness(0);
                myParagraph.Inlines.Add(myFigureLeft);
                //myParagraph.Inlines.Add(myFigureRight);
                //myParagraph.Background = System.Windows.Media.Brushes.Green;
                myParagraph.Inlines.Add(new Run(System.Environment.NewLine) { });
                myParagraph.Inlines.Add(new Run(sn.SN.Substring(0, 6) + System.Environment.NewLine));
                myParagraph.Inlines.Add(new Run(sn.SN.Substring(6, 6) + System.Environment.NewLine));
                myParagraph.Inlines.Add(new Run(string.Format("{0}#{1}" + (sn.APP > 0 ? "@" : ""), sn.ADDR, sn.POS)));

                TableRow rowNew = new TableRow();
                TableCell pCell = new TableCell(myParagraph);
                pCell.Style = styleCell;
                rowNew.Cells.Add(pCell);

                group.Rows.Add(rowNew);

            }
             
        }

        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms,  bitmap.RawFormat);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            return bitmapImage;
        }
    }
}
