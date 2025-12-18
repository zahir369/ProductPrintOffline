using MKSS.IRepository.Base;
using MKSS.IServices;
using MKSS.Model;
using MKSS.Repository.Base;
using MKSS.Services.BASE;

namespace MKSS.Services
{
    /// <summary>
    ///  BoardServices
    /// </summary>	
    public class BatchRelServices : BaseServices<BatchRel>, IBatchRelServices
    {
        IBaseRepository<BatchRel> _dal = new BaseRepository<BatchRel>();
    }

}