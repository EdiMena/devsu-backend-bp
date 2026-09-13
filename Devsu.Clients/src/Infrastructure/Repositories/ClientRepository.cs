using Application.Abstractions;
using Domain;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ClientsDbContext _context;
    public ClientRepository(ClientsDbContext context) => _context = context;
    
    public async Task<Client?> GetByIdAsync(int id) => await _context.Clients.FirstOrDefaultAsync(c => c.PersonId == id);
    
    public async Task<IReadOnlyList<Client>> GetAllAsync() => await _context.Clients.ToListAsync();

    public async Task AddAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Client client) => await _context.SaveChangesAsync();

    public async Task DeactivateAsync(int clientId)
    {
        var client = await GetByIdAsync(clientId);
        if (client is null) throw new NotFoundException($"Cliente {clientId} no encontrado");
        
        client.Deactivate();
        await _context.SaveChangesAsync();
    }
}