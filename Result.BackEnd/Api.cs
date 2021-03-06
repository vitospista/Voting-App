using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Linq;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Result.BackEnd.Helper;
using Result.BackEnd.POCO;
using System.Collections.Generic;
using Result.BackEnd.Repository;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Result.BackEnd
{
    public class Api
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private IRepository _cosmosRepo;

        public Api(
            ILogger<Api> logger,
            IConfiguration config,
            IRepository cosmosRepo
            )
        {
            _logger = logger;
            _config = config;
            _cosmosRepo = cosmosRepo;
        }

        [FunctionName(nameof(GetVotes))]
        public async Task<IActionResult> GetVotes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetVotes processed a request.");

            IEnumerable<Vote> votes = await _cosmosRepo.GetVotes();

            return new OkObjectResult(votes);
        }

        [FunctionName(nameof(GetStandings))]
        public async Task<IActionResult> GetStandings(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetStandings processed a request.");

            var standings = await _cosmosRepo.GetStandings();

            return new OkObjectResult(standings);
        }

        [FunctionName(nameof(GetLastVote))]
        public async Task<IActionResult> GetLastVote(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetLastVote processed a request.");

            Vote lastVote = await _cosmosRepo.GetLastVote();

            return new OkObjectResult(lastVote);
        }

        [FunctionName(nameof(VoteInserted))]
        public Task VoteInserted(
            [CosmosDBTrigger(
                databaseName: "vote-db",
                collectionName: "votes",
                ConnectionStringSetting = "CosmosConnection",
                LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> input,
            [SignalR(HubName = "voteHub")]IAsyncCollector<SignalRMessage> signalRMessages)
        {
            _logger.LogInformation("GetLastVote processed a request.");            

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "voteInserted",
                    Arguments = input.ToArray()
                });
        }

        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [SignalRConnectionInfo(HubName = "voteHub")]SignalRConnectionInfo connectionInfo,
            ILogger log)
        {
            log.LogInformation("Negotiate function processed a request.");

            return connectionInfo;
        }
    }
}
