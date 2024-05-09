using AzureFunctionsVerificationProvider.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsVerificationProvider.Services
{
    public class VerificationCleanerService : IVerificationCleanerService
    {
        private readonly ILogger<VerificationCleanerService> _logger;
        private readonly DataContext _dataContext;

        public VerificationCleanerService(ILogger<VerificationCleanerService> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task RemoveExpiredRecordsAsync()
        {
            try
            {
                //Tar med alla VerificationsRequest som har ett datum innan "DateTime.Now"... Och bildar en lista som vi kan ta bort...
                var expired = await _dataContext.VerificationRequest.Where(x => x.ExpiryDate <= DateTime.Now).ToListAsync();
                //RemoveRange gör det lättare och bättre att ta bort många objekt i en lista/databas...
                _dataContext.RemoveRange(expired);
                await _dataContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError($"RemoveExpiredRecordsAsync.Run() :: {ex.Message}");
            }
        }
    }
}
