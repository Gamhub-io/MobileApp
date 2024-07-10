
using Newtonsoft.Json;

namespace GamHub.Models.Http.Payloads
{
    public class RefreshDiscordPayload
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
