using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamHubApp.Models
{
    public class User
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("public_name")]
        public string PublicName { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("admin")]
        public bool Admin { get; set; }

        [JsonProperty("connectors")]
        public List<object> Connectors { get; set; }

        [JsonProperty("__v")]
        public int Version { get; set; }
    }

}
