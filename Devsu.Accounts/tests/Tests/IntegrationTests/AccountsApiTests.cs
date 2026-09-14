using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Contracts;
using Domain.Enums;

namespace Tests.IntegrationTests;

public class AccountsApiTests : IClassFixture<AccountsApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly AccountsApiFactory _factory;
    private readonly HttpClient _client;
    
    public AccountsApiTests(AccountsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task PostAccounts_WithUnknownClient_Returns404()
    {
        var request = new CreateAccountRequest("999999", AccountType.Ahorros, 100, ClientId: 999);

        var response = await _client.PostAsJsonAsync("/accounts", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostAccounts_WithKnownClient_Returns201WithClientNameFromCache()
    {
        await _factory.SeedKnownClientAsync(1, "Jose Lema");
        var request = new CreateAccountRequest("111111", AccountType.Ahorros, 1000, ClientId: 1);

        var response = await _client.PostAsJsonAsync("/accounts", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AccountResponse>(JsonOptions);
        Assert.Equal("Jose Lema", body!.ClientName);
    }

    [Fact]
    public async Task PostMovement_WithdrawalGreaterThanBalance_Returns422WithSaldoNoDisponible()
    {
        await _factory.SeedKnownClientAsync(2, "Marianela Montalvo");
        await _client.PostAsJsonAsync("/accounts",
            new CreateAccountRequest("222222", AccountType.Corriente, 100, ClientId: 2));

        var response = await _client.PostAsJsonAsync("/accounts/222222/movements",
            new RegisterMovementRequest(MovementType.Retiro, 500));

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Saldo no disponible", content);
    }

    [Fact]
    public async Task GetReports_AfterMovements_ReturnsFlattenedStatementRows()
    {
        await _factory.SeedKnownClientAsync(3, "Juan Osorio");
        await _client.PostAsJsonAsync("/accounts",
            new CreateAccountRequest("333333", AccountType.Ahorros, 200, ClientId: 3));
        await _client.PostAsJsonAsync("/accounts/333333/movements",
            new RegisterMovementRequest(MovementType.Deposito, 300));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var response = await _client.GetAsync(
            $"/reports?clientId=3&startDate={today:yyyy-MM-dd}&endDate={today:yyyy-MM-dd}");

        response.EnsureSuccessStatusCode();
        var report = await response.Content.ReadFromJsonAsync<List<AccountStatementItem>>(JsonOptions);
        Assert.Single(report!);
        Assert.Equal(500, report![0].AvailableBalance);
    }
}