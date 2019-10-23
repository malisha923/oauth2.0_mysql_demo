using oauth2._0_mysql_data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace oauth2._0_mysql_demo.Controllers
{
    [RoutePrefix("api/user")]
    [Authorize]
    public class UserController : ApiController
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [Route(""), HttpGet, AllowAnonymous]
        public async Task<ApiResult<List<IdentityUser>>> Get()
        {
            using (var mysqldb = new MysqlContext())
            {
                var result = await mysqldb.Users.OrderBy(o => o.LastLogin).ToListAsync();
                return result.ToApiResult();
            }
        }

    }
}