using System;
using System.Collections.Generic;
using System.Text;

namespace Worker.Poco
{
    public class QueueVote
    {
        public string CorrelationId { get; set; }
        public string VoterId { get; set; }
        public string Vote { get; set; }
    }
}
