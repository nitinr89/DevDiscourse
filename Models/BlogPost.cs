using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Devdiscourse.Models
{
    public class BlogPost
    {

        [JsonProperty("ID")]
        public int Id { get; set; }

        [JsonProperty("date")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("URL")]
        public string Url { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }
    }
}