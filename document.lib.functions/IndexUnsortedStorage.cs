using System;
using System.Threading.Tasks;
using document.lib.functions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace document.lib.functions
{
    public static class IndexUnsortedStorage
    {
        [FunctionName("IndexUnsortedStorage")]
        public static async Task Run([BlobTrigger("library-storage/unsorted/{name}", Connection = "AzureWebJobsDocumentLibStorage")]ICloudBlob blob, string name, ILogger log)
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
