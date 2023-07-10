using System.Globalization;
using Azure.Storage.Blobs;
using document.lib.ef;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Repositories;
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
builder.Services.AddDbContext<DocumentLibContext>(opts =>
{
    opts.UseSqlServer(config["DbConnectionString"], x => x.MigrationsAssembly("document.lib.ef"));
});

// Repositories
var provider = config["Provider"];
switch (provider)
{
    case "sql":
        builder.Services.AddScoped<IDocumentRepository, DocumentSqlRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategorySqlRepository>();
        builder.Services.AddScoped<ITagRepository, TagSqlRepository>();
        builder.Services.AddScoped<IFolderRepository, FolderSqlRepository>();
        break;
    case "cosmos":
        builder.Services.AddScoped<IDocumentRepository, DocumentCosmosRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryCosmosRepository>();
        builder.Services.AddScoped<ITagRepository, TagCosmosRepository>();
        builder.Services.AddScoped<IFolderRepository, FolderCosmosRepository>();
        break;
}

// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFolderService,FolderService>();
builder.Services.AddTransient<OneTimeSetup>();

builder.Services.AddScoped(typeof(QueryService));
builder.Services.AddScoped(typeof(MetadataService));
builder.Services.AddSingleton<IndexerService>();
builder.Services.AddSingleton(typeof(BlobClientHelper));

var cosmosClient = new CosmosClient(config["CosmosDbConnection"]);
builder.Services.AddSingleton(cosmosClient);

var blobContainerClient = new BlobContainerClient(config["BlobServiceConnectionString"], config["BlobContainer"]);
builder.Services.AddSingleton(blobContainerClient);

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
await setup.CreateDefaultsAsync();

app.Run();
