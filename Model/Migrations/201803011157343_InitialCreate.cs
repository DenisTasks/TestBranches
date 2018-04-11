namespace Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentId = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        BeginningDate = c.DateTime(nullable: false),
                        EndingDate = c.DateTime(nullable: false),
                        OrganizerId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        Organizer_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.AppointmentId)
                .ForeignKey("dbo.Locations", t => t.LocationId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.Organizer_UserId)
                .Index(t => t.LocationId)
                .Index(t => t.Organizer_UserId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        Room = c.String(),
                    })
                .PrimaryKey(t => t.LocationId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        ParentId = c.Int(),
                        CreatorId = c.Int(nullable: false),
                        Creator_UserId = c.Int(),
                        Parent_GroupId = c.Int(),
                    })
                .PrimaryKey(t => t.GroupId)
                .ForeignKey("dbo.Users", t => t.Creator_UserId)
                .ForeignKey("dbo.Groups", t => t.Parent_GroupId)
                .Index(t => t.Creator_UserId)
                .Index(t => t.Parent_GroupId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "dbo.UserAppointments",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        AppointmentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.AppointmentId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.AppointmentId);
            
            CreateTable(
                "dbo.GroupGroups",
                c => new
                    {
                        GroupId = c.Int(nullable: false),
                        RelatedId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.RelatedId })
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .ForeignKey("dbo.Groups", t => t.RelatedId)
                .Index(t => t.GroupId)
                .Index(t => t.RelatedId);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        GroupId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.GroupId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        Role_RoleId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.Roles", t => t.Role_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Appointments", "Organizer_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.UserGroups", "UserId", "dbo.Users");
            DropForeignKey("dbo.Groups", "Parent_GroupId", "dbo.Groups");
            DropForeignKey("dbo.GroupGroups", "RelatedId", "dbo.Groups");
            DropForeignKey("dbo.GroupGroups", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "Creator_UserId", "dbo.Users");
            DropForeignKey("dbo.UserAppointments", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.UserAppointments", "UserId", "dbo.Users");
            DropForeignKey("dbo.Appointments", "LocationId", "dbo.Locations");
            DropIndex("dbo.RoleUsers", new[] { "User_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleId" });
            DropIndex("dbo.UserGroups", new[] { "GroupId" });
            DropIndex("dbo.UserGroups", new[] { "UserId" });
            DropIndex("dbo.GroupGroups", new[] { "RelatedId" });
            DropIndex("dbo.GroupGroups", new[] { "GroupId" });
            DropIndex("dbo.UserAppointments", new[] { "AppointmentId" });
            DropIndex("dbo.UserAppointments", new[] { "UserId" });
            DropIndex("dbo.Groups", new[] { "Parent_GroupId" });
            DropIndex("dbo.Groups", new[] { "Creator_UserId" });
            DropIndex("dbo.Appointments", new[] { "Organizer_UserId" });
            DropIndex("dbo.Appointments", new[] { "LocationId" });
            DropTable("dbo.RoleUsers");
            DropTable("dbo.UserGroups");
            DropTable("dbo.GroupGroups");
            DropTable("dbo.UserAppointments");
            DropTable("dbo.Roles");
            DropTable("dbo.Groups");
            DropTable("dbo.Users");
            DropTable("dbo.Locations");
            DropTable("dbo.Appointments");
        }
    }
}
