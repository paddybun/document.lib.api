using document.lib.rest;
using document.lib.rest.Api.Validators;
using document.lib.shared.Enums;
using document.lib.shared.Models;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add sdk services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Build configuration
var sharedConfigSection = builder.Configuration.GetSection("Config"); 
var sharedConfig = sharedConfigSection.Get<SharedConfig>();
var apiConfig = builder.Configuration.GetSection("ApiConfig").Get<ApiConfig>();
if (sharedConfig == null || apiConfig == null)
{
    throw new Exception("Required config section not found!");
}

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Init api services
builder.Services.AddSingleton<ApiConfig>();
builder.Services.AddScoped<FolderApiService>();
builder.Services.AddScoped<TagApiService>();
builder.Services.AddScoped<DocumentApiService>();

// Validators
ValidatorOptions.Global.LanguageManager.Enabled = false; // Disable localization
builder.Services.AddScoped<IValidator<FolderPutParameters>, FolderPutValidator>();
builder.Services.AddScoped<IValidator<DocumentUpdateParameters>, DocumentPostValidator>();
builder.Services.AddScoped<IValidator<DocumentTagsParameters>, DocumentTagsValidator>();

// Init document lib services
builder.Services.UseDocumentLibShared(sharedConfigSection);
if (sharedConfig.DatabaseProvider == DatabaseProvider.Sql)
{
    builder.Services.AddDbContext<DocumentLibContext>(opts =>
    {
        opts.UseSqlServer(sharedConfig.DbConnectionString, x => x.MigrationsAssembly("document.lib.ef"));
    });    
}

// Build app and configure the HTTP request pipeline.
var app = builder.Build();

// Add apis
app.UseFolderApi();
app.UseTagApi();
app.UseDocumentApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.Run();