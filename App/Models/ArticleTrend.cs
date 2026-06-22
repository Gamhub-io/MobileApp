using Newtonsoft.Json;
using SQLite;
using System.Collections.ObjectModel;

namespace GamHubApp.Models
{
    public class ArticleTrend
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public int Count { get; set; }

        public Article Article { get; set; }
    }
}