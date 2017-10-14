using Kookaburra.Domain.Model;
using System.Data.Entity;

namespace Kookaburra.Repository
{
    public class KookaburraContext : DbContext
    {
        public KookaburraContext() : base("DefaultConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.CommandTimeout = 180;
        }

        public KookaburraContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            //Database.SetInitializer<KookaburraContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.CommandTimeout = 180;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OfflineMessage> OfflineMessages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Visitor>()
            //    .HasOptional(s => s.Messages)
            //    .WithMany()
            //    .WillCascadeOnDelete(true);
        }
    }
}