
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Payloads
{
    public class RefreshDiscordPayload
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
