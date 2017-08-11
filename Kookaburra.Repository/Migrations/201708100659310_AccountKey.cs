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
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Int(nullable: false));
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: false);           

            AddColumn("dbo.Visitors", "AccountId", c => c.Int(nullable: false));            
            CreateIndex("dbo.Visitors", "AccountId");            
            AddForeignKey("dbo.Visitors", "AccountId", "dbo.Accounts", "Id", cascadeDelete: false);                        
        }
        
        public override void Down()
        {                        
            DropForeignKey("dbo.Visitors", "AccountId", "dbo.Accounts");            
            DropIndex("dbo.Visitors", new[] { "AccountId" });           
            DropColumn("dbo.Visitors", "AccountId");

           
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Int());
            CreateIndex("dbo.OfflineMessages", "AccountId");
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id");
            AddForeignKey("dbo.OfflineMessages", "AccountId", "dbo.Accounts", "Id", cascadeDelete: false);
        }
    }
}
