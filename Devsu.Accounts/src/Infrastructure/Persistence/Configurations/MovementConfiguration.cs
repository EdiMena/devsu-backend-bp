using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class MovementConfiguration : IEntityTypeConfiguration<Movement>
{
    public void Configure(EntityTypeBuilder<Movement> builder)
    {
        builder.ToTable("movements");
        builder.HasKey(m => m.MovementId);

        builder.Property(m => m.MovementDate).IsRequired();
        builder.Property(m => m.MovementType).HasMaxLength(20).IsRequired();
        builder.Property(m => m.Amount).HasPrecision(18,2).IsRequired();
        builder.Property(m => m.Balance).HasPrecision(18,2).IsRequired();
        builder.Property(m => m.AccountNumber).HasMaxLength(20).IsRequired();
    }
}