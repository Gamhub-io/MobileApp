using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models.Http.Responses
{
    public class DiscordAuthResponse
    {
        [JsonProperty("userdata")]
        public User UserData { get; set; }
        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}
