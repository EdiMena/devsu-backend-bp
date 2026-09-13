using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.Property(c => c.PersonId).Metadata.SetColumnName("client_id", StoreObjectIdentifier.Table("clients", null));
        builder.Property(c => c.PasswordHash).HasMaxLength(255).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
    }
}