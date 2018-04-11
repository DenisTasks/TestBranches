using Model.Entities;
using Model.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using static Model.WPFOutlookInitializer;
using SqlProviderServices = System.Data.Entity.SqlServer.SqlProviderServices;

namespace Model
{
    public class WPFOutlookInitializer : IDatabaseInitializer<WPFOutlookContext>
    {
        public void InitializeDatabase(WPFOutlookContext context)
        {
            if (!context.Database.Exists())
            {
                context.Database.Create();
                var salt = EncryptionHelpers.GenerateSalt();
                User user = new User
                {
                    UserId = 1,
                    IsActive = true,
                    Name = "admin",
                    UserName = "admin",
                    Salt = salt,
                    Password = EncryptionHelpers.HashPassword("admin", salt)
                };
                context.Users.Add(user);
                context.Roles.Add(new Role { RoleId = 1, Name = "admin", Users = new List<User> { user } });
                context.Roles.Add(new Role { RoleId = 2, Name = "user", Users = new List<User> { user } });
                context.Locations.Add(new Location {LocationId = 1, Room = "Room1"});
                context.Locations.Add(new Location { LocationId = 2, Room = "Room2" });
                context.SaveChanges();
            }
        }
    }


    public class WPFOutlookContext : DbContext
    {
        public WPFOutlookContext()
            : base("name=WPFOutlookContext")
        {
            Database.SetInitializer(new WPFOutlookInitializer());
        }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany<Group>(s => s.Groups)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("GroupId");
                    cs.ToTable("UserGroups");
                });


            modelBuilder.Entity<User>()
                .HasMany<Appointment>(s => s.Appointments)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("AppointmentId");
                    cs.ToTable("UserAppointments");
                });


            modelBuilder.Entity<User>()
                .HasMany<Notification>(s => s.Notifications)
                .WithMany(c => c.Users)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("NotificationId");
                    cs.ToTable("UserNotifications");
                });

            //modelBuilder.Entity<Group>()
            //    .HasMany<Group>(s => s.Groups)
            //    .WithMany()
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("GroupId");
            //        cs.MapRightKey("RelatedId");
            //        cs.ToTable("GroupGroups");
            //    });


            //modelBuilder.Entity<Group>()
            //    .HasMany<Appointment>(s => s.Appointments)
            //    .WithMany(c => c.Users)
            //    .Map(cs =>
            //    {
            //        cs.MapLeftKey("UserId");
            //        cs.MapRightKey("AppointmentId");
            //        cs.ToTable("UserAppointments");
            //    });

        }
    }
}