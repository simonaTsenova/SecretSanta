using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecretSanta.Data.Contracts;
using SecretSanta.Models;
using SecretSanta.Data.Configurations;

namespace SecretSanta.Data
{
    public class SecretSantaDbContext : IdentityDbContext<User>, ISecretSantaDbContext
    {
        public SecretSantaDbContext()
            : base("LocalSecretSantaConnection", throwIfV1Schema: false)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<SecretSantaDbContext, Configuration>());
        }

        public static SecretSantaDbContext Create()
        {
            return new SecretSantaDbContext();
        }

        public DbSet<UserSession> UserSessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new UserEntityConfiguration());
            modelBuilder.Configurations.Add(new UserSessionEntityConfiguration());
        }

        public void Add<TEntry>(TEntry entity) where TEntry : class
        {
            var entry = this.Entry(entity);
            entry.State = EntityState.Added;
        }

        public IDbSet<TEntity> DbSet<TEntity>() where TEntity : class
        {
            return this.Set<TEntity>();
        }

        public void Delete<TEntry>(TEntry entity) where TEntry : class
        {
            var entry = this.Entry(entity);
            entry.State = EntityState.Deleted;
        }

        public void Update<TEntry>(TEntry entity) where TEntry : class
        {
            var entry = this.Entry(entity);
            entry.State = EntityState.Modified;
        }
    }
}
