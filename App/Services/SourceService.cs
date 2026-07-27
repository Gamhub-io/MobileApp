using GamHubApp.Models;
using System.Collections.ObjectModel;

namespace GamHubApp.Services;

public class SourceService
{
    public ObservableCollection<Source> Sources { get; } = new();

    // Event raised whenever any source's IsSelected property changes
    public event EventHandler<Source>? SourcesChanged;

    public void NotifySourcesChanged(Source source)
    {
        SourcesChanged?.Invoke(this, source);
    }
}
