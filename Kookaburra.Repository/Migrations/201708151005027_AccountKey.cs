namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OfflineMessages", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            DropIndex("dbo.OfflineMessages", new[] { "AccountId" });
            AddColumn("dbo.Visitors", "AccountId", c => c.Int(nullable: false));
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Visitors", "AccountId");
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            AddForeignKey("dbo.Visitors", "AccountId", "dbo.Accounts", "Id", cascadeDelete: false);
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: false);
            DropColumn("dbo.OfflineMessages", "AccountId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OfflineMessages", "AccountId", c => c.Int(nullable: false));
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropForeignKey("dbo.Visitors", "AccountId", "dbo.Accounts");
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            DropIndex("dbo.Visitors", new[] { "AccountId" });
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Int());
            DropColumn("dbo.Visitors", "AccountId");
            CreateIndex("dbo.OfflineMessages", "AccountId");
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id");
            AddForeignKey("dbo.OfflineMessages", "AccountId", "dbo.Accounts", "Id", cascadeDelete: false);
        }
    }
}
