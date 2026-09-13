using Application.Abstractions;
using Application.Contracts;
using Domain;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ClientsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ClientsDb")).UseSnakeCaseNamingConvention());
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/clients",
    async (CreateClientRequest request, IClientRepository repository, IPasswordHasher hasher) =>
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("La contraseña es obligatoria");
            var passwordHash = hasher.Hash(request.Password);
            var client = new Client(request.Name, request.Gender, request.Age, request.IdentificationNumber,
                request.Address, request.PhoneNumber, passwordHash);
            await repository.AddAsync(client);
            return Results.Created($"/clients/{client.PersonId}", ToResponse(client));
        }
        catch (DbUpdateException)
        {
            return Results.Problem(detail: "Ya existe un cliente con esa identificación.", statusCode: 409, title: "Conflict");
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

app.MapPut("/clients/{id:int}", async (int id, UpdateClientRequest request, IClientRepository repository) =>
{
    try
    {
        var client = await repository.GetByIdAsync(id);
        if (client is null) return Results.NotFound();

        client.UpdateProfile(request.Name, request.Gender, request.Age, request.IdentificationNumber, request.Address,
            request.PhoneNumber);
        await repository.UpdateAsync(client);
        return Results.Ok(ToResponse(client));
    }
    catch (AppException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
    }
});

app.MapDelete("/clients/{id:int}", async (int id, IClientRepository repository) =>
{
    await repository.DeactivateAsync(id);
    return Results.NoContent();
});

app.MapPut("/clients/{id:int}/activate", async (int id, IClientRepository repository) =>
{
    await repository.ActivateAsync(id);
    return Results.NoContent();
});

ClientResponse ToResponse(Client c) => new(c.PersonId, c.Name, c.Gender, c.Age, c.IdentificationNumber, c.Address,
    c.PhoneNumber, c.IsActive)
;

app.Run();