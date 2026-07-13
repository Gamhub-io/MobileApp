using CommunityToolkit.Maui.Alerts;
using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private bool _dealPageSett;
    public bool DealPageSett
    {
        get => _dealPageSett;
        set
        {
            if (_dealPageSett == value) return;
            UpdateSettings(PreferencesKeys.DealPageEnable, _dealPageSett = value);

            OnPropertyChanged(nameof(DealPageSett));
        }
    }

    private bool _dealViewSett;
    public bool DealViewSett
    {
        get => _dealViewSett;
        set
        {
            if (_dealViewSett == value) return;
            UpdateSettings(PreferencesKeys.DealArticleEnable, _dealViewSett = value);
            OnPropertyChanged(nameof(DealViewSett));
        }
    }

    private bool _dealReminderSett;
    public bool DealReminderSett
    {
        get => _dealReminderSett;
        set
        {
            if (_dealReminderSett == value) return;
            UpdateSettings(PreferencesKeys.DealReminderEnabled, _dealReminderSett = value);
            OnPropertyChanged(nameof(DealReminderSett));
        }
    }

    private ObservableCollection<Source> _outlets;
    public ObservableCollection<Source> Outlets 
    { 
        get => _outlets; 
        set
        {
            _outlets = value;
            OnPropertyChanged(nameof(Outlets));
        }
    }

    public Command OpenSettingsCommand
    {
        get => new Command(() => AppInfo.Current.ShowSettingsUI());
    }
    public SettingsViewModel ()
    {
        _dealPageSett = Preferences.Get(PreferencesKeys.DealPageEnable, true);
        _dealViewSett = Preferences.Get(PreferencesKeys.DealArticleEnable, true);
        _dealReminderSett = Preferences.Get(PreferencesKeys.DealReminderEnabled, true);
        var fetcher = (App.Current as App).DataFetcher;
        _ = Task.Run(async () =>
        {
            string selection = Preferences.Get(PreferencesKeys.SourceSelection, string.Empty);
            Outlets = new (Fetcher.Sources?.Count <=0 ? await fetcher.GetSources() : Fetcher.Sources);

            if (string.IsNullOrEmpty(selection))
                return;

            for (int i = 0; i < _outlets.Count; i++)
            {
                Outlets[i].IsSelected = selection.Contains(_outlets[i].MongoId);
            }

        });
    }

    /// <summary>
    /// Update boolean settings
    /// </summary>
    /// <param name="settingsKey">Key of settings</param>
    /// <param name="value">the new bool value</param>
    private static void UpdateSettings(string settingsKey, bool value)
    {
        Preferences.Set(settingsKey, value);

        // Notify the user we are updating the preferences
        // - Making sure we do it on the main thread
        MainThread.BeginInvokeOnMainThread(async () =>
                   await (Toast.Make("Settings changes saved")).Show());
    }

}
