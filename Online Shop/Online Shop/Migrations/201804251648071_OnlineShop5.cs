namespace Online_Shop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OnlineShop5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ShippingDetails", "PaymentType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShippingDetails", "PaymentType", c => c.String());
        }
    }
}
