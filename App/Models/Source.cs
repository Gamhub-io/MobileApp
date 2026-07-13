using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Core;
using GamHubApp.Services;
using Newtonsoft.Json;
using Sentry.Protocol;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.ObjectModel;

namespace GamHubApp.Models;

public class Source
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Column("mongoId"), JsonProperty("_id")]
    public string MongoId { get; set; }
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("url")]
    public string Url { get; set; }
    [JsonProperty("primaryColour")]
    public string PrimaryColour { get; set; }
    [JsonProperty("secondaryColour")]
    public string SecondaryColour { get; set; }
    [JsonProperty("domain")]
    public string Domain { get; set; }
    [JsonProperty("logo")]
    public string Logo { get; set; }
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
    private bool _iselected = true;
    public bool IsSelected
    {
        get
        {
            return _iselected;
        }
        set 
        {
            bool prev = _iselected;
            _iselected = value;

            if (prev == _iselected)
                return;

            string selection = Preferences.Get(PreferencesKeys.SourceSelection, string.Empty);
            string idTag = $"_{MongoId}";
            if (selection.Contains(MongoId))
            {
                selection = selection.Replace(idTag, string.Empty);
            }
            else
            {
                selection += idTag;
            }
            Preferences.Set(PreferencesKeys.SourceSelection, selection);

        }
    }
    [JsonIgnore]
    //public bool IsSelected { get; set; }
    [OneToMany(CascadeOperations = CascadeOperation.All)]
    public Collection<Article> RelatedArticles { get; set; }

    [Ignore]
    public Command SelectCommand
    {
        get
        {
            return new Command(async () =>
            {
                string selection = Preferences.Get(PreferencesKeys.SourceSelection, string.Empty);
                string idTag = $"_{MongoId}";
                if (selection.Contains(MongoId))
                {
                    selection.Replace(idTag, string.Empty);
                }
                else
                {
                    selection += idTag;
                }

            }); ;
        }
    }
    public Source()
    {
    }

}
