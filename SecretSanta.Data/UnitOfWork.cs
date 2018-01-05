using SecretSanta.Data.Contracts;
using System;

namespace SecretSanta.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISecretSantaDbContext dbContext;

        public UnitOfWork(ISecretSantaDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException("Db context cannot be null");
            }

            this.dbContext = dbContext;
        }

        public void Commit()
        {
            this.dbContext.SaveChanges();
        }
    }
}
