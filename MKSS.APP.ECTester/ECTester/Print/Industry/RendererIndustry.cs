using MKSS.Service.ECTester;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MKSS.APP.ECTester.Print
{
    public class RendererIndustry : PrintRendererInterface
    {

        public XMode PrintMode = XMode.None;
        public RendererIndustry(XMode v) {
            PrintMode = v;
        }

        public FlowDocument LoadDocument(PrintDataList data)
        {
            string strTmplName = "ECTester\\Print\\Industry\\Document.xaml";
            FlowDocument doc = (FlowDocument)Application.LoadComponent(new Uri(strTmplName, UriKind.RelativeOrAbsolute));
            doc.PagePadding = new Thickness(0);
            doc.DataContext = data;
            this.FillData(doc, data);
            return doc;
        }

        void FillData(FlowDocument doc, PrintDataList data)
        {
            ////if (1 == 1) return;
            PrintDataList docData = (PrintDataList)data;

            //缓存
            Dictionary<string, PrintDataItem> all = new Dictionary<string, PrintDataItem>();
            foreach (PrintDataItem sn in docData.List)
            {
                try
                {
                    all.Add(string.Format("{0}_{1}", sn.REGION_ROW, sn.REGION_COL), sn);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            TableRowGroup group = doc.FindName("rowsDetails") as TableRowGroup;
            Style CellStyle = doc.Resources["CellStyle"] as Style;
            Style ParagraphStyle = doc.Resources["ParagraphStyle"] as Style;
            Style TextBlockStyle = doc.Resources["TextBlockStyle"] as Style;

            group.Rows.Clear();

            //工业传感器 8 * 8， 4 * *
            for (int c = 8; c >= 1; c--)
            {
                TableRow rowNew = new TableRow();
                for (int r = 8; r >= 1; r--)
                {
                    PrintDataItem sn = all[string.Format("{0}_{1}", r, c)];
                    Paragraph myParagraph = new Paragraph();
                    myParagraph.Style = ParagraphStyle;
                    TextBlock text = new TextBlock();
                    text.Style = TextBlockStyle;
                    text.VerticalAlignment = VerticalAlignment.Top;
                    text.HorizontalAlignment = HorizontalAlignment.Right;
                    text.Text = string.Format("{2}", sn.REGION_ROW, sn.REGION_COL, sn.CONTENT);
                    //text.Text = string.Format("行{0}.列{1}", sn.REGION_ROW, sn.REGION_COL, sn.CONTENT);
                    text.LayoutTransform = new RotateTransform(90);
                    text.RenderTransform = new ScaleTransform(1.2, 1);
                    myParagraph.Inlines.Add(text);

                    TableCell pCell = new TableCell(myParagraph);
                    pCell.Style = CellStyle;
                    rowNew.Cells.Add(pCell);
                }
                group.Rows.Add(rowNew);

            }



        }
         
    }
}
