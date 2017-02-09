namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VisitorLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visitors", "Country", c => c.String(maxLength: 200));
            AddColumn("dbo.Visitors", "Region", c => c.String());
            AddColumn("dbo.Visitors", "City", c => c.String());
            AddColumn("dbo.Visitors", "Latitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Visitors", "Longitude", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Visitors", "Location");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visitors", "Location", c => c.String(maxLength: 200));
            DropColumn("dbo.Visitors", "Longitude");
            DropColumn("dbo.Visitors", "Latitude");
            DropColumn("dbo.Visitors", "City");
            DropColumn("dbo.Visitors", "Region");
            DropColumn("dbo.Visitors", "Country");
        }
    }
}
