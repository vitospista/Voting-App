using System;
using System.Collections.Generic;
using System.Text;

namespace Result.BackEnd.POCO
{
    public class Vote
    {
        public string id { get; set; }
        public string voterId { get; set; }
        public string choice { get; set; }
    }
}
