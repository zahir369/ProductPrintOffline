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
    ///  DataBusServices
    /// </summary>	
    public class DataBusServices : BaseServices<DataBus>, IDataBusServices
    {
        IBaseRepository<DataBus> _dal = new BaseRepository<DataBus>();
    }
}