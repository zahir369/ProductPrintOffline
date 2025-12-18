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
    public class RendererAqueous : PrintRendererInterface
    {

        public XMode PrintMode = XMode.None;
        public RendererAqueous(XMode v) {
            PrintMode = v;
        }

        public FlowDocument LoadDocument(PrintDataList data)
        {
            string strTmplName = "ECTester\\Print\\Aqueous\\Document.xaml";
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

            //水性传感器 16 * 4， 8 * 2
            for (int r = 8; r >= 1; r--)
            {
                TableRow rowNew = new TableRow();

                int lineCount = 0;
                for (int c = 8; c >= 1; c--)
                {

                    string str = "";
                    string k = string.Format("{0}_{1}", r, c);
                    if (all.ContainsKey(k))
                    {
                        PrintDataItem sn = all[k];
                        str = string.Format("{2}", sn.REGION_ROW, sn.REGION_COL, sn.CONTENT);
                        lineCount++;
                    } 

                    Paragraph myParagraph = new Paragraph();
                    myParagraph.Style = ParagraphStyle;
                    TextBlock text = new TextBlock();
                    text.Style = TextBlockStyle;
                    text.VerticalAlignment = VerticalAlignment.Top;
                    text.HorizontalAlignment = HorizontalAlignment.Right;
                    text.Text = str;
                    //text.Text = string.Format("行{0}.列{1}", sn.REGION_ROW, sn.REGION_COL, sn.CONTENT);
                    text.LayoutTransform = new RotateTransform(90);
                    text.RenderTransform = new ScaleTransform(1.2, 1);
                    myParagraph.Inlines.Add(text);

                    TableCell pCell = new TableCell(myParagraph);
                    pCell.Style = CellStyle;
                    rowNew.Cells.Add(pCell);
                }
                if (lineCount > 0) {
                    group.Rows.Add(rowNew);
                }

            }



        }
         
    }
}
