using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.Util;
using MKSS.APP.LaoHua.Config;
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

namespace MKSS.APP.LaoHua.UILaoHua
{
    /// <summary>
    /// UILaoHuaGuanChaAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaAdd : Window
    {

        UILaoHuaGuanChaAddModel _model = null;
        public UILaoHuaGuanChaAdd()
        {

            InitializeComponent();
            _model = base.DataContext as UILaoHuaGuanChaAddModel;

            _model.InitPage(this);
             

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
                        Content = string.Format("{0}#{1}",c.F_BoardCaseAddress, item.F_FloorNO),
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

                if (_model.SelectScrwEntity == null) {
                    MessageBox.Show("请选择老化所属任务单！");
                    return;
                }
                 
                this._model.TxtTimeTotal = double.Parse(this.TxtTimeTotal.Text);
                if(this._model.TxtTimeTotal<=0)
                {
                    MessageBox.Show("请输入老化时间！");
                    return;
                }

                if (this._model.SelectBatchBoard.Count(w => w.Value) <= 0)
                {
                    MessageBox.Show("选择柜子后，请选择托盘！");
                    return;
                }
                this._model.TxtProjectTitle = this.TxtProjectTitle.Text;

                TxtProjectTitle_MouseDoubleClick(null, null);
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
            string case_list = "";
            foreach (var item in _model.TxtBoardCaseSelectObject)
            {
                if (!string.IsNullOrEmpty(case_list)) case_list = case_list + ",";
                case_list = case_list + item.F_BoardCaseAddress;
            }
            string prefix = "";
            if (_model.SelectScrwEntity != null) prefix = string.Format("{0}[{1}]", _model.SelectScrwEntity.ProductFullName, _model.SelectScrwEntity.OrderNumber);
            this.TxtProjectTitle.Text =
                string.Format("{0}_{2}#{3}_{1}H_{4}",
                prefix,
                this.TxtTimeTotal.Text,
                case_list,
                this._model.SelectBatchBoard.Count(w => w.Value),
                DateTime.Now.ToString("MMddHHMM")
                ); ;

        }
    }

}