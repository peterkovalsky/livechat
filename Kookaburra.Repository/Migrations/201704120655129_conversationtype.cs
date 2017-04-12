namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class conversationtype : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropPrimaryKey("dbo.Conversations");
            AlterColumn("dbo.Conversations", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Messages", "ConversationId", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Conversations", "Id");
            CreateIndex("dbo.Messages", "ConversationId");
            AddForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations");
            DropIndex("dbo.Messages", new[] { "ConversationId" });
            DropPrimaryKey("dbo.Conversations");
            AlterColumn("dbo.Messages", "ConversationId", c => c.Int(nullable: false));
            AlterColumn("dbo.Conversations", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Conversations", "Id");
            CreateIndex("dbo.Messages", "ConversationId");
            AddForeignKey("dbo.Messages", "ConversationId", "dbo.Conversations", "Id", cascadeDelete: true);
        }
    }
}
