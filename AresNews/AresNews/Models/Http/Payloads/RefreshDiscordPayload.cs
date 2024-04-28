
using Newtonsoft.Json;

namespace AresNews.Models.Http.Payloads
{
    public class RefreshDiscordPayload
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
