using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using document.lib.functions.v4.Models;
using document.lib.shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

// ReSharper disable UnusedMember.Global

namespace document.lib.functions.v4
{
    public class IndexUnsortedStorage(ILogger<IndexUnsortedStorage> logger, IDocumentService service)
    {
        [Function("TriggerMe")]
        public IActionResult TriggerMe([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            return new OkObjectResult($"Welcome to Azure Functions, {req.Query["name"]}!");
        }

        [Function("IndexUnsortedStorage")]
        public async Task Run([QueueTrigger(
            "new-docs",
            Connection = "DocumentStorageConnection")] QueueMessage message)
        {
            try
            {
                var createdEvent = await JsonSerializer.DeserializeAsync<EventGridBlobCreated>(message.Body.ToStream());
                if (createdEvent == null)
                {
                    logger.LogError("Failed to deserialize queue message");
                    return;
                }

                var blob = new BlobClient(new Uri(createdEvent.Data.Url));
                var name = blob.Name.Split("/")[^1];

                await service.AddDocumentToIndexAsync(blob.Name);

                logger.LogInformation($"Processed blob\n Name:{name} \n Size: Bytes");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }

        }
    }
}
