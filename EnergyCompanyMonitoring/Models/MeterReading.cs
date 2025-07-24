using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyCompanyMonitoring.Models;

public class MeterReading
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Account))]
    public int AccountId { get; set; }
    
    public Account? Account { get; set; }
    
    public DateTime MeterReadingDateTime { get; set; }
    
    public int MeterReadValue { get; set; }
}
