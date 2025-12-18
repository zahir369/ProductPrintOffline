using MKSS.APP.WinNBOnlinePrinterTest;
using Seagull.BarTender.Print;
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest.Ass
{
    public class TmpPrintJinKaCHCO : TmpPrintAbs
    {

        pd_device D1; 
        public override string TmpName { get; set; } = "金卡标签双气+NB.btw";
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
                        if (labelFormat.SubStrings.Count(w => w.Name == "TEXT2") > 0)
                            //正常版本
                            labelFormat.SubStrings.SetSubString("TEXT2", dev1.F_SerialNO);

                        //临时版本舍弃验证码
                        //labelFormat.SubStrings.SetSubString("TEXT2", dev1.F_SerialNO.Split(';')[0]);
                    }
                    else
                    {
                        if (labelFormat.SubStrings.Count(w => w.Name == "TEXT2") > 0)
                            labelFormat.SubStrings.SetSubString("TEXT2", "12345678");
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

    }
}
