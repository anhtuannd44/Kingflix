namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingRefund2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RefundSetting", "Value", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RefundSetting", "Value", c => c.Double(nullable: false));
        }
    }
}
