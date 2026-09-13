using Application.Abstractions;
using Domain;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly AccountsDbContext _context;
    public AccountRepository(AccountsDbContext context) => _context = context;

    public async Task<Account?> GetByNumberAsync(string accountNumber) => await _context.Accounts
        .Include(a => a.Movements.OrderByDescending(m => m.MovementId).Take(1))
        .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

    public async Task<Account?> GetByNumberWithMovementsAsync(string accountNumber) => await _context.Accounts
        .Include(a => a.Movements.OrderBy(m => m.MovementId))
        .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

    public async Task<IReadOnlyList<Account>> GetAllAsync() => await _context.Accounts.ToListAsync();

    public async Task AddAsync(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Account account) => await _context.SaveChangesAsync();
}