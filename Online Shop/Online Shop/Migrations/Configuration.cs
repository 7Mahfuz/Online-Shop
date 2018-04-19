namespace Online_Shop.Migrations
{
    using Online_Shop.DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Online_Shop.DAL.OnlineShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        //OnlineShopContext context = new OnlineShopContext();
        protected override void Seed(OnlineShopContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Role.AddOrUpdate(x => x.RoleId,
            new Roles() { RoleName="Admin" },
            new Roles() { RoleName="User"}
                        );


        }
    }
}
