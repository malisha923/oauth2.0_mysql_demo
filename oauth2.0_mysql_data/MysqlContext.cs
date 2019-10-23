using MySql.Data.Entity;
using System.Configuration;
using System.Data.Entity;

namespace oauth2._0_mysql_data
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MysqlContext : DbContext
    {
        public MysqlContext() : base(ConfigurationManager.ConnectionStrings["mysqldb"].ConnectionString)
        {
            Database.SetInitializer(new MySqlInitializer());
        }


        public static MysqlContext Create()
        {
            return new MysqlContext();
        }


        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<IdentityUser> Users { get; set; }
    }
}
