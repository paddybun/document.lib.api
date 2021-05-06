using System;
using System.Threading.Tasks;
using document.lib.functions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace document.lib.functions
{
    public static class IndexUnsorted
    {
        [Disable("TriggeredByBlob")]
        [FunctionName("IndexUnsorted")]
        public static async Task Run([TimerTrigger("0 */30 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                var indexerService = new IndexerService();
                var result = await indexerService.IndexUnsortedAsync();
                log.LogInformation("Indexed {0} unsorted documents", result.Count);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
            }
        }
    }
}
