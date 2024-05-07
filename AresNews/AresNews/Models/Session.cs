using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class Session
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }
        [JsonIgnore]
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }

}
