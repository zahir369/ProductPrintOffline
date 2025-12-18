using MKSS.APP.WinNBOnlinePrinterTest;
using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest.Ass
{

    public abstract class TmpPrintAbs
    {
        public Engine btEngine = null;
        public PictureBox pictureBox1 = null;
        public ComboBox printer_comboBox = null;
        public LabelFormatDocument labelFormat = null;
        public int Height = 150;
        public int Width = 600;
        public abstract string TmpName { get; set; } 
        public abstract bool Match(string tmpName);
        public abstract void Print(pd_device dev, int sudata, bool printDirect);
        public abstract bool PrintBar(bool isPrint = false);
        public void InitLabel() {
            labelFormat = btEngine.Documents.Open(System.IO.Directory.GetCurrentDirectory() + "\\" + TmpName);

            //1366 768

            float Height_temp = labelFormat.PageSetup.LabelHeight * 6;
            float Width_temp = labelFormat.PageSetup.LabelWidth * 6;
            while (Width_temp < 1200 && Height_temp < 371) {
                Height_temp = Height_temp * 1.1F;
                Width_temp = Width_temp * 1.1F;
                if (Width_temp > 1200 || Height_temp > 371) break;
                Height = (int)Height_temp;
                Width = (int)Width_temp;
            } 
        }

        static TmpPrintOther TmpPrintOther = new TmpPrintOther();
        static List<TmpPrintAbs> TmpPrintNBCache = null; 
        public static void CreateAllInstancesOf<T>(Engine btEngine, PictureBox pictureBox1, ComboBox printer_comboBox)
        {
            List< TmpPrintAbs > em = typeof(TmpPrintAbs).Assembly.GetTypes() //获取当前类库下所有类型
                .Where(t => typeof(TmpPrintAbs).IsAssignableFrom(t)) //获取间接或直接继承t的所有类型
                .Where(t => !t.IsAbstract && t.IsClass && t.FullName!=typeof(TmpPrintOther).FullName) //获取 抽象类 排除接口继承
                .Select(t => (TmpPrintAbs)Activator.CreateInstance(t)).ToList(); //创造实例，并返回结果（项目需求，可删除）
            foreach (var item in em)
            {
                item.btEngine =  btEngine;
                item.pictureBox1 =  pictureBox1;
                item.printer_comboBox =  printer_comboBox;
                item.InitLabel();
            }
            TmpPrintNBCache = em;
        } 
        public static TmpPrintAbs Of(string tmpName) {
            List<TmpPrintAbs> list = TmpPrintNBCache;
            TmpPrintAbs find = list.FirstOrDefault(w => w.TmpName == tmpName);
            if (find == null) {
                TmpPrintOther.TmpName = tmpName;
                return TmpPrintOther;
            }
            return find;
        }
        public static List<TmpPrintAbs> All()
        { 
            return TmpPrintNBCache;
        }

        public override string ToString()
        {
            return TmpName;
        }

    }
}
