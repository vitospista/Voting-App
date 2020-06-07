using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Worker.Poco;
using System.Threading.Tasks;

namespace Worker
{
    public static class QueueProcessor
    {
        //example vote:
        //{"VoterId":"8f4a64c5-0ab7-422e-8b3f-fdbdbb51bb44","Vote":"a","CorrelationId":"67e69cd8-ae59-46a0-971e-8fe0870cf916"}
        
        [FunctionName("QueueTrigger")]
        public static async Task Run(            
            [QueueTrigger("vote-queue")] string vote,
            [CosmosDB(ConnectionStringSetting = "CosmosConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation($"Processing vote: {vote}");

            QueueVote obj = JsonConvert.DeserializeObject<QueueVote>(vote);

            Uri uri = UriFactory.CreateDocumentCollectionUri(databaseId: "vote-db", collectionId: "votes");
            var response = await client.CreateDocumentAsync(
                uri, new
                {
                    id = obj.CorrelationId,
                    voterId = obj.VoterId,
                    choice = obj.Vote
                });

            log.LogInformation($"Inserted {response.Resource.Id}, cost: {response.RequestCharge}");

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
