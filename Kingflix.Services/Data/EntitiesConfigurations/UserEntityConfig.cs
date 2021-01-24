using System.Data.Entity.ModelConfiguration;
using Kingflix.Domain.DomainModel.IdentityModel;

namespace Kingflix.Services.Data.EntitiesConfigurations
{
    internal class UserEntityConfig : EntityTypeConfiguration<AppUser>
    {
        public UserEntityConfig()
        {
            HasKey(c => c.Id);

            Property(c => c.DateCreated)
                .IsRequired()
                .HasColumnType("datetime");

            Property(c => c.FullName)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
