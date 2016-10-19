namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessageLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "SentBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "SentBy");
        }
    }
}
