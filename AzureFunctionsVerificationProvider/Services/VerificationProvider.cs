using Azure.Messaging.ServiceBus;
using AzureFunctionsVerificationProvider.Data.Contexts;
using AzureFunctionsVerificationProvider.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsVerificationProvider.Services
{
    public class VerificationProvider : IVerificationProvider
    {
        private readonly ILogger<VerificationProvider> _logger;
        private readonly IServiceProvider _serviceProvider;

        public VerificationProvider(ILogger<VerificationProvider> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        //EmailRequest anger vad det är för något som metoden returnera... 
        public EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(verificationRequest.Email) && !string.IsNullOrEmpty(code))
                {
                    var emailRequest = new EmailRequest()
                    {
                        To = verificationRequest.Email,
                        Subject = $"Verification Code {code}",
                        HtmlBody = $@"<html><body>
                        <h1>Test</h1>
                        <p>Please verify your account using this verification code: {code}<p>
                        </body></html>",
                        PlainText = $"Please verify your account using this verification code: {code}."
                    };

                    return emailRequest;
                }
                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.GenerateEmailRequest() :: {ex.Message}");
                return null!;
            }
        }

        public VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message)
        {
            try
            {
                var verificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(message.Body.ToString());
                if (verificationRequest != null && !string.IsNullOrEmpty(verificationRequest.Email))
                {
                    return verificationRequest;
                }
                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.UnpackVerificationReques() :: {ex.Message}");
                return null!;
            }
        }

        public async Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code)
        {
            try
            {
                //Genom att använda GetRequiredService<DataContext>() får du en instans av DataContext, som sedan lagras i variabeln context.
                //Detta gör så att du sedan kan ändra och lägga till saker i databasen...
                using var context = _serviceProvider.GetRequiredService<DataContext>();

                var existingRequest = await context.VerificationRequest.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);

                if (existingRequest != null)
                {
                    existingRequest.Code = code;
                    existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                    context.Entry(existingRequest).State = EntityState.Modified;
                }
                else
                {
                    context.VerificationRequest.Add(new Data.Entities.VerificatonRequestEntity() { Email = verificationRequest.Email, Code = code });
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.Run() :: {ex.Message}");
                return false;
            }
        }

        public string GenerateCode()
        {
            try
            {
                var random = new Random();
                var code = random.Next(1000, 9999);

                return code.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.UnpackVerificationReques() :: {ex.Message}");
                return null!;
            }
        }

        public string GenerateServiceBusEmailRequest(EmailRequest emailRequest)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(emailRequest);
                if (!string.IsNullOrEmpty(payload))
                {
                    return payload;
                }

                return null!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GenerateVerificationCode.GenerateServiceBusEmailRequest() :: {ex.Message}");
                return null!;
            }
        }
    }
}

//try
//{

//}
//catch (Exception ex)
//{
//    _logger.LogError($"GenerateVerificationCode.Run() :: {ex.Message}");
//}