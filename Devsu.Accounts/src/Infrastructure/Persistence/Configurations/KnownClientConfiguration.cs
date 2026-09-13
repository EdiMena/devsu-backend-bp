using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class KnownClientConfiguration : IEntityTypeConfiguration<KnownClient>
{
    public void Configure(EntityTypeBuilder<KnownClient> builder)
    {
        builder.ToTable("known_clients");
        builder.HasKey(k => k.ClientId);
        builder.Property(k => k.ClientId).ValueGeneratedNever();
        builder.Property(k => k.Name).HasMaxLength(150).IsRequired();
        builder.Property(k => k.IsActive).IsRequired();
    }
}