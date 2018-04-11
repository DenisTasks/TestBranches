namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LogCreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Action = c.String(),
                        CreatorId = c.Int(nullable: false),
                        EventTime = c.DateTime(nullable: false),
                        Creator_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Users", t => t.Creator_UserId)
                .Index(t => t.Creator_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "Creator_UserId", "dbo.Users");
            DropIndex("dbo.Logs", new[] { "Creator_UserId" });
            DropTable("dbo.Logs");
        }
    }
}
