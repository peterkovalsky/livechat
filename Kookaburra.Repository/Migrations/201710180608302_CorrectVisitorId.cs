namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectVisitorId : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Conversations", "Visitor_Id", "dbo.Visitors");
            DropForeignKey("dbo.OfflineMessages", "Visitor_Id", "dbo.Visitors");
            DropIndex("dbo.Conversations", new[] { "Visitor_Id" });
            DropIndex("dbo.OfflineMessages", new[] { "Visitor_Id" });
            DropColumn("dbo.Conversations", "VisitorId");
            DropColumn("dbo.OfflineMessages", "VisitorId");
            RenameColumn(table: "dbo.Conversations", name: "Visitor_Id", newName: "VisitorId");
            RenameColumn(table: "dbo.OfflineMessages", name: "Visitor_Id", newName: "VisitorId");
            AlterColumn("dbo.Conversations", "VisitorId", c => c.Long(nullable: false));
            AlterColumn("dbo.Conversations", "VisitorId", c => c.Long(nullable: false));
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Long(nullable: false));
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Long(nullable: false));
            CreateIndex("dbo.Conversations", "VisitorId");
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            AddForeignKey("dbo.Conversations", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: true);
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropForeignKey("dbo.Conversations", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            DropIndex("dbo.Conversations", new[] { "VisitorId" });
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Long());
            AlterColumn("dbo.OfflineMessages", "VisitorId", c => c.Int(nullable: false));
            AlterColumn("dbo.Conversations", "VisitorId", c => c.Long());
            AlterColumn("dbo.Conversations", "VisitorId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.OfflineMessages", name: "VisitorId", newName: "Visitor_Id");
            RenameColumn(table: "dbo.Conversations", name: "VisitorId", newName: "Visitor_Id");
            AddColumn("dbo.OfflineMessages", "VisitorId", c => c.Int(nullable: false));
            AddColumn("dbo.Conversations", "VisitorId", c => c.Int(nullable: false));
            CreateIndex("dbo.OfflineMessages", "Visitor_Id");
            CreateIndex("dbo.Conversations", "Visitor_Id");
            AddForeignKey("dbo.OfflineMessages", "Visitor_Id", "dbo.Visitors", "Id");
            AddForeignKey("dbo.Conversations", "Visitor_Id", "dbo.Visitors", "Id");
        }
    }
}
