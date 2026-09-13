using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AccountsDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("AccountsDb")).UseSnakeCaseNamingConvention());
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
