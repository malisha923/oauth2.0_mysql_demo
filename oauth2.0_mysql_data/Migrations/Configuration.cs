namespace oauth2._0_mysql_data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<oauth2._0_mysql_data.MysqlContext>
    {
        public Configuration()
        {
            // Ĭ������²����Զ�Ǩ�����ݿ�ṹ�������������������ã��꿴 DbMigrationsConfiguration<TContext>
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(oauth2._0_mysql_data.MysqlContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
