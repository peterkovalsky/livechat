namespace Kookaburra.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class offline_messages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OfflineMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Message = c.String(),
                        DateSent = c.DateTime(nullable: false),
                        VisitorId = c.Int(),
                        IsRead = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Visitors", t => t.VisitorId)
                .Index(t => t.VisitorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OfflineMessages", "VisitorId", "dbo.Visitors");
            DropIndex("dbo.OfflineMessages", new[] { "VisitorId" });
            DropTable("dbo.OfflineMessages");
        }
    }
}
