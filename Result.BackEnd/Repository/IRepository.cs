using Result.BackEnd.POCO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Result.BackEnd.Repository
{
    public interface IRepository
    {
        Task<IEnumerable<Vote>> GetVotes();
        Task<Dictionary<string, int>> GetStandings();
        Task<Vote> GetLastVote();
    }
}
