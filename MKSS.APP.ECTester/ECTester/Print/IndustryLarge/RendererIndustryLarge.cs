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
    public class RendererIndustryLarge : PrintRendererInterface
    {

        public XMode PrintMode = XMode.None;
        public Thickness Margin { get; set; }
        public string IndustryModeLabelLargeContent { get; set; }
        public RendererIndustryLarge(XMode v) {
            PrintMode = v;
            IndustryModeLabelLargeContent = ECTesterConfgig.Instance.IndustryModeLabelLargeContent;
            Margin = new Thickness(
                ToPixels(ECTesterConfgig.Instance.IndustryModeLabelXmm), 
                ToPixels(ECTesterConfgig.Instance.IndustryModeLabelYmm), 0, 0);
        }

        public FlowDocument LoadDocument(PrintDataList data)
        {
            string strTmplName = "ECTester\\Print\\IndustryLarge\\Document.xaml";
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
            Style BlockUIContainerStyle = doc.Resources["BlockUIContainerStyle"] as Style;
            //Style TextBlockStyle = doc.Resources["TextBlockStyle"] as Style;

            group.Rows.Clear();

            //工业传感器 8 * 8， 4 * *
            for (int i = docData.List.Count-1; i >= 0; i--)
            {

                PrintDataItem sn = docData.List[i];
                TextBlock text = new TextBlock();
                //text.Style = TextBlockStyle;
                text.Margin = Margin;
                text.Text = IndustryModeLabelLargeContent.Replace("{灵敏度}", sn.CONTENT);

                BlockUIContainer _BlockUIContainer = new BlockUIContainer(text);
                _BlockUIContainer.Style = BlockUIContainerStyle;

                TableCell pCell = new TableCell(_BlockUIContainer);
                pCell.Style = CellStyle;

                TableRow rowNew = new TableRow();
                rowNew.Cells.Add(pCell);
                group.Rows.Add(rowNew);

            }


        }

        //1 英寸=25.4 毫米
        public int ToPixels(double len_mm) {
            return (int)((len_mm / 25.4) * 96);
        }


    }
}
