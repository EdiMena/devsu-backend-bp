using Domain;

namespace Application.Abstractions;

public interface IClientRepository
{
    Task<Client> GetByIdAsync(int id);
    Task<IReadOnlyList<Client>> GetAllAsync();
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
    Task DeleteAsync(int clientId);
}