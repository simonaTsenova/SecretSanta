using SecretSanta.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SecretSanta.Data.Configurations
{
    public class LinkEntityConfiguration : EntityTypeConfiguration<Link>
    {
        public LinkEntityConfiguration()
        {
            this.Property(l => l.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
