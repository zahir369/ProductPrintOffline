using MKSS.APP.BiaodingAlcohol.UIBiaoDing;
using MKSS.APP.UserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MKSS.APP.UIBiaoDing.Print
{
    partial class PrintPreviewWindow : Window
    {
        static Seagull.BarTender.Print.Engine btEngine = null;
        static Dictionary<string, Seagull.BarTender.Print.LabelFormatDocument> labelFormatDic = new Dictionary<string, Seagull.BarTender.Print.LabelFormatDocument>();
        static Seagull.BarTender.Print.LabelFormatDocument SetupLabelFormat(string strTmplName)
        {
            if (!labelFormatDic.ContainsKey(strTmplName))
            {
                //加载速度慢,静态只加载一次
                if (btEngine == null)
                {
                    btEngine = new Seagull.BarTender.Print.Engine(true);
                }
                Seagull.BarTender.Print.LabelFormatDocument x = btEngine.Documents.Open(System.IO.Directory.GetCurrentDirectory() + "\\" + strTmplName);
                labelFormatDic.Add(strTmplName, x);
            }
            return labelFormatDic[strTmplName];
        }

        Seagull.BarTender.Print.LabelFormatDocument labelFormat = null;
        BarCodeMode BarMode = BarCodeMode.SerialCodeWithValidCode;
        bool AddValidCode { get { return BarMode == BarCodeMode.SerialCodeWithValidCode; } }
        DocumentMiniSNData Data = null;
        public int Current { get; set; } = 1;
        public int Total { get { return Data == null ? 0 : Data.SN_LIST.Count; } }
        public int HeightImage = 150;
        public int WidthImage = 600;

        public PrintPreviewWindow(string strTmplName, DocumentMiniSNData data, BarCodeMode mode)
        {
            InitializeComponent();
            Data = data; BarMode = mode;

            Seagull.BarTender.Print.Printers printers = new Seagull.BarTender.Print.Printers();
            foreach (Seagull.BarTender.Print.Printer printer in printers)
            {
                printer_comboBox.Items.Add(printer.PrinterName);
            }

            if (printers.Count > 0)
            {
                printer_comboBox.SelectedItem = printers.Default.PrinterName;
            }

            labelFormat = SetupLabelFormat(strTmplName);
            float Height_temp = labelFormat.PageSetup.LabelHeight * 6;
            float Width_temp = labelFormat.PageSetup.LabelWidth * 6;
            while (Width_temp < 1200 && Height_temp < 371)
            {
                Height_temp = Height_temp * 1.1F;
                Width_temp = Width_temp * 1.1F;
                if (Width_temp > 1200 || Height_temp > 371) break;
                HeightImage = (int)Height_temp;
                WidthImage = (int)Width_temp;
            }
            LoadPage(false);

            WaitWindow.CloseWindow(this);
        }
         
        private void PrePage_Click(object sender, RoutedEventArgs e)
        {
            if (Current > 1) Current = Current - 1;
            LoadPage(false);
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (Current < this.Total) Current = Current + 1;
            LoadPage(false);
        }

        void LoadPage(bool isPrint)
        {

            try
            {
                this.PageInfo.Content = string.Format("当前第{0}/{1}个", this.Current, this.Total);
                SN_ITEM dev1 = new SN_ITEM()
                {
                    SN = "000000000000",
                    POS = 0,
                    F_SerialValidCode = 0000,
                };
                SN_ITEM dev2 = new SN_ITEM()
                {
                    SN = "000000000000",
                    POS = 0,
                    F_SerialValidCode = 0000,
                };
                if (Current <= this.Total && Current >= 1)
                {
                    if (Data.SN_LIST.Count >= Current) dev1 = Data.SN_LIST[Current - 1];
                    if (Data.SN_LIST.Count >= Current + 1) dev2 = Data.SN_LIST[Current];
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(dev1.SN + (AddValidCode ? (";" + dev1.F_SerialValidCode) : ""));
                stringBuilder.Append("\t").Append(dev1.SN);
                stringBuilder.Append("\t").Append(dev1.POS_STR);
                stringBuilder.Append("\t").Append(dev2.SN + (AddValidCode ? (";" + dev2.F_SerialValidCode) : ""));
                stringBuilder.Append("\t").Append(dev2.SN);
                stringBuilder.Append("\t").Append(dev2.POS_STR);
                stringBuilder.AppendLine();
                File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\test.txt", stringBuilder.ToString());
                ((Seagull.BarTender.Print.Database.TextFile)labelFormat.DatabaseConnections["test"]).FileName = System.IO.Directory.GetCurrentDirectory() + "\\test.txt";

                if (labelFormat != null)
                {

                    labelFormat.SubStrings.SetSubString("QRCODE1", dev1.SN + (AddValidCode ? (";" + dev1.F_SerialValidCode) : ""));
                    labelFormat.SubStrings.SetSubString("TEXT1", dev1.SN);
                    labelFormat.SubStrings.SetSubString("POS1", dev1.POS_STR);

                    //Generate a thumbnail for it.
                    labelFormat.ExportImageToFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp", Seagull.BarTender.Print.ImageType.BMP, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit,
                        new Seagull.BarTender.Print.Resolution(WidthImage, HeightImage), Seagull.BarTender.Print.OverwriteOptions.Overwrite);

                    string path = System.IO.Directory.GetCurrentDirectory() + "\\test.bmp";
                    using (BinaryReader loader = new BinaryReader(File.Open(path, FileMode.Open)))
                    {
                        FileInfo fd = new FileInfo(path);
                        int Length = (int)fd.Length;
                        byte[] buf = new byte[Length];
                        buf = loader.ReadBytes((int)fd.Length);
                        loader.Dispose();
                        loader.Close();


                        //开始加载图像
                        BitmapImage bim = new BitmapImage();
                        bim.BeginInit();
                        bim.StreamSource = new MemoryStream(buf);
                        bim.EndInit();
                        PreviewImage.Source = bim;
                        GC.Collect(); //强制回收资源
                    }

                    //PreviewImage.Source = img;// new BitmapImage(new Uri(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp", UriKind.RelativeOrAbsolute)); ;

                }
                else
                {
                    MessageBox.Show("生成图片错误", "操作提示");
                }

                if (!isPrint) return;

                if (printer_comboBox.Text != "")
                {
                    labelFormat.PrintSetup.PrinterName = printer_comboBox.Text;
                    labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                }
                else
                {
                    MessageBox.Show("请先选择打印机", "操作提示");
                }

            }
            catch (Exception ex)
            {
                WaitWindow.CloseWindow(this);
                MessageBox.Show(ex.Message);
            }
            finally {

                WaitWindow.CloseWindow(this);

            }

        }

        private void PrintSinglePage_Click(object sender, RoutedEventArgs e)
        {
            LoadPage(true);
        }
         
        private void PrintAllPage_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                WaitWindow.ShowWindow("正在打印", "正在打印所有页......", this);

                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 1; i <= this.Total; i = i + 1)
                {
                    SN_ITEM dev1 = Data.SN_LIST[i - 1];
                    stringBuilder.Append(dev1.SN + (AddValidCode ? (";" + dev1.F_SerialValidCode) : ""));
                    stringBuilder.Append("\t").Append(dev1.SN);
                    stringBuilder.Append("\t").Append(dev1.POS_STR); 
                    stringBuilder.AppendLine();
                }
                File.WriteAllText(System.IO.Directory.GetCurrentDirectory() + "\\test.txt", stringBuilder.ToString());
                ((Seagull.BarTender.Print.Database.TextFile)labelFormat.DatabaseConnections["test"]).FileName = System.IO.Directory.GetCurrentDirectory() + "\\test.txt";

                if (printer_comboBox.Text != "")
                {
                    labelFormat.PrintSetup.PrinterName = printer_comboBox.Text;
                    labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                }
                else
                {
                    WaitWindow.CloseWindow(this);
                    MessageBox.Show("请先选择打印机", "操作提示");
                    return;
                }

            }
            catch (Exception ex)
            {
                WaitWindow.CloseWindow(this);
                MessageBox.Show(ex.Message);
            }
            finally {
                WaitWindow.CloseWindow(this);
            }
           
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
