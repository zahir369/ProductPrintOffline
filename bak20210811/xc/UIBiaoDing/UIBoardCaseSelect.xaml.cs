using DeviceDataMonitorWPF.UIBiaoDing;
using MKSS.Model.Laohua;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
    /// <summary>
    /// UIBoardCaseSelect.xaml 的交互逻辑
    /// </summary>
    public partial class UIBoardCaseSelect : Window
    {

        UIBoardCaseSelectEntity _model = null;
        public UIBoardCaseSelect()
        {

            InitializeComponent();
            

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _model = UISheBeiBiaoDingViewModel.Intance.BoardCaseSelectEntity;
            if (_model == null)
            {
                _model = UISheBeiBiaoDingViewModel.Intance.BoardCaseSelectEntity;
            }

            _model.Init();
            this.TxtBoardCaseList01.Items.Clear();
            List<BoardCase> list = _model.DBListBoardCase;
            for (int i = 0; i < list.Count; i++)
            {
                BoardCase item = list[i];
                bool has_free = false;
                if (_model.DBBoardCaseDictionary[item].Count <= 0) continue;
                foreach (Board _bb in _model.DBBoardCaseDictionary[item])
                {
                    if (_bb.EnumUseInFree != EnumUseInFree.InUse) has_free = true;
                }
                this.TxtBoardCaseList01.Items.Add(new ListBoxItem() { Tag = item, Content = item, IsEnabled = true, IsSelected = _model.SelectBoardCaseSelect[item] });
            }
        }


        private void TxtBoardCaseSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_model == null || this.TxtBatchCaseSelectFloor == null || TxtBoardCaseList01 == null) return;
            foreach (ListBoxItem item in this.TxtBoardCaseList01.Items)
            {
                if (item.Content is BoardCase)
                {
                    BoardCase c = item.Content as BoardCase;
                    if (item.IsSelected)
                    {
                        _model.SelectBoardCaseSelect[c] = true;
                        if (_model.SelectBoard.Where(w => w.Key.F_BoarCaseId == c.F_BoardCaseId).Count(w=>w.Value)==0) {
                            //如果整柜都没选择过任何一个
                            foreach (var b in _model.SelectBoard.Keys.ToList())
                            {
                                if (b.F_BoarCaseId == c.F_BoardCaseId) _model.SelectBoard[b] = true;//默认全选柜子所有层
                            }
                        }
                    }
                    else
                    {
                        _model.SelectBoardCaseSelect[c] = false;
                        foreach (var b in _model.SelectBoard.Keys.ToList())
                        {
                            if (b.F_BoarCaseId == c.F_BoardCaseId) _model.SelectBoard[b] = false;//取消柜子时取消所有层
                        }
                    }
                }
            }

            TxtBatchCaseSelectFloor.Items.Clear();
            foreach (BoardCase cas in _model.SelectBoardCaseSelect.Keys.ToArray())
            {
                if (_model.SelectBoardCaseSelect[cas]) {
                    foreach (Board board in _model.SelectBoard.Keys.Where(w => w.F_BoarCaseId == cas.F_BoardCaseId).ToArray())
                    {
                        this.TxtBatchCaseSelectFloor.Items.Add(new ListBoxItem() { Tag = board, Content = string.Format("{0}#{1}", cas.F_BoardCaseAddress, board.F_FloorNO), IsEnabled = true, IsSelected = _model.SelectBoard[board] });
                    }
                }
            }

        }

        private void TxtBatchCaseSelectFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_model == null) return;
            List<Board> sel = new List<Board>();
            foreach (ListBoxItem item in this.TxtBatchCaseSelectFloor.Items)
            {
                Board _Board = item.Tag as Board;
                if (item.IsSelected)
                {
                     _model.SelectBoard[_Board] = true;
                }
                else {
                    _model.SelectBoard[_Board] = false;
                }
            } 

        }


    }

}
