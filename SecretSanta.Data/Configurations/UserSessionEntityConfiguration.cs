using SecretSanta.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SecretSanta.Data.Configurations
{
    public class UserSessionEntityConfiguration : EntityTypeConfiguration<UserSession>
    {
        public UserSessionEntityConfiguration()
        {
            this.Property(s => s.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
