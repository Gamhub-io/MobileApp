using System.Collections.ObjectModel;

namespace GamHubApp.Models;

public interface ITrendResponse
{
    public Collection<ArticleTrend> Data { get; set; }
}
