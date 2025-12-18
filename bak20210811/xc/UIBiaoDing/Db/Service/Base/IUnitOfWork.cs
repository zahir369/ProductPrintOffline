using SqlSugar;

namespace MKSS.ICommon
{
    public interface IUnitOfWork
    {
        SqlSugarClient GetDbClient();

        void BeginTran();
        void CommitTran();
        void RollbackTran();
    }
}
