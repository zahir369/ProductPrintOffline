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
    ///  BoardCaseServices
    /// </summary>	
    public class BoardCaseServices : BaseServices<BoardCase>, IBoardCaseServices
    {
        IBaseRepository<BoardCase> _dal = new BaseRepository<BoardCase>();
    }
}