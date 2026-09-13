using Domain;

namespace Application.Abstractions;

public interface IKnownClientRepository
{
    Task<KnownClient?> GetByIdAsync(int clientId);
    Task AddAsync(KnownClient knownClient);
    Task UpdateAsync(KnownClient knownClient);
}