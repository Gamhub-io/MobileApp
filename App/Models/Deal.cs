﻿using Newtonsoft.Json;
using SQLite;

namespace GamHubApp.Models;

public class Deal
{
    [JsonProperty("_id")]
    public string Id { get; set; }
    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }
    [JsonProperty("description")]
    public string Description { get; set; }
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("discount")]
    public string Discount { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("createdAt")]
    public DateTime Created { get; set; }
    [JsonProperty("expiresAt")]
    public DateTime Expires { get; set; }
    [JsonProperty("partner"), Ignore]
    public Partner Partner { get; set; }
    public Command Navigate
    {
        get 
        {
            return new Command(async () => 
                await Browser.OpenAsync(Url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.External,
                    TitleMode = BrowserTitleMode.Default,
                }));
        }
    }
    [JsonIgnore, Ignore]
    public bool EpirationDisplayed { get => (Expires - DateTime.Now).TotalDays < 30; }
}
