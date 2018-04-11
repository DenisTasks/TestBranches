namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupMigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.GroupGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupGroups", "RelatedId", "dbo.Groups");
            DropIndex("dbo.GroupGroups", new[] { "GroupId" });
            DropIndex("dbo.GroupGroups", new[] { "RelatedId" });
            DropTable("dbo.GroupGroups");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GroupGroups",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        RelatedId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.RelatedId });
            
            CreateIndex("dbo.GroupGroups", "RelatedId");
            CreateIndex("dbo.GroupGroups", "GroupId");
            AddForeignKey("dbo.GroupGroups", "RelatedId", "dbo.Groups", "GroupId");
            AddForeignKey("dbo.GroupGroups", "GroupId", "dbo.Groups", "GroupId");
        }
    }
}
