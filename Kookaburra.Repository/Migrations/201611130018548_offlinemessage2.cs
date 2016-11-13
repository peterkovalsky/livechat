namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offlinemessage2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfflineMessages", "AccountId", c => c.Int(nullable: false));
            CreateIndex("dbo.OfflineMessages", "AccountId");
            AddForeignKey("dbo.OfflineMessages", "AccountId", "dbo.Accounts", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfflineMessages", "AccountId", "dbo.Accounts");
            DropIndex("dbo.OfflineMessages", new[] { "AccountId" });
            DropColumn("dbo.OfflineMessages", "AccountId");
        }
    }
}
