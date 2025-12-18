using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.Util;
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
using MKSS.APP.UIBiaoDing;
using MKSS.Service.UIBiaoDing;

namespace MKSS.APP.LaoHua.UILaoHua
{
    /// <summary>
    /// UILaoHuaGuanChaAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaAdd : Window
    {

        UILaoHuaGuanChaAddModel _model = null;
        public SerialNoService SerialNoFactory = new SerialNoService();
        public UILaoHuaGuanChaAdd() {
            InitializeComponent();
            InitSelect();
        }
        public UILaoHuaGuanChaAdd(List<Board> selectBoards, SerialNoRuleEntity scrwEntity, List<SensorInfo> sensconfig)
        {
            InitializeComponent();
            InitSelect(selectBoards, scrwEntity, sensconfig);
        }
        List<SensorInfo> SensorInfos = null;
        bool InitSelecting = false;//初始化时禁用选择引发的联动
        void InitSelect(List<Board> selectBoards =null, SerialNoRuleEntity scrwEntity =null, List<SensorInfo> sensconfig=null) {

            try
            {


                InitSelecting = true;

                SerialNoConfig config = this.SerialNoFactory.SerialNoBtnConfig;
                this.BtnScrwd.Content = config.BtnTootip;
                this.BtnScrwd.ToolTip = config.BtnTootip;

                _model = base.DataContext as UILaoHuaGuanChaAddModel;

                _model.InitPage(this);
                _model.TxtBoardCaseSelectObject = new List<BoardCase>();

                if (selectBoards == null) selectBoards = new List<Board>();
                Dictionary<long, Board> dicSelectBoards = selectBoards.ToDictionary(w => w.F_BoardId, w => w);
                this.TxtBoardCaseList01.Items.Clear();
                List<BoardCase> list = _model.Data.DBListBoardCase;
                List<BoardCase> listSelectBoardCase = new List<BoardCase>();
                for (int i = 0; i < list.Count; i++)
                {
                    BoardCase item = list[i];
                    bool has_free = item.EnumUseInFree== EnumUseInFree.Free;
                    //foreach (Board _bb in _model.Data.DBBoardCaseDictionary[item])
                    //{
                    //    if (_bb.EnumUseInFree != EnumUseInFree.InUse) has_free = true;
                    //}
                    List<Board> bos = _model.Data.DBBoardCaseDictionary[item];
                    bool chd = bos.Count(w => dicSelectBoards.ContainsKey(w.F_BoardId)) > 0;
                    this.TxtBoardCaseList01.Items.Add(new ListBoxItem() { Tag = item, Content = item, IsEnabled = has_free, IsSelected = chd });
                    if (chd) {
                        listSelectBoardCase.Add(item);
                        _model.TxtBoardCaseSelectObject.Add(item);
                    } 
                }
                _model.SelectBatchEventByAddBoardCase(listSelectBoardCase);

                this.TxtBatchCaseSelectFloor.Items.Clear();
                foreach (var c in listSelectBoardCase)
                {
                    foreach (Board item in _model.Data.DBBoardCaseDictionary[c])
                    {
                        bool selxx = dicSelectBoards.ContainsKey(item.F_BoardId) ? true : false;
                        this.TxtBatchCaseSelectFloor.Items.Add(new ListBoxItem()
                        {
                            Tag = item,
                            Content = string.Format("{0}#{1}", c.F_BoardCaseAddress, item.F_FloorNO),
                            IsSelected = selxx,
                            IsEnabled = item.EnumUseInFree == EnumUseInFree.Free
                        });
                    }
                }
                _model.SelectBatchEventByAddBoard(selectBoards);

                if (scrwEntity != null)
                {
                    UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity = scrwEntity;
                    _model.TxtProjectTitle = string.Format("{0}[{1}]", _model.SelectRuleEntity.ProductFullName, _model.SelectRuleEntity.OrderNumber);
                    _model.TxtProductName = _model.SelectRuleEntity.ProductFullName;
                    _model.TxtProductCode = _model.SelectRuleEntity.ProductCode;
                    _model.TxtSerialPrefix = _model.SelectRuleEntity.OrderNumberPrefix;
                    _model.PageContext.TxtProductName.Text = _model.TxtProductName;
                    _model.PageContext.TxtSerialPrefix.Text = _model.TxtSerialPrefix;
                    _model.PageContext.TxtProjectTitle.Text = _model.TxtProjectTitle;
                }
                // 的串号设备时间信息，来自串号写入功能
                SensorInfos = sensconfig;




            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally {
                InitSelecting = false;
            }
            

        }



        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
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
            if (_model == null|| InitSelecting) return;
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
            if (_model == null || InitSelecting) return;
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

                if (_model.SelectRuleEntity == null) {
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
                _model.AddBatch(this.SensorInfos);


                UISheBeiBiaoDingViewModel.Intance.BtnDisConnect();
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
            if (_model.SelectRuleEntity != null) prefix = string.Format("{0}[{1}]", _model.SelectRuleEntity.ProductFullName, _model.SelectRuleEntity.OrderNumber);
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