namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSMSToDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SMSTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SMSTemplate");
        }
    }
}
