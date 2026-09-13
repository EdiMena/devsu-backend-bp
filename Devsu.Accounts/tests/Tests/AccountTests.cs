using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests;

public class AccountTests
{
    private static Account CreateValidAccount(decimal initialBalance = 1000) =>
        new("478758", AccountType.Ahorros, initialBalance, clientId: 1, clientName: "Jose Lema");

    [Fact]
    public void Constructor_WithValidData_CreatesActiveAccountWithInitialBalanceAsAvailableBalance()
    {
        var account = CreateValidAccount(1000);

        Assert.True(account.IsActive);
        Assert.Equal(1000, account.AvailableBalance);
        Assert.Empty(account.Movements);
    }

    [Fact]
    public void Constructor_WithNegativeInitialBalance_ThrowsValidationException()
    {
        var exception = Assert.Throws<ValidationException>(() =>
            new Account("478758", AccountType.Ahorros, -100, clientId: 1, clientName: "Jose Lema"));

        Assert.Equal("El saldo inicial no puede ser negativo", exception.Message);
    }

    [Fact]
    public void RegisterDeposit_WithValidAmount_IncreasesAvailableBalanceAndCreatesMovement()
    {
        var account = CreateValidAccount(1000);

        var movement = account.RegisterDeposit(500);

        Assert.Equal(1500, account.AvailableBalance);
        Assert.Equal(MovementType.Deposito, movement.MovementType);
        Assert.Equal(500, movement.Amount);
        Assert.Equal(1500, movement.Balance);
        Assert.Single(account.Movements);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void RegisterDeposit_WithNonPositiveAmount_ThrowsValidationException(decimal invalidAmount)
    {
        var account = CreateValidAccount();

        var exception = Assert.Throws<ValidationException>(() => account.RegisterDeposit(invalidAmount));

        Assert.Equal("El valor del deposito debe ser mayor a cero", exception.Message);
    }

    [Fact]
    public void RegisterWithdrawal_WithSufficientBalance_DecreasesAvailableBalanceAndCreatesMovement()
    {
        var account = CreateValidAccount(1000);

        var movement = account.RegisterWithdrawal(300);

        Assert.Equal(700, account.AvailableBalance);
        Assert.Equal(MovementType.Retiro, movement.MovementType);
        Assert.Equal(-300, movement.Amount);
        Assert.Equal(700, movement.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void RegisterWithdrawal_WithNonPositiveAmount_ThrowsValidationException(decimal invalidAmount)
    {
        var account = CreateValidAccount();

        var exception = Assert.Throws<ValidationException>(() => account.RegisterWithdrawal(invalidAmount));

        Assert.Equal("El valor del retiro debe ser mayor a cero", exception.Message);
    }

    [Fact]
    public void RegisterWithdrawal_WithAmountGreaterThanAvailableBalance_ThrowsInsufficientBalanceException()
    {
        var account = CreateValidAccount(100);

        var exception = Assert.Throws<InsufficientBalanceException>(() => account.RegisterWithdrawal(200));

        Assert.Equal("Saldo no disponible", exception.Message);
        Assert.Equal(422, exception.StatusCode);
    }

    [Fact]
    public void RegisterWithdrawal_WithAmountEqualToAvailableBalance_Succeeds()
    {
        var account = CreateValidAccount(100);
        account.RegisterWithdrawal(100);
        Assert.Equal(0, account.AvailableBalance);
    }

    [Fact]
    public void RegisterReversal_OfADeposit_CreatesReversoMovementWithNegatedAmount()
    {
        var account = CreateValidAccount(1000);
        var deposit = account.RegisterDeposit(500);

        var reversal = account.RegisterReversal(deposit);

        Assert.Equal(MovementType.Reverso, reversal.MovementType);
        Assert.Equal(-500, reversal.Amount);
        Assert.Equal(1000, account.AvailableBalance);
        Assert.Equal(2, account.Movements.Count);
    }

    [Fact]
    public void RegisterReversal_OfAWithdrawal_RestoresTheWithdrawnAmount()
    {
        var account = CreateValidAccount(1000);
        var withdrawal = account.RegisterWithdrawal(300);

        account.RegisterReversal(withdrawal);

        Assert.Equal(1000, account.AvailableBalance);
    }

    [Fact]
    public void RegisterReversal_ThatWouldMakeBalanceNegative_ThrowsInsufficientBalanceException()
    {
        var account = CreateValidAccount(1000);
        var deposit = account.RegisterDeposit(500);
        account.RegisterWithdrawal(1400);

        Assert.Throws<InsufficientBalanceException>(() => account.RegisterReversal(deposit));
    }

    [Fact]
    public void CorrectMovement_ReplacesAWithdrawalWithADeposit_ResultsInReversalPlusNewMovement()
    {
        var account = CreateValidAccount(1000);
        var wrongWithdrawal = account.RegisterWithdrawal(300);

        account.CorrectMovement(wrongWithdrawal, MovementType.Deposito, 50);

        Assert.Equal(1050, account.AvailableBalance);
        Assert.Equal(3, account.Movements.Count);
        Assert.Equal(MovementType.Reverso, account.Movements.ElementAt(1).MovementType);
        Assert.Equal(MovementType.Deposito, account.Movements.ElementAt(2).MovementType);
    }

    [Fact]
    public void AvailableBalance_ReflectsTheMostRecentlyAddedMovement_NotTheHighestMovementId()
    {
        var account = CreateValidAccount(1000);
        var original = account.RegisterWithdrawal(300);
        account.CorrectMovement(original, MovementType.Retiro, 30);
        Assert.Equal(970, account.AvailableBalance);
    }

    [Fact]
    public void ChangeAccountType_UpdatesTheAccountType()
    {
        var account = CreateValidAccount();
        account.ChangeAccountType(AccountType.Corriente);
        Assert.Equal(AccountType.Corriente, account.AccountType);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var account = CreateValidAccount();
        account.Deactivate();
        Assert.False(account.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        var account = CreateValidAccount();
        account.Deactivate();
        account.Activate();
        Assert.True(account.IsActive);
    }
}