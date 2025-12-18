using MKSS.IRepository.Base;
using MKSS.IServices;
using MKSS.Model;
using MKSS.Repository.Base;
using MKSS.Services.BASE;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MKSS.Services
{
    /// <summary>
    ///  DataItemDetailServices
    /// </summary>	
    public class DataItemDetailServices : BaseServices<DataItemDetail>, IDataItemDetailServices
    {
        IBaseRepository<DataItemDetail> _dal = new BaseRepository<DataItemDetail>();
    }
}