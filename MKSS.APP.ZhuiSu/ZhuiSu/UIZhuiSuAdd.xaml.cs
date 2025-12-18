using MKSS.APP.ZhuiSu;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MKSS.Service.ZhuiSu;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuAdd : Window
    {

        UIZhuiSuAddModel _model = null;
        public UIZhuiSuAdd()
        {

            InitializeComponent();
            _model = base.DataContext as UIZhuiSuAddModel;

            _model.InitPage(this);

            this.TxtProductList.Items.Clear();
            foreach (ProductItem item in Products.Instance.List)
            {
                this.TxtProductList.Items.Add(item); 
            }

            this.TxtBoardCaseList01.Items.Clear(); 
            List<BoardCase> list = _model.Data.DBListBoardCase;
            for (int i = 0; i < list.Count; i++)
            {
                BoardCase item = list[i];
                bool has_free = false;
                foreach (Board _bb in _model.Data.DBBoardCaseDictionary[item])
                {
                    if (_bb.EnumUseInFree != EnumUseInFree.InUse) has_free = true;
                }
                this.TxtBoardCaseList01.Items.Add(new ListBoxItem() { Tag = item, Content = item, IsEnabled = has_free });
            }
            if (list.Count > 0) this.TxtBoardCaseList01.SelectedItem = list[0];

        }
         
        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
        }

        public static void InputAddress(KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else if ((e.Key == Key.OemComma || e.Key == Key.OemMinus || e.Key == Key.Subtract || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else
            {

                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字，“,”或“-”");
                return;
            }
        }

        public static void InputNumber(KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else
            {

                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字");
                return;

            }
        }

        private void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model == null) return;
            ProductItem c = this.TxtProductList.SelectedItem as ProductItem;
            _model.ProductConfig = c;
        }
         

        private void TxtBatchCaseSelectFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model == null) return;
            Board _Sample = null;
            List<Board> sel = new List<Board>();
            foreach (ListBoxItem item in this.TxtBatchCaseSelectFloor.Items)
            {
                if (item.Tag is Board)
                {
                    _Sample = item.Tag as Board;
                    if (item.IsSelected)
                        sel.Add(item.Tag as Board);
                }
            }
            if (_Sample == null) return;
            _model.SelectBatchEventByAddBoard(sel);

            
        }


        private void TxtBoardCaseSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model == null) return;
            _model.TxtBoardCaseSelectObject = new List<BoardCase>();
            List<BoardCase> sel = _model.TxtBoardCaseSelectObject;
            if (TxtBoardCaseList01 != null)
            {
                foreach (ListBoxItem item in this.TxtBoardCaseList01.Items)
                {
                    if (item.Content is BoardCase)
                    {
                        if (item.IsSelected)
                            sel.Add(item.Content as BoardCase);
                    }
                }
            }
             

            StringBuilder s = new StringBuilder();
            foreach (BoardCase item in sel)
            {
                s.Append(string.Format("{0},", item.F_BoardCaseAddress.ToString("00")));
            }
             
            _model.SelectBatchEventByAddBoardCase(sel);

            this.TxtBatchCaseSelectFloor.Items.Clear();
            foreach (var c in sel)
            {
                foreach (Board item in _model.Data.DBBoardCaseDictionary[c])
                {
                    bool selxx = _model.SelectBatchBoard.ContainsKey(item)
                        ? _model.SelectBatchBoard[item] : false;
                    this.TxtBatchCaseSelectFloor.Items.Add(new ListBoxItem()
                    {
                        Tag = item,
                        Content = string.Format("{0}{1}",c.F_BoardCaseName, item.F_FloorNO),
                        IsSelected = selxx,
                        IsEnabled = item.EnumUseInFree == EnumUseInFree.Free
                    });
                }
            }

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Cursor = Cursors.AppStarting;
                ProductItem _ProductConfig = this.TxtProductList.SelectedItem as ProductItem;
                if (_ProductConfig == null)
                {
                    MessageBox.Show("请选择检测产品型号！");
                    return;
                }

                this._model.TxtProductCode = _ProductConfig == null ? "" : _ProductConfig.Code + "";
                this._model.TxtProductName = _ProductConfig == null ? "" : _ProductConfig.Name + "";
                this._model.TxtProjectTitle = this.TxtProjectTitle.Text;
                this._model.TxtTimeTotal = double.Parse(this.TxtTimeTotal.Text);
                this._model.TxtReadSpeed = double.Parse(this.TxtReadSpeed.Text);
                this._model.TxtTestTimePoints = this.TxtTestTimePoints.Text;


                if (this._model.TxtTimeTotal <= 0)
                {
                    MessageBox.Show("请输入检测时间！");
                    return;
                }
                else {
                    double dd = double.Parse(this.TxtTimeTotal.Text);
                    if (dd > 30 * 24 * 60) {
                        MessageBox.Show("检测时间最多30天！");
                    }
                }
                if (this._model.TxtReadSpeed <= 0)
                {
                    MessageBox.Show("请输入读取间隔！");
                    return;
                }

                if (string.IsNullOrEmpty(this._model.TxtTestTimePoints))
                {
                    MessageBox.Show("请输入采样时间点！");
                    return;
                }
                else
                {
                    List<int> ret = new List<int>();
                    string[] arr = (TxtTestTimePoints.Text + "").Split(",");
                    foreach (var item in arr)
                    {
                        if (string.IsNullOrEmpty(item)) continue;
                        ret.Add(int.Parse(item));
                    }
                    if (ret.Count == 0)
                    {
                        MessageBox.Show("请输入有效采样时间点，例如：“4,30,50,70,80,150,180,250,280,300”！");
                        return;
                    }
                }

                if (this._model.SelectBatchBoard.Count(w => w.Value) <= 0)
                {
                    MessageBox.Show("选择柜子后，请选择托盘！");
                    return;
                }

                if (string.IsNullOrEmpty(this.TxtProjectTitle.Text)) TxtProjectTitle_MouseDoubleClick(null, null);
                _model.AddBatch();
                this.Cursor = Cursors.Arrow;
                MessageBox.Show("添加成功");
                this.DialogResult = true;

            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }
            
        }

        private void TxtProjectTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ProductItem _ProductConfig = this.TxtProductList.SelectedItem as ProductItem;
            if (_ProductConfig!=null)
            {
                string case_list = "";
                foreach (var item in _model.TxtBoardCaseSelectObject)
                {
                    if (!string.IsNullOrEmpty(case_list)) case_list = case_list + ",";
                    case_list = case_list + item.F_BoardCaseName;
                }
                this.TxtProjectTitle.Text =
                    string.Format("{0}_{2}{3}_{1}M_{4}",
                    _ProductConfig == null ? "" : _ProductConfig.Name + "",
                    this.TxtTimeTotal.Text,
                    case_list,
                    this._model.SelectBatchBoard.Count(w => w.Value),
                    DateTime.Now.ToString("MMddHHMM")
                    ); ;

            }
        }

        private void TxtTimeTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                double dd = double.Parse(this.TxtTimeTotal.Text);
                if (dd > 2 * 60)
                {
                    this.TxtReadSpeed.Text = "5";
                }
                if (dd > 24 * 60)
                {
                    this.TxtReadSpeed.Text = "30";
                }
                if (dd > 3 * 24 * 60)
                {
                    this.TxtReadSpeed.Text = "300";
                }
                // 300 “4,30,50,70,80,150,180,250,280,300”
                double[] pers = new double[] {
                D(4,300),D(30,300),D(50,300),D(70,300),D(80,300),D(150,300),D(180,300),D(250,300),D(280,300),D(290,300),
            };
                string c = string.Join(",", pers.Select(w => D10(w, dd)).ToArray());
                if (this.TxtTestTimePoints != null) this.TxtTestTimePoints.Text = c;
                if (this.TxtTimeTotalDesc != null) TxtTimeTotalDesc.Text = string.Format("检测总时间：{0}", ToMyFormat(TimeSpan.FromMinutes(dd)));

            }
            catch (Exception ex)
            {

            }
            
        }
        public static string ToMyFormat(TimeSpan ts)
        {
            if (ts.TotalMinutes < 5) {
                return ts.ToString("''m'′'s'″'");
            }
            if (ts.TotalHours < 1)
            {
                return ts.ToString("''m'′'s'″'");
            }
            string format = ts.Days >= 1 ? "''d'd'h'h'm'′'s'″'" : "''h'h'm'′'s'″'";
            return ts.ToString(format);
        }
        double D(int a, int b) {
            return (double)a / (double)b;
        }

        /// <summary>
        ///  取 整10
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        int D10(double per, double total)
        {

            int i = (int)(per * total * 60);
            if (i < 30) return i;
            return i- i%10 + 10;
        }

    }

}