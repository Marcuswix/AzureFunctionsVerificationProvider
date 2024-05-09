using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionsVerificationProvider.Data.Entities
{
    public class VerificatonRequestEntity
    {
        [Key]
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        //Detta skapar hur länge denna verifieringskod ska vara giltlig
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMinutes(5);
    }
}
