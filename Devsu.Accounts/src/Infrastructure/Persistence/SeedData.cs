using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedAsync(AccountsDbContext context)
    {
        if (await context.Accounts.AnyAsync()) return;

        var joseAhorros = new Account("478758", AccountType.Ahorros, 2000, clientId: 1, clientName: "Jose Lema");
        var marianelaCorriente = new Account("225487", AccountType.Corriente, 100, clientId: 2, clientName: "Marianela Montalvo");
        var juanAhorros = new Account("495878", AccountType.Ahorros, 0, clientId: 3, clientName: "Juan Osorio");
        var marianelaAhorros = new Account("496825", AccountType.Ahorros, 540, clientId: 2, clientName: "Marianela Montalvo");
        var joseCorriente = new Account("585545", AccountType.Corriente, 1000, clientId: 1, clientName: "Jose Lema");

        joseAhorros.RegisterWithdrawal(575);
        marianelaCorriente.RegisterDeposit(600);
        juanAhorros.RegisterDeposit(150);
        marianelaAhorros.RegisterWithdrawal(540);

        context.Accounts.AddRange(joseAhorros, marianelaCorriente, juanAhorros, marianelaAhorros, joseCorriente);
        await context.SaveChangesAsync();
    }
}