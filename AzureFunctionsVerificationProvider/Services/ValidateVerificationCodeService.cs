using AzureFunctionsVerificationProvider.Data.Contexts;
using AzureFunctionsVerificationProvider.Functions;
using AzureFunctionsVerificationProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsVerificationProvider.Services
{
    public class ValidateVerificationCodeService : IValidateVerificationCodeService
    {
        private readonly ILogger<ValidateVerificationCode> _logger;
        private readonly DataContext _dataContext;

        public ValidateVerificationCodeService(ILogger<ValidateVerificationCode> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public async Task<ValidateRequest> UnpackValidateRequestAsync(HttpRequest req)
        {
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    var validateRequest = JsonConvert.DeserializeObject<ValidateRequest>(body);

                    if (validateRequest != null)
                        return validateRequest;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidateVerificationCode.Run() :: {ex.Message}");
            }

            return null!;

        }

        public async Task<bool> ValidateCodeAsync(ValidateRequest validateRequest)
        {
            try
            {
                //Denna del tar bort om valideringskoden om den har lyckats.
                var entity = await _dataContext.VerificationRequest.FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);
                if (entity != null)
                {
                    _dataContext.VerificationRequest.Remove(entity);
                    await _dataContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidateCodeAsync.Run() :: {ex.Message}");
                return false;
            }
        }
    }
}
