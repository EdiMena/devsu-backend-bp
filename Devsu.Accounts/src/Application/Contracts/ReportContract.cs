using System.Text.Json.Serialization;
using Domain.Enums;

namespace Application.Contracts;

public record AccountStatementItem(
    [property: JsonPropertyName("Fecha")]
    DateOnly Date,
    [property: JsonPropertyName("Cliente")]
    string ClientName,
    [property: JsonPropertyName("Numero Cuenta")]
    string AccountNumber,
    [property: JsonPropertyName("Tipo")]
    AccountType AccountType,
    [property: JsonPropertyName("Saldo Inicial")]
    decimal InitialBalance,
    [property: JsonPropertyName("Estado")]
    bool IsActive,
    [property: JsonPropertyName("Movimiento")]
    decimal MovementAmount,
    [property: JsonPropertyName("Saldo Disponible")]
    decimal AvailableBalance);