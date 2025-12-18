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
    ///  SensorDataServices
    /// </summary>	
    public class SensorGroupDataServices : BaseServices<SensorGroupData>, ISensorGroupDataServices
    {
        IBaseRepository<SensorGroupData> _dal = new BaseRepository<SensorGroupData>();
    }
}