using Application.Abstractions;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ClientsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ClientsDb")).UseSnakeCaseNamingConvention());
builder.Services.AddScoped<IClientRepository, ClientRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
