using System.Text.Json.Serialization;

namespace GamHubApp.Models.Http.Responses;

public class NEupdateResponse
{
    [JsonPropertyName("message")]
    public string Msg { get; set; }
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}
