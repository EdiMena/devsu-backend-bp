using Domain.Enums;
using Domain.Exceptions;

namespace Domain;

public class Account
{
    public string AccountNumber { get; private set; } = null!;
    public string AccountType { get; private set; } = null!;
    public decimal InitialBalance { get; private set; }
    public bool IsActive { get; private set; }
    public int ClientId { get; private set; }
    public string ClientName { get; private set; } = null!;

    private readonly List<Movement> _movements = new();
    public IReadOnlyCollection<Movement> Movements => _movements.AsReadOnly();

    public decimal AvailableBalance => _movements.Count > 0
        ? _movements.OrderByDescending(m => m.MovementId).First().Balance
        : InitialBalance;

    protected Account()
    {
    }

    public Account(string accountNumber, string accountType, decimal initialBalance, int clientId, string clientName)
    {
        if (initialBalance < 0) throw new ValidationException("El saldo inicial no puede ser negativo");

        AccountNumber = accountNumber;
        AccountType = accountType;
        InitialBalance = initialBalance;
        IsActive = true;
        ClientId = clientId;
        ClientName = clientName;
    }

    public Movement RegisterDeposit(decimal amount)
    {
        if (amount <= 0) throw new ValidationException("El valor del deposito debe ser mayor a cero");

        var newBalance = AvailableBalance + amount;
        var movement = new Movement(DateOnly.FromDateTime(DateTime.UtcNow), MovementType.Deposito, amount, newBalance,
            AccountNumber);
        _movements.Add(movement);
        return movement;
    }

    public Movement RegisterWithdrawal(decimal amount)
    {
        if (amount <= 0) throw new ValidationException("El valor del retiro debe ser mayor a cero");
        if (amount > AvailableBalance) throw new InsufficientBalanceException();

        var newBalance = AvailableBalance - amount;
        var movement = new Movement(DateOnly.FromDateTime(DateTime.UtcNow), MovementType.Retiro, -amount, newBalance,
            AccountNumber);
        _movements.Add(movement);
        return movement;
    }

    public Movement RegisterReversal(Movement originalMovement)
    {
        var reversalAmount = -originalMovement.Amount;
        var newBalance = AvailableBalance + reversalAmount;
        if (newBalance < 0) throw new InsufficientBalanceException();

        var movement = new Movement(DateOnly.FromDateTime(DateTime.UtcNow), MovementType.Reverso, reversalAmount, newBalance,
            AccountNumber);
        _movements.Add(movement);
        return movement;
    }

    public void CorrectMovement(Movement originalMovement, MovementType correctMovementType, decimal correctAmount)
    {
        RegisterReversal(originalMovement);

        if (correctMovementType == MovementType.Deposito) RegisterDeposit(correctAmount);
        else RegisterWithdrawal(correctAmount);
    }
}