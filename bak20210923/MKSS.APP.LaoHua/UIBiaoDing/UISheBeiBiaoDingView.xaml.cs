using MKSS.APP.UIBiaoDing.Config;
using MKSS.APP.UIBiaoDing.Util;
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

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDing.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingView : UserControl
    {

        UISheBeiBiaoDingViewModel _model = null;
        public UISheBeiBiaoDingView()
        {

            InitializeComponent();

            _model = UISheBeiBiaoDingViewModel.Intance;
            this.TxtBoardCaseList01.Items.Clear();
            List<BoardCase> list = UISheBeiBiaoDingViewModel.Intance.DBListBoardCase;
            for (int i = 0; i < list.Count; i++)
            {
                BoardCase item = list[i];
                bool has_free = false;
                if (UISheBeiBiaoDingViewModel.Intance.DBBoardCaseDictionary[item].Count <= 0) continue;
                foreach (Board _bb in UISheBeiBiaoDingViewModel.Intance.DBBoardCaseDictionary[item])
                {
                    if (_bb.EnumUseInFree != EnumUseInFree.InUse) has_free = true;
                }
                this.TxtBoardCaseList01.Items.Add(new ListBoxItem() { Tag = item, Content = item, IsEnabled = true });
            }
            if (list.Count > 0) this.TxtBoardCaseList01.SelectedItem = list[0];

            UISheBeiBiaoDingViewModel.Intance.InitPage(this);

            MKSS.APP.LaoHua.Config.ProductConfig c = MKSS.APP.LaoHua.Config.LaoHuaConfgig.Instance.Product.FirstOrDefault();
            if (c == null) return;

            this.TxtZreo.Text = c.Zero.ToString();
            this.TxtSpan.Text = c.Span.ToString();
            //this.TxtTimeInterval.Text = c.TimeInterval.ToString();
            this.TxtValueAdd.Text = c.ValueAdd.ToString();
            this.TxtValueBase.Text = c.ValueBase.ToString();
            this.TxtValueMinus.Text = c.ValueMinus.ToString();

            this.TxtVoltageValueAdd.Text = c.VoltageValueAdd.ToString();
            this.TxtVoltageValueBase.Text = c.VoltageValueBase.ToString();
            this.TxtVoltageValueMinus.Text = c.VoltageValueMinus.ToString();

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
            _model.SelectAddBoard(sel);


        }


        private void TxtBoardCaseSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model == null || this.TxtBatchCaseSelectFloor==null) return;
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

            _model.SelectAddBoardCase(sel);

            this.TxtBatchCaseSelectFloor.Items.Clear();
            foreach (var c in sel)
            {
                foreach (Board item in _model.DBBoardCaseDictionary[c])
                {
                    bool selxx = _model.SelectBoard.ContainsKey(item)
                        ? _model.SelectBoard[item] : false;
                    this.TxtBatchCaseSelectFloor.Items.Add(new ListBoxItem()
                    {
                        Tag = item,
                        Content = string.Format("{0}#{1}", c.F_BoardCaseAddress, item.F_FloorNO),
                        IsSelected = selxx,
                        IsEnabled = true
                    });
                }
            }

        }

        private void TxtSensorGrougAddress_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            InputAddress(e);
        }

        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
        }

        private void TxtVoltageValueBase_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
            
        }


        private void TxtValueBase_InputNumber(object sender, KeyEventArgs e)
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
         

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            UISheBeiBiaoDingViewModel.Intance.Closed();
        }
         

        private void TxtValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtValueBase.Text == "0" || TxtValueBase.Text == "")
            {
                //读取平均值
                List<SensorGroupDataModel> list = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
                List<Util.SensormData> listSensor = new List<Util.SensormData>();
                foreach (SensorGroupDataModel d in list)
                {
                    listSensor.AddRange(d.ProductTable.Where(w => !w.IsEmpData));
                }
                int min = listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Min(w => w.V07.Value);
                int max = listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Max(w => w.V07.Value);
                int avg = (int)listSensor.Where(w => w.V07 != null && w.V07.Value > 0 && w.V07.Value != 170).Average(w => w.V07.Value);
                //读取平均值
                this.TxtValueBase.Text = avg.ToString();
                this.TxtValueAdd.Text = (max - avg).ToString();
                this.TxtValueMinus.Text = (avg - min).ToString();
                //UISheBeiBiaoDingModel.Intance.BtnJudge();
            }
        }

        private void TxtVoltageValueBase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtVoltageValueBase.Text == "0" || TxtVoltageValueBase.Text == "")
            {
                List<SensorGroupDataModel> list = UISheBeiBiaoDingViewModel.Intance.ProductModelGroupTable.Values.ToList();
                List<Util.SensormData> listSensor = new List<Util.SensormData>();
                foreach (SensorGroupDataModel d in list)
                {
                    listSensor.AddRange(d.ProductTable.Where(w => !w.IsEmpData));
                }
                int min = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Min(w => w.V01.Value);
                int max = listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Max(w => w.V01.Value);
                int avg = (int)listSensor.Where(w => w.V01 != null && w.V01.Value > 0 && w.V01.Value != 170).Average(w => w.V01.Value);
                //读取平均值
                this.TxtVoltageValueBase.Text = avg.ToString();
                this.TxtVoltageValueAdd.Text = (max - avg).ToString();
                this.TxtVoltageValueMinus.Text = (avg - min).ToString();
                //UISheBeiBiaoDingModel.Intance.BtnJudge();
            }
        }
    }
}
