namespace Online_Shop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnlineShop2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CategoryDetails",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 100),
                        IsActive = c.Boolean(),
                        IsDelete = c.Boolean(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
            CreateIndex("dbo.Carts", "MemberId");
            AddForeignKey("dbo.Carts", "MemberId", "dbo.Members", "MemberId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Carts", "MemberId", "dbo.Members");
            DropIndex("dbo.Carts", new[] { "MemberId" });
            DropTable("dbo.CategoryDetails");
        }
    }
}
