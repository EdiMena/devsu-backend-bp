using Application.Abstractions;
using Domain;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class KnownClientRepository : IKnownClientRepository
{
    private readonly AccountsDbContext _context;
    public KnownClientRepository(AccountsDbContext context) => _context = context;

    public async Task<KnownClient?> GetByIdAsync(int clientId) => await _context.KnownClients.FindAsync(clientId);

    public async Task AddAsync(KnownClient client)
    {
        _context.KnownClients.Add(client);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(KnownClient client) => await _context.SaveChangesAsync();
}