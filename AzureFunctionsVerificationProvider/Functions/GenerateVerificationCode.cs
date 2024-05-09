using Azure.Messaging.ServiceBus;
using AzureFunctionsVerificationProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsVerificationProvider.Functions
{
    public class GenerateVerificationCode
    {
        private readonly ILogger<GenerateVerificationCode> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IVerificationProvider _verificationProvider;


        public GenerateVerificationCode(ILogger<GenerateVerificationCode> logger, IServiceProvider serviceProvider, IVerificationProvider verificationProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _verificationProvider = verificationProvider;
        }

        [Function(nameof(GenerateVerificationCode))]
        [ServiceBusOutput("verification_request", Connection = "ServiceBusConnection")]
        public async Task<string> Run(
            [ServiceBusTrigger("verification_request", Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            try
            {
                var verificationRequest = _verificationProvider.UnpackVerificationRequest(message);
                if (verificationRequest != null)
                {
                    var code = _verificationProvider.GenerateCode();
                    if (!string.IsNullOrEmpty(code))
                    {
                        var result = await _verificationProvider.SaveVerificationRequest(verificationRequest, code);

                        if (result == true)
                        {
                            var emailRequest = _verificationProvider.GenerateEmailRequest(verificationRequest, code);
                            if (emailRequest != null)
                            {
                                var payload = _verificationProvider.GenerateServiceBusEmailRequest(emailRequest);
                                if (!string.IsNullOrEmpty(payload))
                                {
                                    await messageActions.CompleteMessageAsync(message);
                                    return payload;
                                }
                            }
                        }
                    }
                }

                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.Run() :: {ex.Message}");
                return null!;
            }
        }
    }
}

       
