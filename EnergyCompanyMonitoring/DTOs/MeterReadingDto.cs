namespace EnergyCompanyMonitoring.DTOs;

public class MeterReadingDto
{
    public int AccountId { get; set; }
    public string MeterReadingDateTime { get; set; } = string.Empty;
    public string MeterReadValue { get; set; } = string.Empty;
}

public class MeterReadingUploadResultDto
{
    public int SuccessfulReadings { get; set; }
    public int FailedReadings { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
}
