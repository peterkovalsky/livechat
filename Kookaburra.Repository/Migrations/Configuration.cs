namespace Kookaburra.Repository.Migrations
{
    using Domain.Common;
    using Domain.Model;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<KookaburraContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(KookaburraContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var testAccount = new Account { Name = "John Dou", Identifier = "086FBDC2-14F3-4F68-B3C6-9EA42D257061" };
            context.Accounts.AddOrUpdate(a => a.Identifier, testAccount);

            context.Operators.AddOrUpdate(o => o.Identity,
                new Operator
                {
                    Identity = "7a55ab0f-c4e7-47e6-92f0-50af290352e3",
                    FirstName = "John",
                    LastName = "Dou",
                    Email = "peter.kovalskyy@gmail.com",
                    Type = OperatorType.INDIVIDUAL.ToString(),
                    Account = testAccount
                });
        }
    }
}