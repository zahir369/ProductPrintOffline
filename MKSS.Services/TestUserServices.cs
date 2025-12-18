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
    /// RoleServices
    /// </summary>	
    public class TestUserServices : BaseServices<TestUser>, ITestUserServices
    {
        IBaseRepository<TestUser> _dal = new BaseRepository<TestUser>();
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="testid"></param>
        /// <returns></returns>
        public async Task<TestUser> GetUser(string testid)
        {
            try
            {
                var result = await _dal.Query(x => x.TestId == testid);
                var res= result.FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());                
            }

        }
    }
}
