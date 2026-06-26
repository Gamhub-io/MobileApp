using Newtonsoft.Json;
using SQLite;
using System.Collections.ObjectModel;

namespace GamHubApp.Models
{
    public class TrendItem
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        public int Count { get; set; }
    }
}