namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accountupdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Website", c => c.String(maxLength: 500));
            AddColumn("dbo.Accounts", "IsTrial", c => c.Boolean(nullable: false));
            AddColumn("dbo.Accounts", "SignUpDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Accounts", "TrialExpiryDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Accounts", "Key", c => c.String(maxLength: 50));
            DropColumn("dbo.Accounts", "Name");
            DropColumn("dbo.Accounts", "Identifier");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "Identifier", c => c.String(maxLength: 50));
            AddColumn("dbo.Accounts", "Name", c => c.String(maxLength: 500));
            DropColumn("dbo.Accounts", "Key");
            DropColumn("dbo.Accounts", "TrialExpiryDate");
            DropColumn("dbo.Accounts", "SignUpDate");
            DropColumn("dbo.Accounts", "IsTrial");
            DropColumn("dbo.Accounts", "Website");
        }
    }
}
