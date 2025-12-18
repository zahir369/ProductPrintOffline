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
    ///  DataItemServices
    /// </summary>	
    public class DataItemServices : BaseServices<DataItem>, IDataItemServices
    {
        IBaseRepository<DataItem> _dal = new BaseRepository<DataItem>();
    }
}