using Application.Abstractions;
using Application.Contracts;
using Domain;
using Domain.Enums;
using Domain.Exceptions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AccountsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountsDb")).UseSnakeCaseNamingConvention());
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IMovementRepository, MovementRepository>();
builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/accounts", async (CreateAccountRequest request, IAccountRepository repository) =>
{
    try
    {
        var account = new Account(request.AccountNumber, request.AccountType, request.InitialBalance, request.ClientId,
            request.ClientName);
        await repository.AddAsync(account);
        return Results.Created($"/accounts/{account.AccountNumber}", ToAccountResponse(account));
    }
    catch (DbUpdateException)
    {
        return Results.Problem(detail: "Ya existe una cuenta con ese número", statusCode: 409, title: "Conflict");
    }
    catch (AppException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
    }
});

app.MapGet("/accounts/{accountNumber}", async (string accountNumber, IAccountRepository repository) =>
{
    var account = await repository.GetByNumberAsync(accountNumber);
    return account is null ? Results.NotFound() : Results.Ok(ToAccountResponse(account));
});

app.MapGet("/accounts", async (IAccountRepository repository) =>
{
    var accounts = await repository.GetAllAsync();
    return Results.Ok(accounts.Select(ToAccountResponse));
});

app.MapPut("/accounts/{accountNumber}",
    async (string accountNumber, UpdateAccountRequest request, IAccountRepository repository) =>
    {
        var account = await repository.GetByNumberAsync(accountNumber);
        if (account is null) return Results.NotFound();

        account.ChangeAccountType(request.AccountType);
        await repository.UpdateAsync(account);
        return Results.Ok(ToAccountResponse(account));
    });

app.MapPut("/accounts/{accountNumber}/deactivate", async (string accountNumber, IAccountRepository repository) =>
{
    var account = await repository.GetByNumberAsync(accountNumber);
    if (account is null) return Results.NotFound();
    account.Deactivate();
    await repository.UpdateAsync(account);
    return Results.NoContent();
});

app.MapPut("/accounts/{accountNumber}/activate", async (string accountNumber, IAccountRepository repository) =>
{
    var account = await repository.GetByNumberAsync(accountNumber);
    if (account is null) return Results.NotFound();
    account.Activate();
    await repository.UpdateAsync(account);
    return Results.NoContent();
});

app.MapPost("/accounts/{accountNumber}/movements",
    async (string accountNumber, RegisterMovementRequest request, IAccountRepository repository) =>
    {
        try
        {
            var account = await repository.GetByNumberAsync(accountNumber);
            if (account is null) return Results.NotFound();

            var movement = request.MovementType == MovementType.Deposito
                ? account.RegisterDeposit(request.Amount)
                : account.RegisterWithdrawal(request.Amount);

            await repository.UpdateAsync(account);
            return Results.Created($"/movements/{movement.MovementId}", ToMovementResponse(movement));
        }
        catch (AppException ex)
        {
            return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
        }
    });

app.MapGet("/movements/{movementId:int}", async (int movementId, IMovementRepository repository) =>
{
    var movement = await repository.GetByIdAsync(movementId);
    return movement is null ? Results.NotFound() : Results.Ok(ToMovementResponse(movement));
});

app.MapGet("/accounts/{accountNumber}/movements", async (string accountNumber, IMovementRepository repository) =>
{
    var movements = await repository.GetByAccountAsync(accountNumber);
    return Results.Ok(movements.Select(ToMovementResponse));
});

app.MapPut("/movements/{movementId:int}", async (int movementId, CorrectMovementRequest request,
    IMovementRepository movementRepository, IAccountRepository accountRepository) =>
{
    try
    {
        var original = await movementRepository.GetByIdAsync(request.OriginalMovementId);
        if (original is null) return Results.NotFound();

        var account = await accountRepository.GetByNumberAsync(original.AccountNumber);
        if (account is null) return Results.NotFound();

        account.CorrectMovement(original, request.CorrectMovementType, request.CorrectAmount);
        await accountRepository.UpdateAsync(account);
        return Results.Ok(ToAccountResponse(account));
    }
    catch (AppException ex)
    {
        return Results.Problem(detail: ex.Message, statusCode: ex.StatusCode, title: ex.GetType().Name);
    }
});

AccountResponse ToAccountResponse(Account a) => new(a.AccountNumber, a.AccountType, a.InitialBalance,
    a.AvailableBalance,
    a.IsActive, a.ClientId, a.ClientName);

MovementResponse ToMovementResponse(Movement m) =>
    new(m.MovementId, m.MovementDate, m.MovementType, m.Amount, m.Balance, m.AccountNumber);

app.MapPost("/seed", async (AccountsDbContext context) =>
{
    await SeedData.SeedAsync(context);
    return Results.Ok("Seed ejecutado.");
});

app.Run();