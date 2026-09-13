using Domain.Enums;

namespace Application.Contracts;

public record RegisterMovementRequest(
    MovementType MovementType,
    decimal Amount);

public record CorrectMovementRequest(int OriginalMovementId, MovementType CorrectMovementType, decimal CorrectAmount);

public record MovementResponse(
    int MovementId,
    DateOnly MovementDate,
    MovementType MovementType,
    decimal Amount,
    decimal Balance,
    string AccountNumber);