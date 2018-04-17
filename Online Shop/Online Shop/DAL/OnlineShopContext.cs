using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Online_Shop.DAL
{
    public class OnlineShopContext: DbContext
    {
        public DbSet<Member> Member { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartStatus> CartStatus { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<MemberRole> MemberRole { get; set; }
        public DbSet<Roles> Role { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ShippingDetails> ShippingDetail { get; set; }

        public System.Data.Entity.DbSet<Online_Shop.Models.CategoryDetail> CategoryDetails { get; set; }
    }
}