using Azure.Messaging.ServiceBus;
using AzureFunctionsVerificationProvider.Models;

namespace AzureFunctionsVerificationProvider.Services
{
    public interface IVerificationProvider
    {
        string GenerateCode();
        EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code);
        string GenerateServiceBusEmailRequest(EmailRequest emailRequest);
        Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code);
        VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message);
    }
}