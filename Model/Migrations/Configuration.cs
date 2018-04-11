namespace Model.Migrations
{
    using Model.Helpers;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Model.WPFOutlookContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Model.WPFOutlookContext";
        }

        protected override void Seed(Model.WPFOutlookContext context)
        {
            foreach(var item in context.Users)
            {
                var salt = EncryptionHelpers.GenerateSalt();
                item.Salt = salt;
                item.Password = EncryptionHelpers.HashPassword(item.Password, salt);
                context.Users.AddOrUpdate(item);
            }
            context.SaveChanges();
        }
    }
}
