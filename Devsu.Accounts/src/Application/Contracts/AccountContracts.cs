using Domain;
using Domain.Enums;

namespace Application.Contracts;

public record CreateAccountRequest(
    string AccountNumber,
    AccountType AccountType,
    decimal InitialBalance,
    int ClientId);

public record UpdateAccountRequest(
    AccountType AccountType);

public record AccountResponse(
    string AccountNumber,
    AccountType AccountType,
    decimal InitialBalance,
    decimal AvailableBalance,
    bool IsActive,
    int ClientId,
    string ClientName);