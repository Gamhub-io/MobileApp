using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses
{
    public class RefreshSessionResponse
    {
        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}
