using Newtonsoft.Json;

namespace GamHub.Models.Http.Responses
{
    public class RefreshSessionResponse
    {
        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}
