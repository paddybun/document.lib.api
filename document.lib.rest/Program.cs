using document.lib.ef;
using document.lib.rest;
using document.lib.rest.ApiServices;
using document.lib.shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

var config = builder.Configuration.GetSection("Config");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Init folder services
builder.Services.AddScoped<FolderApiService>();


// Init document lib services
builder.Services.ConfigureDocumentLibShared(
    config["DatabaseProvider"],
    config["CosmosDbConnection"],
    config["BlobServiceConnectionString"],
    config["BlobContainer"]);
builder.Services.AddDbContext<DocumentLibContext>(opts =>
{
    opts.UseSqlServer(config["DbConnectionString"], x => x.MigrationsAssembly("document.lib.ef"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// var scopeRequiredByApi = app.Configuration["AzureAd:Scopes"] ?? ""; -- scopes required by the api
// httpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi); -- inside the endpoint
//.RequireAuthorization(); -- append to the endpoint

app.AddFolderApi();

app.Run();