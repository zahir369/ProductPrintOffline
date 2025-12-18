using MKSS.APP.WinNBOnlinePrinterTest;
using Seagull.BarTender.Print;
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest.Ass
{
    public class TmpPrintJinKaXo : TmpPrintAbs
    {

        pd_device D1;
        string str = "004030122040500000000001";
        public override string TmpName { get; set; } = "金卡标签新奥外壳.btw";
        public override bool Match(string tmpName)
        {
            return tmpName == TmpName;
        }
        public override void Print(pd_device dev, int sudata, bool printDirect)
        {
            D1 = dev;
            if (D1 != null )
            {
                bool print = false;
                if (printDirect && (D1 != null)) print = true;
                PrintBar(print);

            }
        }


        public override bool PrintBar(bool isPrint = false)
        {

            {
                pd_device dev1 = D1; 
                try
                {

                    if (dev1 != null)
                    {

                        string srrF_SerialNO = TmpPrintJinKaXo.ToSerialNo24(dev1.F_SerialNO);
                        if (labelFormat.SubStrings.Count(w => w.Name == "QRCODE1") > 0)
                            labelFormat.SubStrings.SetSubString("QRCODE1", srrF_SerialNO);
                        if (labelFormat.SubStrings.Count(w => w.Name == "TEXT1") > 0)
                            labelFormat.SubStrings.SetSubString("TEXT1", srrF_SerialNO);
                        if (labelFormat.SubStrings.Count(w => w.Name == "DATE1") > 0)
 
                        labelFormat.SubStrings.SetSubString("DATE1", dev1.F_SerialNO.Substring(7, 2) + "年" +
                        dev1.F_SerialNO.Substring(9, 2) + "月");
                   
                    }
                    else
                    {

                        if (labelFormat.SubStrings.Count(w => w.Name == "QRCODE1") > 0)
                            labelFormat.SubStrings.SetSubString("QRCODE1", "000000000000000000000000");
                        if (labelFormat.SubStrings.Count(w => w.Name == "TEXT1") > 0)
                            labelFormat.SubStrings.SetSubString("TEXT1", "000000000000000000000000");
                        if (labelFormat.SubStrings.Count(w => w.Name == "DATE1") > 0)
                            labelFormat.SubStrings.SetSubString("DATE1", "0000年00月");

                    }

                }

                catch (Exception ex)
                {
                    SplashForm.Instance.Close();
                    MessageBox.Show("修改内容出错 " + ex.Message, "操作提示");
                }

                if (labelFormat != null)
                {

                    //Generate a thumbnail for it.
                    labelFormat.ExportImageToFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp", ImageType.BMP, Seagull.BarTender.Print.ColorDepth.ColorDepth24bit,
                        new Resolution(Width, Height), OverwriteOptions.Overwrite);

                    System.Drawing.Image image = System.Drawing.Image.FromFile(System.IO.Directory.GetCurrentDirectory() + "\\test.bmp");
                    Bitmap NmpImage = new Bitmap(image);
                    //pictureBox1.Width = Width;
                    //pictureBox1.Height = Height;
                    pictureBox1.Image = NmpImage;
                    image.Dispose();

                }
                else
                {
                    SplashForm.Instance.Close();
                    MessageBox.Show("生成图片错误", "操作提示");
                }

                if (!isPrint) return true;

                if (printer_comboBox.Text != "")
                {
                    labelFormat.PrintSetup.PrinterName = printer_comboBox.Text;
                    labelFormat.Print("BarPrint" + DateTime.Now, 3 * 1000);
                }
                else
                {
                    SplashForm.Instance.Close();
                    MessageBox.Show("请先选择打印机", "操作提示");
                }
                D1 = null; 
                return true;
            }
        }

        /// <summary>
        ///  转24位显示
        ///  （004）03012204（0500000）000001
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSerialNo24(string str) {
            string ret = "";
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length == 24) return str;
            if (str.Length != 14) return str;
            string a1 = str.Substring(1, 7);//去掉第一位防 13 位编号位
            string a2 = str.Substring(8, 6);
            return string.Format("{0}0{1}{2}{3}", "004", a1, "0500000", a2 );
        }
        /// <summary>
        ///  恢复14位显示
        ///  （004）03012204（0500000）000001
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FromSerialNo24(string str)
        {
            string ret = "";//。。。
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length != 24) return str;
            string a1 = str.Substring(4, 7);
            string a2 = str.Substring(18, 6);
            return string.Format("1{0}{1}",  a1,   a2);
        }
    }
}
