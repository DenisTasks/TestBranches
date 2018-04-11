namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlerLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "AppointmentName", c => c.String());
            AddColumn("dbo.Logs", "ActionAuthorId", c => c.Int(nullable: false));
            AddColumn("dbo.Logs", "ActionAuthor_UserId", c => c.Int());
            CreateIndex("dbo.Logs", "ActionAuthor_UserId");
            AddForeignKey("dbo.Logs", "ActionAuthor_UserId", "dbo.Users", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "ActionAuthor_UserId", "dbo.Users");
            DropIndex("dbo.Logs", new[] { "ActionAuthor_UserId" });
            DropColumn("dbo.Logs", "ActionAuthor_UserId");
            DropColumn("dbo.Logs", "ActionAuthorId");
            DropColumn("dbo.Logs", "AppointmentName");
        }
    }
}
