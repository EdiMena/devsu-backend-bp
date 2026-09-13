using Application.Abstractions;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(ClientsDbContext context, IPasswordHasher hasher)
    {
        if (await context.Persons.AnyAsync()) return;

        var clients = new[]
        {
            new Client("Jose Lema", Gender.M, 35, "1234567890", "Otavalo sn y principal", "098254785", hasher.Hash("1234")),
            new Client("Marianela Montalvo", Gender.F, 28, "1234567891", "Amazonas y NNUU", "097548965", hasher.Hash("5678")),
            new Client("Juan Osorio", Gender.M, 42, "1234567892", "13 junio y Equinoccial", "098874587", hasher.Hash("1245"))
        };
        
        context.Clients.AddRange(clients);
        await context.SaveChangesAsync();
    }
}