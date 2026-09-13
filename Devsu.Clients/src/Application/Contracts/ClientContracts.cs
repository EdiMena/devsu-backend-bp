using Domain.Enums;

namespace Application.Contracts;

public record CreateClientRequest(
    string Name,
    Gender Gender,
    int Age,
    string IdentificationNumber,
    string Address,
    string PhoneNumber,
    string Password);

public record UpdateClientRequest(
    string Name,
    Gender Gender,
    int Age,
    string IdentificationNumber,
    string Address,
    string PhoneNumber);

public record ClientResponse(
    int ClientId,
    string Name,
    Gender Gender,
    int Age,
    string IdentificationNumber,
    string Address,
    string PhoneNumber,
    bool IsActive);