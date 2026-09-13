using Application.Abstractions;
using Application.Contracts;
using Contracts;
using Domain;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<ClientsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ClientsDb")).UseSnakeCaseNamingConvention());
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMq:User"] ?? "guest");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "guest");
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/clients",
    async (CreateClientRequest request, IClientRepository repository, IPasswordHasher hasher,
        IPublishEndpoint publishEndpoint) =>
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("La contraseña es obligatoria");
            var passwordHash = hasher.Hash(request.Password);
            var client = new Client(request.Name, request.Gender, request.Age, request.IdentificationNumber,
                request.Address, request.PhoneNumber, passwordHash);
            await repository.AddAsync(client);
            await publishEndpoint.Publish(new ClientCreated(client.PersonId, client.Name, client.IsActive));
            return Results.Created($"/clients/{client.PersonId}", ToResponse(client));
        }
        catch (DbUpdateException)
        {
            return Results.Problem(detail: "Ya existe un cliente con esa identificación.", statusCode: 409,
                title: "Conflict");
        }
        catch (AppException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
        }
    });

app.MapGet("/clients/{id:int}", async (int id, IClientRepository repository) =>
{
    var client = await repository.GetByIdAsync(id);
    return client is null ? Results.NotFound() : Results.Ok(ToResponse(client));
});

app.MapGet("/clients", async (IClientRepository repository) =>
{
    var clients = await repository.GetAllAsync();
    return Results.Ok(clients.Select(ToResponse));
});

app.MapPut("/clients/{id:int}",
    async (int id, UpdateClientRequest request, IClientRepository repository, IPublishEndpoint publishEndpoint) =>
    {
        try
        {
            var client = await repository.GetByIdAsync(id);
            if (client is null) return Results.NotFound();

            client.UpdateProfile(request.Name, request.Gender, request.Age, request.IdentificationNumber,
                request.Address,
                request.PhoneNumber);
            await publishEndpoint.Publish(new ClientCreated(client.PersonId, client.Name, client.IsActive));
            await repository.UpdateAsync(client);
            return Results.Ok(ToResponse(client));
        }
        catch (AppException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
        }
    });

app.MapDelete("/clients/{id:int}", async (int id, IClientRepository repository, IPublishEndpoint publishEndpoint) =>
{
    var client = await repository.GetByIdAsync(id);
    if (client is null) return Results.NotFound();

    await repository.DeactivateAsync(id);
    await publishEndpoint.Publish(new ClientCreated(client.PersonId, client.Name, false));
    return Results.NoContent();
});

app.MapPut("/clients/{id:int}/activate",
    async (int id, IClientRepository repository, IPublishEndpoint publishEndpoint) =>
    {
        var client = await repository.GetByIdAsync(id);
        if (client is null) return Results.NotFound();

        await repository.ActivateAsync(id);
        await publishEndpoint.Publish(new ClientCreated(client.PersonId, client.Name, true));
        return Results.NoContent();
    });

ClientResponse ToResponse(Client c) => new(c.PersonId, c.Name, c.Gender, c.Age, c.IdentificationNumber, c.Address,
    c.PhoneNumber, c.IsActive)
;

app.MapPost("/seed", async (ClientsDbContext context, IPasswordHasher hasher) =>
{
    await SeedData.SeedAsync(context, hasher);
    return Results.Ok("Seed ejecutado.");
});

app.Run();