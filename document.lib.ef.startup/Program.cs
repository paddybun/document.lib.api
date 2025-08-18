using document.lib.data.context;
using document.lib.ef.startup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((builder, config) =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddJsonFile("appsettings.Development.json");
    })
    .ConfigureServices((builder, services) =>
    {
        services.AddHostedService<StartupService>();
        services.AddDbContext<DatabaseContext>(config =>
        {
            config.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
    });

var app = builder.Build();
app.Run();