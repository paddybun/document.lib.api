using document.lib.shared.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

var cosmosDbConnectionString = builder.Configuration.GetValue<string>("CosmosDbConnection");
var blobConnectionString = builder.Configuration.GetValue<string>("BlobContainerConnectionString");
var blobContainer = builder.Configuration.GetValue<string>("BlobContainer");
var queryService = new QueryService(cosmosDbConnectionString);
var metadataService = new MetadataService(cosmosDbConnectionString);
var blobClientHelper = new BlobClientHelper(blobConnectionString,blobContainer);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
var docLibService = new DocLibService(blobConnectionString, blobContainer,cosmosDbConnectionString);

builder.Services.AddSingleton(docLibService);
builder.Services.AddSingleton(queryService);
builder.Services.AddSingleton(metadataService);
builder.Services.AddSingleton<IndexerService>();
builder.Services.AddSingleton(blobClientHelper);

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
