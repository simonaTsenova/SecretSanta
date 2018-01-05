using SecretSanta.Data.Contracts;
using System;
using System.Linq;

namespace SecretSanta.Data
{
    public class EfRepository<T> : IEfRepository<T>
        where T : class
    {
        private readonly ISecretSantaDbContext dbContext;

        public EfRepository(ISecretSantaDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException("DbContext cannot be null");
            }

            this.dbContext = dbContext;
        }

        public IQueryable<T> All
        {
            get
            {
                return this.dbContext.DbSet<T>();
            }
        }

        public void Add(T entity)
        {
            this.dbContext.Add(entity);
        }

        public void Delete(T entity)
        {
            this.dbContext.Delete(entity);
        }

        public T GetById(object id)
        {
            return this.dbContext.DbSet<T>().Find(id);
        }

        public void Update(T entity)
        {
            this.dbContext.Update(entity);
        }
    }
}
