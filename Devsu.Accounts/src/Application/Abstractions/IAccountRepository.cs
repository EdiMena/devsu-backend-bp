using Domain;

namespace Application.Abstractions;

public interface IAccountRepository
{
    Task<Account?> GetByNumberAsync(string accountNumber);
    Task<Account?> GetByNumberWithMovementsAsync(string accountNumber);
    Task<IReadOnlyList<Account>> GetAllAsync();
    Task<IReadOnlyList<Account>> GetAllByClientIdAsync(int clientId);
    Task AddAsync(Account account);
    Task UpdateAsync(Account account);
}