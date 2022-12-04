using System.Globalization;
using document.lib.shared;
using document.lib.shared.Interfaces;
using document.lib.shared.Models;
using document.lib.shared.Repositories;
using document.lib.shared.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
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
builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("Config"));

// Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IFolderRepository, FolderRepository>();


// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFolderService,FolderService>();

builder.Services.AddScoped(typeof(QueryService));
builder.Services.AddScoped(typeof(MetadataService));
builder.Services.AddSingleton<IndexerService>();
builder.Services.AddSingleton(typeof(BlobClientHelper));

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

app.Run();
