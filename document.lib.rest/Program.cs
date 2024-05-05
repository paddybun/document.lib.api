using document.lib.rest.Api.Validators;
using document.lib.shared.Enums;
using document.lib.shared.Models;
using FluentValidation;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add sdk services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Build configuration
var appConfig = builder.Configuration.GetSection("Config").Get<SharedConfig>();
if (appConfig == null)
{
    throw new Exception("Config section not found!");
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Init api services
builder.Services.AddScoped<FolderApiService>();
builder.Services.AddScoped<TagApiService>();
builder.Services.AddScoped<DocumentApiService>();

// Validators
ValidatorOptions.Global.LanguageManager.Enabled = false; // Disable localization
builder.Services.AddScoped<IValidator<FolderPutParameters>, FolderPutValidator>();

// Init document lib services
builder.Services.ConfigureDocumentLibShared(builder.Configuration.GetSection("Config"));
if (appConfig.DatabaseProvider == DatabaseProvider.Sql)
{
    builder.Services.AddDbContext<DocumentLibContext>(opts =>
    {
        opts.UseSqlServer(appConfig.DbConnectionString, x => x.MigrationsAssembly("document.lib.ef"));
    });    
}

// Build app and configure the HTTP request pipeline.
var app = builder.Build();

// Add apis
app.AddFolderApi();
app.AddTagApi();
app.AddDocumentApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();