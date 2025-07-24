using CsvHelper;
using CsvHelper.Configuration;
using EnergyCompanyMonitoring.Data;
using EnergyCompanyMonitoring.DTOs;
using EnergyCompanyMonitoring.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EnergyCompanyMonitoring.Services;

public interface IMeterReadingService
{
    Task<MeterReadingUploadResultDto> ProcessMeterReadingsAsync(IFormFile file);
}

public class MeterReadingService : IMeterReadingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MeterReadingService> _logger;

    public MeterReadingService(ApplicationDbContext context, ILogger<MeterReadingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MeterReadingUploadResultDto> ProcessMeterReadingsAsync(IFormFile file)
    {
        var result = new MeterReadingUploadResultDto();
        
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            });
            
            var records = csv.GetRecords<MeterReadingDto>().ToList();
            
            // process each record
            foreach (var record in records)
            {
                try
                {
                    if (!IsValidMeterReading(record, out string validationError))
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Invalid reading for account {record.AccountId}: {validationError}");
                        continue;
                    }
                    
                    var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountId == record.AccountId);
                    if (account == null)
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Account {record.AccountId} does not exist");
                        continue;
                    }
                    
                    // Use specific date format for parsing with various formats
                    DateTime readingDate;
                    if (!DateTime.TryParseExact(record.MeterReadingDateTime, 
                                               new[] { 
                                                   "dd/MM/yyyy HH:mm",      // format for "24/5/2019 22:24" 
                                                   "d/M/yyyy H:mm",         // format for "6/5/2019 9:24"
                                                   "M/d/yyyy H:mm",         // format for "12/5/2019 9:24"
                                                   "MM/dd/yyyy HH:mm",      // format for "12/15/2019 15:24"
                                                   "yyyy-MM-dd HH:mm:ss"    // format for "2019-12-26 19:30:45"
                                               }, 
                                               CultureInfo.InvariantCulture,
                                               DateTimeStyles.None, 
                                               out readingDate))
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Invalid date format for account {record.AccountId}. Expected format: dd/MM/yyyy HH:mm or d/M/yyyy H:mm");
                        continue;
                    }
                    
                    if (!int.TryParse(record.MeterReadValue, out int readingValue))
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Invalid meter reading value for account {record.AccountId}");
                        continue;
                    }
                    
                    var existingReading = await _context.MeterReadings
                        .FirstOrDefaultAsync(m => m.AccountId == record.AccountId && m.MeterReadingDateTime == readingDate);
                    
                    if (existingReading != null)
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Duplicate reading for account {record.AccountId} at {readingDate}");
                        continue;
                    }
                    
                    var latestReading = await _context.MeterReadings
                        .Where(m => m.AccountId == record.AccountId)
                        .OrderByDescending(m => m.MeterReadingDateTime)
                        .FirstOrDefaultAsync();
                    
                    if (latestReading != null && readingDate < latestReading.MeterReadingDateTime)
                    {
                        result.FailedReadings++;
                        result.Errors.Add($"Reading date {readingDate} for account {record.AccountId} is older than existing reading {latestReading.MeterReadingDateTime}");
                        continue;
                    }
                    
                    var meterReading = new MeterReading
                    {
                        AccountId = account.Id,
                        MeterReadingDateTime = readingDate,
                        MeterReadValue = readingValue
                    };
                    
                    await _context.MeterReadings.AddAsync(meterReading);
                    result.SuccessfulReadings++;
                }
                catch (Exception ex)
                {
                    result.FailedReadings++;
                    result.Errors.Add($"Error processing reading for account {record.AccountId}: {ex.Message}");
                    _logger.LogError(ex, "Error processing meter reading for account {AccountId}", record.AccountId);
                }
            }
            
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing meter readings file");
            result.Errors.Add($"Error processing file: {ex.Message}");
        }
        
        return result;
    }
    
    private bool IsValidMeterReading(MeterReadingDto reading, out string error)
    {
        error = string.Empty;
        
        // check if account id is valid
        if (reading.AccountId <= 0)
        {
            error = "Invalid account ID";
            return false;
        }
        
        // check if meter reading value is in the NNNNN format (up to 5 digits)
        if (string.IsNullOrWhiteSpace(reading.MeterReadValue))
        {
            error = "Meter reading value is required";
            return false;
        }
        
        if (!Regex.IsMatch(reading.MeterReadValue, @"^\d{1,5}$"))
        {
            error = "Meter reading value must be in NNNNN format (up to 5 digits)";
            return false;
        }
        
        return true;
    }
}
