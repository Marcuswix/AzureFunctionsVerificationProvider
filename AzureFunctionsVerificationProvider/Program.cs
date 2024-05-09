using AzureFunctionsVerificationProvider.Data.Contexts;
using AzureFunctionsVerificationProvider.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddDbContext<DataContext>(x => x.UseSqlServer(Environment.GetEnvironmentVariable("VerificationRequestDataBase")));
        services.AddScoped<VerificationProvider>();
    })
    .Build();

//Denna del g�r s� att vi kommer �t connectionstring utan att programmet har startat och l�gger till tabellerna i din databas
//... Varje g�ng programmet starat s� startar en migrering...

using (var scope = host.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        var migrations = context.Database.GetPendingMigrations();
        if (migrations != null && migrations.Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine("Program.cs" + ex.Message);
    }

}

host.Run();
