using MKSS.Model.Laohua;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MKSS.APP.UIBiaoDing
{
    public class UIBoardCaseSelectEntity
    {

        public Dictionary<Board, bool> SelectBoardInner { get; set; }
        public Dictionary<BoardCase, bool> SelectBoardCaseSelectInner { get; set; }
        public List<Board> DBListBoard { get; set; }
        public List<BoardCase> DBListBoardCase { get;  set; }
        public List<DataBus> DBListDataBus { get;  set; }
        public Dictionary<BoardCase, List<Board>> DBBoardCaseDictionary { get; set; }


        public Dictionary<Board, bool> SelectBoard { get { Init(); return SelectBoardInner; } }
        public Dictionary<BoardCase, bool> SelectBoardCaseSelect { get { Init(); return SelectBoardCaseSelectInner; } }
        
        public UIBoardCaseSelectEntity() {


        }

        public void Init() {

            if (DBListBoard == null)
            {
                try
                {
                    string str =  UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.db.Ado.Connection.ConnectionString;
                    DBListBoard = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.db.Ado.SqlQuery<Board>("select * from pd_board");
                    DBListBoardCase = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.db.Ado.SqlQuery<BoardCase>("select * from pd_boardcase");
                    DBListDataBus = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.db.Ado.SqlQuery<DataBus>("select * from pd_databus");
                    DBBoardCaseDictionary = new Dictionary<BoardCase, List<Board>>();
                    foreach (BoardCase b in DBListBoardCase)
                    {
                        DBBoardCaseDictionary.Add(b, DBListBoard.Where(w => w.F_BoarCaseId == b.F_BoardCaseId).OrderBy(w => w.F_BoardId).ToList());
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message,"数据库连接出错");
                }
                
            }

            if (SelectBoardInner == null)
            {
                SelectBoardInner = new Dictionary<Board, bool>();
                foreach (Board b in DBListBoard)
                {
                    SelectBoardInner.Add(b, false);
                }
            }

            if (SelectBoardCaseSelectInner == null)
            {
                SelectBoardCaseSelectInner = new Dictionary<BoardCase, bool>();
                foreach (BoardCase b in DBListBoardCase)
                {
                    SelectBoardCaseSelectInner.Add(b, false);
                }
            }

        }

        public Dictionary<long, DataBusExt> GetDataBusOf(List<Board> list )
        {
            Init();
            Dictionary<long, DataBusExt> reta = new Dictionary<long, DataBusExt>();
            Dictionary<DataBus, List<byte>> ret = new Dictionary<DataBus, List<byte>>();
            foreach (Board item in list)
            {

                List<long> ds = item.DataBusList;
                List<byte> bs = item.AddrList;
                if (ds.Count == 2 && bs.Count == 2) {
                    for (int i = 0; i < ds.Count ; i++)
                    {
                        if (!reta.ContainsKey(ds[i])) {
                            reta.Add(ds[i],new DataBusExt() { Address = new List<byte>() });
                        }
                        reta[ds[i]].Address.Add(bs[i]);
                        reta[ds[i]].DataBus = DBListDataBus.FirstOrDefault(w => w.F_DataBusId == ds[i]);
                        reta[ds[i]].BoardCase = DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseId == item.F_BoarCaseId);
                    }
                }

            }

            return reta;
        }

        public bool IsSelected(BoardCase v) {
            Init();
            foreach (var item in SelectBoard.Keys)
            {
                if(item.F_BoarCaseId==v.F_BoardCaseId && SelectBoard[item])
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsSelected(Board v)
        {
            Init();
            foreach (var item in SelectBoard.Keys)
            {
                if (item.F_BoardId == v.F_BoardId && SelectBoard[item])
                {
                    return true;
                }
            }
            return false;
        }

         
    }

    public class DataBusExt
    {
        public long Key { get { return DataBus.F_DataBusId; } }
        public DataBus DataBus { get; set; }
        public BoardCase BoardCase { get; set; }
        public List<byte> Address { get; set; }
    }

}
