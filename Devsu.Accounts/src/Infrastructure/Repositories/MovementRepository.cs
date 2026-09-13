using Application.Abstractions;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class MovementRepository : IMovementRepository
{
    private readonly AccountsDbContext _context;
    public MovementRepository(AccountsDbContext context) => _context = context;
    
    public async Task<Movement?> GetByIdAsync(int movementId) => await _context.Movements.FindAsync(movementId);

    public async Task<IReadOnlyList<Movement>> GetByAccountAsync(string accountNumber) =>
        await _context.Movements.Where(m => m.AccountNumber == accountNumber).ToListAsync();
}