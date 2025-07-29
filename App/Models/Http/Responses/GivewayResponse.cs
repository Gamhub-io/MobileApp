
using Newtonsoft.Json;

namespace GamHubApp.Models.Http.Responses;

public class GivewayResponse
{
    [JsonProperty("data")]
    public List<Giveaway> Data { get; set; }
    
}
