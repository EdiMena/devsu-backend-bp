using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests;

public class ClientTests
{
    private static Client CreateValidClient() =>
        new("Jose Lema", Gender.M, 35, "1234567890", "Otavalo sn y principal", "098254785", "hashed-password");

    [Fact]
    public void Constructor_WithValidData_CreatesActiveClient()
    {
        var client = CreateValidClient();

        Assert.True(client.IsActive);
        Assert.Equal("Jose Lema", client.Name);
        Assert.Equal(Gender.M, client.Gender);
        Assert.Equal("hashed-password", client.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidName_ThrowsValidationException(string? invalidName)
    {
        var exception = Assert.Throws<ValidationException>(() =>
            new Client(invalidName!, Gender.M, 35, "1234567890", "Address", "0999999999", "hash"));

        Assert.Equal("El nombre es obligatorio", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidAge_ThrowsValidationException(int invalidAge)
    {
        var exception = Assert.Throws<ValidationException>(() =>
            new Client("Jose Lema", Gender.M, invalidAge, "1234567890", "Address", "0999999999", "hash"));

        Assert.Equal("La edad debe ser mayor a cero", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidIdentificationNumber_ThrowsValidationException(string invalidId)
    {
        var exception = Assert.Throws<ValidationException>(() =>
            new Client("Jose Lema", Gender.M, 35, invalidId, "Address", "0999999999", "hash"));

        Assert.Equal("El número de identificación es obligatorio", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidPasswordHash_ThrowsValidationException(string? invalidPasswordHash)
    {
        var exception = Assert.Throws<ValidationException>(() =>
            new Client("Jose Lema", Gender.M, 35, "1234567890", "Address", "0999999999", invalidPasswordHash!));

        Assert.Equal("La contraseña es obligatoria", exception.Message);
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var client = CreateValidClient();
        client.Deactivate();
        Assert.False(client.IsActive);
    }

    [Fact]
    public void Activate_SetsIsActiveToTrue()
    {
        var client = CreateValidClient();
        client.Deactivate();
        client.Activate();
        Assert.True(client.IsActive);
    }

    [Fact]
    public void ChangePassword_UpdatesPasswordHash()
    {
        var client = CreateValidClient();
        client.ChangePassword("new-hashed-password");
        Assert.Equal("new-hashed-password", client.PasswordHash);
    }

    [Fact]
    public void UpdateProfile_WithValidData_UpdatesAllFields()
    {
        var client = CreateValidClient();

        client.UpdateProfile("Jose Lema Updated", Gender.M, 36, "1234567890", "New Address", "0988888888");

        Assert.Equal("Jose Lema Updated", client.Name);
        Assert.Equal(36, client.Age);
        Assert.Equal("New Address", client.Address);
        Assert.Equal("0988888888", client.PhoneNumber);
    }

    [Fact]
    public void UpdateProfile_WithInvalidName_ThrowsValidationException_AndDoesNotChangeState()
    {
        var client = CreateValidClient();
        Assert.Throws<ValidationException>(() =>
            client.UpdateProfile("", Gender.M, 36, "1234567890", "New Address", "0988888888"));

        Assert.Equal("Jose Lema", client.Name);
    }
}