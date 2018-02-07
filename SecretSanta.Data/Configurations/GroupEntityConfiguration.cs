using SecretSanta.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace SecretSanta.Data.Configurations
{
    public class GroupEntityConfiguration : EntityTypeConfiguration<Group>
    {
        public GroupEntityConfiguration()
        {
            this.Property(g => g.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_GroupName", 1) { IsUnique = true }));
        }
    }
}
