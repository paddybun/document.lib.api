using document.lib.rest;
using document.lib.rest.Api.Validators;
using document.lib.shared.Enums;
using document.lib.shared.Models;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add sdk services
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = "";
    options.Audience = "https://localhost:7279/";
});

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
// builder.Services.AddAntiforgery();

// Init api services
builder.Services.AddSingleton(apiConfig);
builder.Services.AddScoped<FolderApiService>();
builder.Services.AddScoped<TagApiService>();
builder.Services.AddScoped<DocumentApiService>();
builder.Services.AddScoped<CategoryApiService>();

// Validators global
ValidatorOptions.Global.LanguageManager.Enabled = false; // Disable validation localization

// Validators folders
builder.Services.AddScoped<IValidator<FolderGetParameters>, FolderGetValidator>();
builder.Services.AddScoped<IValidator<FolderUpdateParameters>, FolderUpdateValidator>();

// Validators documents
builder.Services.AddScoped<IValidator<DocumentGetParameters>, DocumentGetValidator>();
builder.Services.AddScoped<IValidator<DocumentUpdateParameters>, DocumentUpdateValidator>();
builder.Services.AddScoped<IValidator<DocumentTagParameters>, DocumentTagsValidator>();
builder.Services.AddScoped<IValidator<DocumentMoveParameters>, DocumentMoveValidator>();

// Validators categories
builder.Services.AddScoped<IValidator<CategoryGetParameters>, CategoryGetValidator>();
builder.Services.AddScoped<IValidator<CategoryUpdateParameters>, CategoryUpdateValidator>();

// Validators tags
builder.Services.AddScoped<IValidator<TagsGetParameters>, TagGetValidator>();
builder.Services.AddScoped<IValidator<TagsUpdateParameters>, TagUpdateValidator>();

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
// app.UseAntiforgery();
app.UseFolderApi();
app.UseTagApi();
app.UseDocumentApi();
app.UseCatergoryApi();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
// app.UseAuthentication();
// app.UseAuthorization();
app.Run();