using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Data.Contracts
{
    public interface ISecretSantaDbContext
    {
        IDbSet<TEntity> DbSet<TEntity>() where TEntity : class;

        void Add<TEntry>(TEntry entity)
            where TEntry : class;

        void Delete<TEntry>(TEntry entity)
            where TEntry : class;

        void Update<TEntry>(TEntry entity)
            where TEntry : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();
    }
}
