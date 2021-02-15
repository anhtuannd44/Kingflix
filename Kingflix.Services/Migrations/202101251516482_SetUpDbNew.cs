namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetUpDbNew : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OrderDetails", "Status");
            DropColumn("dbo.Order", "stat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Order", "stat", c => c.String());
            AddColumn("dbo.OrderDetails", "Status", c => c.Int(nullable: false));
        }
    }
}
