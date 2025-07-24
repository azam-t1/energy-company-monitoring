using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyCompanyMonitoring.Models;

public class MeterReading
{
    [Key]
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    
    [ForeignKey("Account")]
    public int AccountEntityId { get; set; }
    
    public Account? Account { get; set; }
    
    public DateTime MeterReadingDateTime { get; set; }
    
    public int MeterReadValue { get; set; }
}
