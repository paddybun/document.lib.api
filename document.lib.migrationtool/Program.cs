// See https://aka.ms/new-console-template for more information

using document.lib.shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((builder, config) =>
    {
        config.AddJsonFile("appsettings.json", false, false);
        config.AddJsonFile("appsettings.Development.json", true, false);
    })
    .ConfigureServices((builder, config) =>
    {
        var cfgRoot = builder.Configuration;
        config.Configure<AppConfiguration>(cfgRoot.GetSection("Config"));
    })
    .Build();

var cfg = host.Services.GetService<IOptions<AppConfiguration>>();
await host.StartAsync();

Console.WriteLine("Started Host. Enter to exit!");
Console.Read();