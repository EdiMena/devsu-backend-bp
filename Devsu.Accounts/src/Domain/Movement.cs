namespace Domain;

public class Movement
{
    public int MovementId { get; private set; }
    public DateOnly MovementDate { get; private set; }
    public string MovementType { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public decimal Balance { get; private set; }
    public string AccountNumber { get; private set; } = null!;

    protected Movement()
    {
    }

    internal Movement(DateOnly movementDate, string movementType, decimal amount, decimal balance, string accountNumber)
    {
        MovementDate = movementDate;
        MovementType = movementType;
        Amount = amount;
        Balance = balance;
        AccountNumber = accountNumber;
    }
}
