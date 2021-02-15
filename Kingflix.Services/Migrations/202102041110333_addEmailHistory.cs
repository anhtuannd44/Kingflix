namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addEmailHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailHistory", "TimeSend", c => c.DateTime(nullable: false));
            AddColumn("dbo.EmailHistory", "Title", c => c.String());
            AddColumn("dbo.EmailHistory", "Content", c => c.String());
            AddColumn("dbo.EmailHistory", "Status", c => c.Int(nullable: false));
            DropColumn("dbo.EmailHistory", "DateSend");
            DropColumn("dbo.EmailHistory", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailHistory", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.EmailHistory", "DateSend", c => c.DateTime(nullable: false));
            DropColumn("dbo.EmailHistory", "Status");
            DropColumn("dbo.EmailHistory", "Content");
            DropColumn("dbo.EmailHistory", "Title");
            DropColumn("dbo.EmailHistory", "TimeSend");
        }
    }
}
