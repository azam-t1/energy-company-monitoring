using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EnergyCompanyMonitoring.Models;

public class MeterReading
{
    [Key]
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    
    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
    
    public DateTime MeterReadingDateTime { get; set; }
    
    public int MeterReadValue { get; set; }
}
