using Domain;

namespace Application.Abstractions;

public interface IMovementRepository
{
    Task<Movement?> GetByIdAsync(int movementId);
    Task<IReadOnlyList<Movement>> GetByAccountAsync(string accountNumber);
}