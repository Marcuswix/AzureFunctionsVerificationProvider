using AzureFunctionsVerificationProvider.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsVerificationProvider.Functions
{
    public class ValidateVerificationCode
    {
        private readonly IValidateVerificationCodeService _validateVerificationCodeService;
        private readonly ILogger<ValidateVerificationCode> _logger;

        public ValidateVerificationCode(IValidateVerificationCodeService validateVerificationCodeService, ILogger<ValidateVerificationCode> logger)
        {
            _validateVerificationCodeService = validateVerificationCodeService;
            _logger = logger;
        }

        [Function("ValidateVerificationCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "verify")] HttpRequest req)
        {
            try
            {
                var validateRequest = await _validateVerificationCodeService.UnpackValidateRequestAsync(req);
                if (validateRequest != null)
                {
                    bool validateResult = await _validateVerificationCodeService.ValidateCodeAsync(validateRequest);
                    if(validateResult == true)
                    {
                        return new OkResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"ValidateVerificationCode.Run() :: {ex.Message}");
            }

            return new UnauthorizedResult();
        }
    }
}
