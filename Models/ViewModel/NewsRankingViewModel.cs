using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models.ViewModel
{
    public class NewsRankingViewModel
    {
        public Guid RegionId { get; set; }
        public string Region { get; set; }
        public float Ranking { get; set; }
    }
}