using AzureFunctionsVerificationProvider.Models;
using Microsoft.AspNetCore.Http;

namespace AzureFunctionsVerificationProvider.Services
{
    public interface IValidateVerificationCodeService
    {
        Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateRequest validateRequest);
    }
}