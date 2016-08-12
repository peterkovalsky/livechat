using Kookaburra.Domain.Model;
using System.Data.Entity;

namespace Kookaburra.Repository
{
    public class KookaburraContext : DbContext
    {
        public KookaburraContext() : base()
        {
        }

        public KookaburraContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {           
            //Database.SetInitializer<KookaburraContext>(null);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}