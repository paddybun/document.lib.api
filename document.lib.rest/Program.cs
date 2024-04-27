using document.lib.shared.Enums;
using document.lib.shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Add sdk services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Build configuration
var appConfig = builder.Configuration.GetSection("Config").Get<AppConfiguration>();
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

// Init document lib services
builder.Services.ConfigureDocumentLibShared(
    appConfig.DatabaseProvider,
    appConfig.CosmosDbConnection,
    appConfig.BlobServiceConnectionString,
    appConfig.BlobContainer);
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