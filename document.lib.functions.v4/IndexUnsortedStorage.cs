using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using document.lib.functions.v4.Models;
using document.lib.shared.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
// ReSharper disable UnusedMember.Global

namespace document.lib.functions.v4
{
    public class IndexUnsortedStorage
    {
        private readonly ILogger<IndexUnsortedStorage> _logger;

        public IndexUnsortedStorage(ILogger<IndexUnsortedStorage> logger)
        {
            _logger = logger;
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
                    _logger.LogError("Failed to deserialize queue message");
                    return;
                }

                var blob = new BlobClient(new Uri(createdEvent.Data.Url));
                var name = blob.Name.Split("/")[^1];

                var indexerService = new IndexerService();
                _logger.LogInformation($"Processed blob\n Name:{name} \n Size: Bytes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
            
        }
    }
}
