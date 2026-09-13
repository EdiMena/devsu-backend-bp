using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");
        builder.HasKey(p => p.PersonId);

        builder.Property(p => p.Name).HasMaxLength(150).IsRequired();
        builder.Property(p => p.Gender).HasConversion<string>().HasMaxLength(1).IsRequired();
        builder.Property(p => p.IdentificationNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(p => p.IdentificationNumber).IsUnique();
        builder.Property(p => p.Address).HasMaxLength(250);
        builder.Property(p => p.PhoneNumber).HasMaxLength(20);
    }
}