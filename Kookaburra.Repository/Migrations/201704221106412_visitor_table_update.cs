namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class visitor_table_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visitors", "IpAddress", c => c.String(maxLength: 50));
            AddColumn("dbo.Visitors", "CountryCode", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visitors", "CountryCode");
            DropColumn("dbo.Visitors", "IpAddress");
        }
    }
}
