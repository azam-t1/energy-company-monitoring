using EnergyCompanyMonitoring.Data;
using EnergyCompanyMonitoring.Models;
using EnergyCompanyMonitoring.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace EnergyCompanyMonitoring.Tests
{
    [TestFixture]
    public class MeterReadingServiceTests
    {
        private DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private ApplicationDbContext _dbContext;
        private Mock<ILogger<MeterReadingService>> _mockLogger;
        private MeterReadingService _meterReadingService;

        [SetUp]
        public void Setup()
        {
            // setup in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"MeterReadingTestDb_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(_dbContextOptions);
            _mockLogger = new Mock<ILogger<MeterReadingService>>();
            _meterReadingService = new MeterReadingService(_dbContext, _mockLogger.Object);

            // seed test data
            SeedTestData();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        private void SeedTestData()
        {
            // add test accounts
            var accounts = new[]
            {
                new Account { Id = 1, AccountId = 2344, FirstName = "Tommy", LastName = "Test" },
                new Account { Id = 2, AccountId = 2233, FirstName = "Barry", LastName = "Test" },
                new Account { Id = 3, AccountId = 8766, FirstName = "Sally", LastName = "Test" }
            };

            _dbContext.Accounts.AddRange(accounts);

            // add existing meter readings for testing duplicate prevention
            var existingReading = new MeterReading
            {
                Id = 1,
                AccountId = 1, // For account 2344
                MeterReadingDateTime = DateTime.ParseExact("22/04/2019 12:25", 
                    "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture),
                MeterReadValue = 1002
            };

            _dbContext.MeterReadings.Add(existingReading);
            _dbContext.SaveChanges();
        }

        private IFormFile CreateMockCsvFile(string csvContent)
        {
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            var stream = new MemoryStream(bytes);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.FileName).Returns("test.csv");
            mockFile.Setup(f => f.Length).Returns(bytes.Length);

            return mockFile.Object;
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_ValidReading_ShouldAddReadingToDatabase()
        {
            // Arrange
            var csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
2233,22/04/2019 12:25,45522";
            var formFile = CreateMockCsvFile(csvContent);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(formFile);

            // Assert
            Assert.That(result.SuccessfulReadings, Is.EqualTo(1));
            Assert.That(result.FailedReadings, Is.EqualTo(0));
            
            // Verify reading was added to database
            var addedReading = await _dbContext.MeterReadings
                .FirstOrDefaultAsync(m => m.MeterReadValue == 45522);
            Assert.That(addedReading, Is.Not.Null);
            Assert.That(addedReading.AccountId, Is.EqualTo(2)); // Account ID for 2233
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_InvalidAccountId_ShouldFailReading()
        {
            // Arrange
            var csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
9999,22/04/2019 12:25,45522";
            var formFile = CreateMockCsvFile(csvContent);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(formFile);

            // Assert
            Assert.That(result.SuccessfulReadings, Is.EqualTo(0));
            Assert.That(result.FailedReadings, Is.EqualTo(1));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("does not exist"));
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_ReadingTooLarge_ShouldFailReading()
        {
            // Arrange
            var csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
2233,22/04/2019 12:25,123456";
            var formFile = CreateMockCsvFile(csvContent);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(formFile);

            // Assert
            Assert.That(result.SuccessfulReadings, Is.EqualTo(0));
            Assert.That(result.FailedReadings, Is.EqualTo(1));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
            Assert.That(result.Errors[0], Does.Contain("NNNNN format"));
        }
        
        [Test]
        public async Task ProcessMeterReadingsAsync_FlexibleDateFormat_ShouldAcceptReading()
        {
            // Arrange - test with the format "6/5/2019 9:24"
            var csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
8766,6/5/2019 9:24,12345";
            var formFile = CreateMockCsvFile(csvContent);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(formFile);

            // Assert
            Assert.That(result.SuccessfulReadings, Is.EqualTo(1));
            Assert.That(result.FailedReadings, Is.EqualTo(0));
            
            // Verify reading was added to database with correct date
            var addedReading = await _dbContext.MeterReadings
                .FirstOrDefaultAsync(m => m.MeterReadValue == 12345);
            Assert.That(addedReading, Is.Not.Null);
            Assert.That(addedReading.MeterReadingDateTime.Day, Is.EqualTo(6));
            Assert.That(addedReading.MeterReadingDateTime.Month, Is.EqualTo(5));
            Assert.That(addedReading.MeterReadingDateTime.Year, Is.EqualTo(2019));
            Assert.That(addedReading.MeterReadingDateTime.Hour, Is.EqualTo(9));
            Assert.That(addedReading.MeterReadingDateTime.Minute, Is.EqualTo(24));
        }

        [Test]
        public async Task ProcessMeterReadingsAsync_MultipleReadings_ShouldProcessAll()
        {
            // Arrange
            var csvContent = @"AccountId,MeterReadingDateTime,MeterReadValue
2233,22/04/2019 12:25,45522
8766,6/5/2019 9:24,12345
9999,22/04/2019 12:25,45522";  // Invalid account ID
            var formFile = CreateMockCsvFile(csvContent);

            // Act
            var result = await _meterReadingService.ProcessMeterReadingsAsync(formFile);

            // Assert
            Assert.That(result.SuccessfulReadings, Is.EqualTo(2));
            Assert.That(result.FailedReadings, Is.EqualTo(1));
            Assert.That(result.Errors.Count, Is.EqualTo(1));
        }
    }
}
