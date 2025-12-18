using MKSS.APP.WinNBOnlinePrinterTest;
using Seagull.BarTender.Print;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest.Ass
{
    public class TmpPrintNBIMEI : TmpPrintAbs
    {

        pd_device D1;
        pd_device D2;
        public override string TmpName { get; set; } = "NB带验证码IMEI标签.btw";
        public override bool Match(string tmpName)
        {
            return tmpName == TmpName;
        }
        public override void Print(pd_device dev, int sudata, bool printDirect)
        {
            if (sudata % 2 == 0)
            {

                D1 = dev;
            }
            if (sudata % 2 == 1)
            {
                D2 = dev;
            }


            if (D1 != null || D2 != null)
            {
                bool print = false;
                if (printDirect && (D1 != null && D2 != null)) print = true;
                PrintBar(print);

            }
        }


        public override bool PrintBar(bool isPrint = false)
        {

            {
                pd_device dev1 = D1;
                pd_device dev2 = D2;

                try
                {

                    if (dev1 != null && !string.IsNullOrEmpty(dev1.F_IMEI))
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE1", dev1.F_IMEI);
                        labelFormat.SubStrings.SetSubString("TEXT1", dev1.F_SerialNO);
                    }
                    else
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE1", "000000000000000");
                        labelFormat.SubStrings.SetSubString("TEXT1", "000000000000");
                    }

                    if (dev2 != null && !string.IsNullOrEmpty(dev2.F_IMEI))
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE2", dev2.F_IMEI);
                        labelFormat.SubStrings.SetSubString("TEXT2", dev2.F_SerialNO);
                    }
                    else
                    {
                        labelFormat.SubStrings.SetSubString("QRCODE2", "000000000000000");
                        labelFormat.SubStrings.SetSubString("TEXT2", "000000000000");
                    }


                }

                catch (Exception ex)
                {
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
                    MessageBox.Show("请先选择打印机", "操作提示");
                }
                D1 = null;
                D2 = null;
                return true;
            }
        }

    }
}
