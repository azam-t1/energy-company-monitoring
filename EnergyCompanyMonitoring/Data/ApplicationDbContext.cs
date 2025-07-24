using EnergyCompanyMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyCompanyMonitoring.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<MeterReading> MeterReadings { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<MeterReading>()
            .HasIndex(m => new { m.AccountId, m.MeterReadingDateTime })
            .IsUnique();
    }
}
