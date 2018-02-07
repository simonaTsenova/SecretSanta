using SecretSanta.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace SecretSanta.Data.Configurations
{
    public class InvitationEntityConfiguration : EntityTypeConfiguration<Invitation>
    {
        public InvitationEntityConfiguration()
        {
            this.Property(i => i.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }
    }
}
