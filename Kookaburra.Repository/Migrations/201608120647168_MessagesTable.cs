namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessagesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        Identifier = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Operators",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Identity = c.String(maxLength: 50),
                        FirstName = c.String(maxLength: 100),
                        LastName = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        Type = c.String(maxLength: 50),
                        AccountId = c.Int(nullable: false),
                        LastActivity = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Visitors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 200),
                        Email = c.String(maxLength: 200),
                        Location = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Operators", "AccountId", "dbo.Accounts");
            DropIndex("dbo.Operators", new[] { "AccountId" });
            DropTable("dbo.Visitors");
            DropTable("dbo.Operators");
            DropTable("dbo.Accounts");
        }
    }
}
