using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        builder.HasKey(a => a.AccountNumber);
        
        builder.Property(a => a.AccountNumber).HasMaxLength(20);
        builder.Property(a => a.AccountType).HasMaxLength(20).IsRequired();
        builder.Property(a => a.InitialBalance).HasPrecision(18,2).IsRequired();
        builder.Property(a => a.AvailableBalance).HasPrecision(18,2).IsRequired();
        builder.Property(a => a.IsActive).IsRequired();
        builder.Property(a => a.ClientId).IsRequired();
        builder.Property(a => a.ClientName).HasMaxLength(150).IsRequired();
        
        builder.HasMany(a => a.Movements).WithOne().HasForeignKey(m => m.AccountNumber).OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(a => a.Movements).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}