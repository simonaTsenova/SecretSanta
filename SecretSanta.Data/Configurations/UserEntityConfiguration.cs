using SecretSanta.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace SecretSanta.Data.Configurations
{
    public class UserEntityConfiguration : EntityTypeConfiguration<User>
    {
        public UserEntityConfiguration()
        {
            this.Property(user => user.Email)
                .IsRequired();

            this.Property(user => user.UserName)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("IX_UserUserName") { IsUnique = true }));

            this.Property(user => user.DisplayName)
                .IsRequired()
                .HasMaxLength(60);

            this.Property(user => user.FirstName)
                .HasMaxLength(15);

            this.Property(user => user.LastName)
                .HasMaxLength(20);
        }
    }
}
