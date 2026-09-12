using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ClientsDbContext : DbContext
{
    public ClientsDbContext(DbContextOptions<ClientsDbContext> options) : base(options)
    {
    }
    
    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientsDbContext).Assembly);
    }
}