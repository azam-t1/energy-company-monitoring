using EnergyCompanyMonitoring.DTOs;
using EnergyCompanyMonitoring.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnergyCompanyMonitoring.Controllers;

[ApiController]
[Route("meter-reading-uploads")]
public class MeterReadingUploadsController : ControllerBase
{
    private readonly IMeterReadingService _meterReadingService;
    private readonly ILogger<MeterReadingUploadsController> _logger;

    public MeterReadingUploadsController(
        IMeterReadingService meterReadingService, 
        ILogger<MeterReadingUploadsController> logger)
    {
        _meterReadingService = meterReadingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadMeterReadings(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file was uploaded.");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only CSV files are supported.");
        }

        try
        {
            var result = await _meterReadingService.ProcessMeterReadingsAsync(file);
            
            return Ok(new
            {
                result.SuccessfulReadings,
                result.FailedReadings,
                result.Errors
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing meter reading upload");
            return StatusCode(500, "An error occurred while processing the uploaded file.");
        }
    }
}
