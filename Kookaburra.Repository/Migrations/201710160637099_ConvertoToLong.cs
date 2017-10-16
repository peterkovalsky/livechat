namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConvertoToLong : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Conversations", "VisitorId", "dbo.Visitors");
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.Conversations", new[] { "VisitorId" });
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            DropPrimaryKey("dbo.Visitors");
            AddColumn("dbo.Operators", "Identifier", c => c.String(maxLength: 50));
            AddColumn("dbo.Visitors", "Identifier", c => c.String(maxLength: 100));
            AddColumn("dbo.Conversations", "Visitor_Id", c => c.Long());
            AddColumn("dbo.OfflineMessages", "Visitor_Id", c => c.Long());
            AlterColumn("dbo.Visitors", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Visitors", "Region", c => c.String(maxLength: 1000));
            AlterColumn("dbo.Visitors", "City", c => c.String(maxLength: 1000));
            AddPrimaryKey("dbo.Visitors", "Id");
            CreateIndex("dbo.Conversations", "Visitor_Id");
            CreateIndex("dbo.OfflineMessages", "Visitor_Id");
            AddForeignKey("dbo.Conversations", "Visitor_Id", "dbo.Visitors", "Id");
            AddForeignKey("dbo.OfflineMessages", "Visitor_Id", "dbo.Visitors", "Id");
            DropColumn("dbo.Operators", "Identity");
            DropColumn("dbo.Visitors", "SessionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visitors", "SessionId", c => c.String(maxLength: 1000));
            AddColumn("dbo.Operators", "Identity", c => c.String(maxLength: 50));
            DropForeignKey("dbo.OfflineMessages", "Visitor_Id", "dbo.Visitors");
            DropForeignKey("dbo.Conversations", "Visitor_Id", "dbo.Visitors");
            DropIndex("dbo.OfflineMessages", new[] { "Visitor_Id" });
            DropIndex("dbo.Conversations", new[] { "Visitor_Id" });
            DropPrimaryKey("dbo.Visitors");
            AlterColumn("dbo.Visitors", "City", c => c.String());
            AlterColumn("dbo.Visitors", "Region", c => c.String());
            AlterColumn("dbo.Visitors", "Id", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.OfflineMessages", "Visitor_Id");
            DropColumn("dbo.Conversations", "Visitor_Id");
            DropColumn("dbo.Visitors", "Identifier");
            DropColumn("dbo.Operators", "Identifier");
            AddPrimaryKey("dbo.Visitors", "Id");
            CreateIndex("dbo.OfflineMessages", "VisitorId");
            CreateIndex("dbo.Conversations", "VisitorId");
            AddForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Conversations", "VisitorId", "dbo.Visitors", "Id", cascadeDelete: true);
        }
    }
}
