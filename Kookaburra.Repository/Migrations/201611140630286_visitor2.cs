namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class visitor2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visitors", "ConversationStarted", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visitors", "ConversationStarted");
        }
    }
}
