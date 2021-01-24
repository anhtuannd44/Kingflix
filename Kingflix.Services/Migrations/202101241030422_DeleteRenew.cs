namespace Kingflix.Services.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteRenew : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Renew", "PaymentId", "dbo.Payment");
            DropForeignKey("dbo.Profile", "OrderId", "dbo.Renew");
            DropForeignKey("dbo.Renew", "UserId", "dbo.AppUsers");
            DropIndex("dbo.Renew", new[] { "UserId" });
            DropIndex("dbo.Renew", new[] { "PaymentId" });
            DropTable("dbo.Renew");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Renew",
                c => new
                    {
                        OrderId = c.String(nullable: false, maxLength: 128),
                        Month = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(),
                        DateConfirm = c.DateTime(),
                        Profile = c.Int(nullable: false),
                        AmountMoney = c.Double(nullable: false),
                        Status = c.Int(nullable: false),
                        File = c.Binary(),
                        ProfileId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        IsSendMail = c.Boolean(nullable: false),
                        VoucherId = c.String(),
                        PaymentId = c.Int(),
                        CancelNote = c.String(),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateIndex("dbo.Renew", "PaymentId");
            CreateIndex("dbo.Renew", "UserId");
            AddForeignKey("dbo.Renew", "UserId", "dbo.AppUsers", "Id");
            AddForeignKey("dbo.Profile", "OrderId", "dbo.Renew", "OrderId");
            AddForeignKey("dbo.Renew", "PaymentId", "dbo.Payment", "PaymentId");
        }
    }
}
