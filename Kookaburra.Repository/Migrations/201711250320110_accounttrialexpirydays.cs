namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class accounttrialexpirydays : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "TrialPeriodDays", c => c.Int(nullable: false));
            DropColumn("dbo.Accounts", "TrialExpiryDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accounts", "TrialExpiryDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Accounts", "TrialPeriodDays");
        }
    }
}
