using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Result.BackEnd.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Result.BackEnd.Repository
{
    public class VoteRepository : IRepository
    {
        private CosmosClient _client;
        private Database _database;
        private Container _container;

        public VoteRepository(IConfiguration config)
        {
            string connection = config.GetConnectionStringOrSetting("CosmosConnection");
            string db = config.GetConnectionStringOrSetting("CosmosDb");
            string container = config.GetConnectionStringOrSetting("CosmosContainer");

            _client = new CosmosClient(connection);
            _database = _client.GetDatabase(db);
            _container = _database.GetContainer(container);
        }

        public async Task<Vote> GetLastVote()
        {
            var sql = @"select top 1 * from c 
                        where is_defined(c.choice)
                        order by c._ts desc";
            var iterator = await _container.GetItemQueryIterator<Vote>(sql).ReadNextAsync();

            return iterator.FirstOrDefault();
        }

        public async Task<Dictionary<string, int>> GetStandings()
        {
            var sql = @"select 
                            c.choice as choice, 
                            count(c.choice) as choicecount
                        from c
                        where is_defined(c.choice)
                        group by c.choice";
            var standing = await _container.GetItemQueryIterator<VoteCount>(sql).ReadNextAsync();

            return standing.ToDictionary(
                x => x.choice, 
                y => int.Parse(y.choiceCount));
        }

        public async Task<IEnumerable<Vote>> GetVotes()
        {
            List<Vote> votes = new List<Vote>();
            var sql = "select * from c";
            var iterator = _container.GetItemQueryIterator<Vote>(sql);

            while (iterator.HasMoreResults)
            {
                var documents = await iterator.ReadNextAsync();
                votes.AddRange(documents);
            }

            return votes;
        }
    }
}
