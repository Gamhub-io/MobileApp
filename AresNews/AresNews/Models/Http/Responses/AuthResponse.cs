using Newtonsoft.Json;

namespace AresNews.Models.Http.Responses
{
    public class AuthResponse
    {
        [JsonProperty("userdata")]
        public User UserData { get; set; }
        [JsonProperty("session")]
        public Session Session { get; set; }
    }
}
