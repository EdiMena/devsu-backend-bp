using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Contracts;
using Domain.Enums;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.IntegrationTests;

public class ClientsApiTests : IClassFixture<ClientsApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new ()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
    private readonly ClientsApiFactory _factory;
    private readonly HttpClient _client;

    public ClientsApiTests(ClientsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostClients_WithValidData_Returns201AndPublishesClientCreated()
    {
        var request = new CreateClientRequest("Jose Lema", Gender.M, 35, "1234567890", "Otavalo", "0999999999",
            "Password123");

        var response = await _client.PostAsJsonAsync("/clients", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ClientResponse>(JsonOptions);
        Assert.Equal("Jose Lema", body!.Name);

        var harness = _factory.Services.GetRequiredService<ITestHarness>();
        Assert.True(await harness.Published.Any<Contracts.ClientCreated>());
    }

    [Fact]
    public async Task PostClients_WithInvalidAge_Returns400()
    {
        var request = new CreateClientRequest("Jose Lema", Gender.M, 0, "1234567891", "Otavalo", "0999999999",
            "Password123");

        var response = await _client.PostAsJsonAsync("/clients", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetClients_AfterCreatingOne_ReturnsItInTheList()
    {
        var request = new CreateClientRequest("Marianela Montalvo", Gender.F, 28, "1234567899", "Amazonas",
            "0988888888", "Password123");
        await _client.PostAsJsonAsync("/clients", request);

        var response = await _client.GetAsync("/clients");

        response.EnsureSuccessStatusCode();
        var clients = await response.Content.ReadFromJsonAsync<List<ClientResponse>>(JsonOptions);
        Assert.Contains(clients!, c => c.IdentificationNumber == "1234567899");
    }
}