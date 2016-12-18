namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Visitor_Conversation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "OperatorId", "dbo.Operators");
            DropForeignKey("dbo.Messages", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.Messages", new[] { "VisitorId" });
            DropIndex("dbo.Messages", new[] { "OperatorId" });
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VisitorId = c.Int(nullable: false),
                        OperatorId = c.Int(nullable: false),
                        TimeStarted = c.DateTime(nullable: false),
                        TimeFinished = c.DateTime(),
                        Page = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Operators", t => t.OperatorId, cascadeDelete: true)
                .ForeignKey("dbo.Visitors", t => t.VisitorId, cascadeDelete: true)
                .Index(t => t.VisitorId)
                .Index(t => t.OperatorId);
            
            AddColumn("dbo.Messages", "ConversationId", c => c.Int(nullable: false));
            CreateIndex("dbo.Messages", "ConversationId");
            AddForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations", "Id", cascadeDelete: true);
            DropColumn("dbo.Messages", "VisitorId");
            DropColumn("dbo.Messages", "OperatorId");
            DropColumn("dbo.Visitors", "Page");
            DropColumn("dbo.Visitors", "ConversationStarted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visitors", "ConversationStarted", c => c.DateTime(nullable: false));
            AddColumn("dbo.Visitors", "Page", c => c.String());
            AddColumn("dbo.Messages", "OperatorId", c => c.Int());
            AddColumn("dbo.Messages", "VisitorId", c => c.Int());
            DropForeignKey("dbo.Conversations", "VisitorId", "dbo.Visitors");
            DropForeignKey("dbo.Conversations", "OperatorId", "dbo.Operators");
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropIndex("dbo.Conversations", new[] { "OperatorId" });
            DropIndex("dbo.Conversations", new[] { "VisitorId" });
            DropColumn("dbo.Messages", "ConversationId");
            DropTable("dbo.Conversations");
            CreateIndex("dbo.Messages", "OperatorId");
            CreateIndex("dbo.Messages", "VisitorId");
            AddForeignKey("dbo.Messages", "VisitorId", "dbo.Visitors", "Id");
            AddForeignKey("dbo.Messages", "OperatorId", "dbo.Operators", "Id");
        }
    }
}
