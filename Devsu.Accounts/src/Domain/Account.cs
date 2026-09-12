using Domain.Exceptions;

namespace Domain;

public class Account
{
    public string AccountNumber { get; private set; } = null!;
    public string AccountType { get; private set; } = null!;
    public decimal InitialBalance { get; private set; }
    public decimal AvailableBalance { get; private set; }
    public bool IsActive { get; private set; }
    public int ClientId { get; private set; }
    public string ClientName { get; private set; } = null!;

    private readonly List<Movement> _movements = new();

    public IReadOnlyCollection<Movement> Movements => _movements.AsReadOnly();

    protected Account()
    {
    }

    public Account(string accountNumber, string accountType, decimal initialBalance, int clientId, string clientName)
    {
        if (initialBalance < 0) throw new ValidationException("El saldo inicial no puede ser negativo");

        AccountNumber = accountNumber;
        AccountType = accountType;
        InitialBalance = initialBalance;
        AvailableBalance = initialBalance;
        IsActive = true;
        ClientId = clientId;
        ClientName = clientName;
    }

    public Movement RegisterDeposit(decimal amount)
    {
        if (amount <= 0) throw new ValidationException("El valor del deposito debe ser mayor a cero");
        AvailableBalance += amount;
        var movement = new Movement(DateOnly.FromDateTime(DateTime.UtcNow),"Deposito", amount,AvailableBalance, AccountNumber);
        _movements.Add(movement);
        return movement;
    }

    public Movement RegisterWithdrawal(decimal amount)
    {
        if (amount <= 0) throw new ValidationException("El valor del retiro debe ser mayor a cero");
        if (amount > AvailableBalance) throw new InsufficientBalanceException();
        AvailableBalance -= amount;
        var movement = new Movement(DateOnly.FromDateTime(DateTime.UtcNow),"Retiro", -amount, AvailableBalance, AccountNumber);
        _movements.Add(movement);
        return movement;
    }
}