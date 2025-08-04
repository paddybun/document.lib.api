using document.lib.bl.shared;
using document.lib.bl.shared.Folders;
using document.lib.data.context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace document.lib.bl.tests;

public abstract class DatabaseTest
{
    protected readonly DatabaseContext DatabaseContext;
    protected readonly ServiceProvider ServiceProvider;
    
    internal DatabaseTest()
    {
        // Load the configuration from appsettings.json with configuration builder
        var cfgRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = cfgRoot["DbConnectionString"];
        
        // Add service collection and configuration
        var services = new ServiceCollection();
        services.AddDbContext<DatabaseContext>(opts =>
        {
            opts.UseSqlServer(connectionString, x => x.MigrationsAssembly("document.lib.data.context"));
        });
        services.AddBusinessShared();
        var sc = services.BuildServiceProvider();
        DatabaseContext = sc.GetRequiredService<DatabaseContext>();
        ServiceProvider = sc;
    }
}

public class IntegrationTests: DatabaseTest
{

    [Fact]
    public async Task Test()
    {
        var sut = new GetRegisterUseCase(new NullLogger<GetRegisterUseCase>(), DatabaseContext,
            new NextDescriptionQuery(new NullLogger<NextDescriptionQuery>(), DatabaseContext));
        var res = await sut.ExecuteAsync(10);
    }
    
}