namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSMS : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SMSHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(nullable: false),
                        TimeSend = c.DateTime(nullable: false),
                        Title = c.String(),
                        Content = c.String(),
                        Status = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AppUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SMSHistory", "UserId", "dbo.AppUsers");
            DropIndex("dbo.SMSHistory", new[] { "UserId" });
            DropTable("dbo.SMSHistory");
        }
    }
}
