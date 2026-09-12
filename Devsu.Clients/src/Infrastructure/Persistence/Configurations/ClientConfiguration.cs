using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.Property(c => c.PersonId).HasColumnName("client_id");
        builder.Property(c => c.PasswordHash).HasMaxLength(250).IsRequired();
        builder.Property(c => c.IsActive).IsRequired();
    }
}