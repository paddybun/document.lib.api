using Azure.Identity;
using Azure.Storage.Blobs;
using document.lib.ef;
using document.lib.shared.Cqrs;
using document.lib.shared.Cqrs.Interfaces;
using document.lib.shared.Models;
using document.lib.web.v2.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddRadzenComponents();

var configSection = builder.Configuration.GetSection("Config");
var appConfig = configSection.Get<SharedConfig>();
builder.Services.AddDbContext<DocumentLibContext>(opts =>
{
    opts.UseSqlServer(appConfig!.DbConnectionString, x => x.MigrationsAssembly("document.lib.ef"));
});
builder.Services.AddAzureClients(config =>
{
    // config.AddBlobServiceClient(new Uri(appConfig!.StorageAccount!));
    config.AddBlobServiceClient(appConfig!.BlobServiceConnectionString);
    DefaultAzureCredential credential = new();
    config.UseCredential(credential);
});
builder.Services.AddCqrs();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapGet("/api/upload/health", async Task<Ok> () => await Task.FromResult(TypedResults.Ok()));

app.MapPost("/api/upload/single", async (IUploadBlobUseCase uploadBlobUse, [FromForm]IFormFile file) =>
{
    using (var memStream = new MemoryStream())
    {
        await file.CopyToAsync(memStream);
        memStream.Position = 0;
        await uploadBlobUse.ExecuteAsync(file.FileName, memStream);        
    }

    return TypedResults.Ok();
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
