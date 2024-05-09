using System;
using AzureFunctionsVerificationProvider.Data.Contexts;
using AzureFunctionsVerificationProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsVerificationProvider.Functions
{
    public class VerificationCleaner
    {
        private readonly ILogger _logger;
        private readonly IVerificationCleanerService _verificationCleanerService;

        public VerificationCleaner(ILoggerFactory loggerFactory, IVerificationCleanerService verificationCleanerService)
        {
            _logger = loggerFactory.CreateLogger<VerificationCleaner>();
            _verificationCleanerService = verificationCleanerService;
        }

        [Function("VerificationCleaner")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            try
            {
                await _verificationCleanerService.RemoveExpiredRecordsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.Run() :: {ex.Message}");
            }
        }
    }
}
