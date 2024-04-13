// See https://aka.ms/new-console-template for more information

using document.lib.ef;
using document.lib.migrationtool;
using document.lib.shared.Extensions;
using document.lib.shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", false, false);
        config.AddJsonFile("appsettings.Development.json", true, false);
    })
    .ConfigureServices((context, services) =>
    {
        var cfgRoot = context.Configuration;
        var config = cfgRoot.GetSection("Config");
        services.Configure<ConsoleLifetimeOptions>(opts => opts.SuppressStatusMessages = true);
        services.Configure<AppConfiguration>(config);
        services.ConfigureDocumentLibShared(
            config["DatabaseProvider"],
            config["CosmosDbConnection"],
            config["BlobServiceConnectionString"],
            config["BlobContainer"]);
        services.AddHostedService<CosmosToSqlMigration>();
        services.AddDbContext<DocumentLibContext>(opts =>
        {
            opts.UseSqlServer(config["DbConnectionString"], x => x.MigrationsAssembly("document.lib.ef"));
        });
    })
    .UseSerilog()
    .Build();


await host.StartAsync();

Console.WriteLine("\r\nStarted Host. Enter to exit!");
Console.Read();