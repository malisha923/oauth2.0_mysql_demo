using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace oauth2._0_mysql_data
{
    public class MySqlInitializer : IDatabaseInitializer<MysqlContext>
    {
        public void InitializeDatabase(MysqlContext context)
        {
            if (!context.Database.Exists())
            {
                // 若数据库不存在，先创建数据库
                context.Database.Create();
            }
            else
            {
                // 检测数据库中是否存在 __migrationhistory(数据表迁移记录)表
                var migrationHistoryTableExists = ((IObjectContextAdapter)context).ObjectContext.ExecuteStoreQuery<int>(
                    string.Format("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = '{0}' AND table_name = '__MigrationHistory'", "oauth_test_mysql")
                );

                // 若不存在 __migrationhistory(数据表迁移记录)表，先创建该表
                if (migrationHistoryTableExists.FirstOrDefault() == 0)
                {
                    context.Database.Delete();
                    context.Database.Create();
                }
            }

            // 初始化管理员账号
            var ucounter = context.Users.CountAsync().GetAwaiter().GetResult();
            if (ucounter == 0)
            {
                var u = new IdentityUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "malisha923@qq.com",
                    Email = "malisha923@qq.com",
                    PasswordHash = (new PasswordHasher()).HashPassword("123456"),
                    Roles = "SystemAdmin",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    LockoutEndDateUtc = DateTime.Parse("1900-01-01 00:00:00"),
                    LastLogin = DateTime.Parse("1900-01-01 00:00:00")
                };

                context.Users.Add(u);
                context.SaveChanges();
            }

        }
    }
}
