namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offlinemessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OfflineMessages", "Page", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OfflineMessages", "Page");
        }
    }
}
