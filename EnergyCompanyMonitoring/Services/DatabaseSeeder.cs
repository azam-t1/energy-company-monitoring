using CsvHelper;
using CsvHelper.Configuration;
using EnergyCompanyMonitoring.Data;
using EnergyCompanyMonitoring.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EnergyCompanyMonitoring.Services;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider, IWebHostEnvironment env)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            // ensure the database is created and migrations are applied
            await context.Database.MigrateAsync();
            
            // check if any accounts are already seeded
            if (!await context.Accounts.AnyAsync())
            {
                logger.LogInformation("Seeding accounts from CSV file...");
                await SeedAccountsFromCsvAsync(context, env);
                logger.LogInformation("Accounts seeded successfully.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
    
    private static async Task SeedAccountsFromCsvAsync(ApplicationDbContext context, IWebHostEnvironment env)
    {
        var filePath = Path.Combine(env.ContentRootPath, "SeedData", "Test_accounts.csv");
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Test accounts CSV file not found.", filePath);
        }
        
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        });
        
        var records = csv.GetRecords<AccountCsvRecord>().ToList();
        
        foreach (var record in records)
        {
            // Clean up the single quotes from the names
            var firstName = record.FirstName.Replace("'", "");
            var lastName = record.LastName.Replace("'", "");
            
            var account = new Account
            {
                AccountId = record.AccountId,
                FirstName = firstName,
                LastName = lastName
            };
            
            await context.Accounts.AddAsync(account);
        }
        
        await context.SaveChangesAsync();
    }
    
    private class AccountCsvRecord
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
