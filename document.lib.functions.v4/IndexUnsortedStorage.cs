using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using document.lib.shared.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace document.lib.functions.v4
{
    public static class IndexUnsortedStorage
    {
        [FunctionName("IndexUnsortedStorage")]
        public static async Task Run([BlobTrigger("library-storage/unsorted/{name}", Connection = "DocumentStorageConnection")] BlobClient blob, string name, ILogger log)
        {
            try
            {
                var indexerService = new IndexerService();
                await indexerService.IndexSingleDocumentAsync(blob);
                log.LogInformation($"Processed blob\n Name:{name} \n Size: Bytes");
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
            
        }
    }
}
