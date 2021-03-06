namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SettingRefund : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RefundSetting",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RefundSetting");
        }
    }
}
