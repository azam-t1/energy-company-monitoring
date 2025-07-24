using System.ComponentModel.DataAnnotations;

namespace EnergyCompanyMonitoring.Models;

public class Account
{
    [Key]
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    
    public string FirstName { get; set; } = string.Empty;
    
    public string LastName { get; set; } = string.Empty;
    
    public ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();
}
