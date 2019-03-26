using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace trendeamon.Models
{
    public class BrokenLinksDataModel
    {
        public string Url { get; set; }
        public BrokenLinksStatus Status { get; set; }
        public IEnumerable<string> Links { get; set; }
        public int BrokenLinksCount { get; set; }
    }

    public enum BrokenLinksStatus
    {
        Pending,
        Running,
        Done
    }
}
