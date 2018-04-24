namespace Online_Shop.Migrations
{
    using Online_Shop.DAL;
    using Online_Shop.Repository;
    using Online_Shop.Utility;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OnlineShopContext>
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
            context.CartStatus.AddOrUpdate(x => x.CartStatusId, 
                new CartStatus() { Cartstatus = "Added to cart" },
                new CartStatus() { Cartstatus = "Removed from cart" },
                new CartStatus() { Cartstatus = "Purchased the item" }
                );

            GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
            Member mem = new Member();
            mem.FirstName ="Admin";
            mem.LastName = "Admin";
            mem.EmailId = "Admin@gmail.com";
            mem.CreatedOn = DateTime.Now;
            mem.ModifiedOn = DateTime.Now;
            mem.Password = EncryptDecrypt.Encrypt("abc123", true);
            mem.IsActive = true;
            mem.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Member>().Add(mem);

            // Adding Member Role                 
            MemberRole mem_Role = new MemberRole();
            mem_Role.MemberId = mem.MemberId;
            mem_Role.RoleId = 1;
            _unitOfWork.GetRepositoryInstance<MemberRole>().Add(mem_Role);

           

        }
    }
}
