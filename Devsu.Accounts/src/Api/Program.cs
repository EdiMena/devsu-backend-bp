using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AccountsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AccountsDb")).UseSnakeCaseNamingConvention());
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IMovementRepository, MovementRepository>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();