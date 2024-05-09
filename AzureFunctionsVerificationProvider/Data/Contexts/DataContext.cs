using AzureFunctionsVerificationProvider.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureFunctionsVerificationProvider.Data.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<VerificatonRequestEntity> VerificationRequest {  get; set; }
    }
}
