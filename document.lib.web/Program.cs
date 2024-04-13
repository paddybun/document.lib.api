using System.Globalization;
using Azure.Storage.Blobs;
using document.lib.ef;
using document.lib.shared.Extensions;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Repositories.Cosmos;
using document.lib.shared.Repositories.Sql;
using document.lib.shared.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy
    options.FallbackPolicy = options.DefaultPolicy;
});

CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();

// ----- Dependency Injection -----
var config = builder.Configuration.GetSection("Config");
builder.Services.Configure<AppConfiguration>(config);

builder.Services.ConfigureDocumentLibShared(
    config["DatabaseProvider"],
    config["CosmosDbConnection"],
    config["BlobServiceConnectionString"],
    config["BlobContainer"]);

var provider = config["DatabaseProvider"];
switch (provider)
{
    case "Sql":
        builder.Services.AddDbContext<DocumentLibContext>(opts =>
        {
            opts.UseSqlServer(config["DbConnectionString"], x => x.MigrationsAssembly("document.lib.ef"));
        });
        break;
}



var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var setup = app.Services.GetService<OneTimeSetup>();
await setup!.CreateDefaultsAsync();

app.Run();
