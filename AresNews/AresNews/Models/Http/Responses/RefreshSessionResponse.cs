using Newtonsoft.Json;

namespace AresNews.Models.Http.Responses
{
    public class RefreshSessionResponse
    {
        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}
