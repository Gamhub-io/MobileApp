using CommunityToolkit.Maui.Alerts;
using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly GeneralDataBase _generalDb;
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

    private bool _isInitialised;

    public Command OpenSettingsCommand
    {
        get => new Command(() => AppInfo.Current.ShowSettingsUI());
    }
    public Command SelectCommand
    {
        get
        {
            return new Command<Source>(async (source) =>
            {
                if (!_isInitialised || source == null)
                    return;
                await _generalDb.UpdateSourceById(source);
                DisplayUpdateToadt();
            }); ;
        }
    }
    public SettingsViewModel (GeneralDataBase generalDataBase)
    {
        _generalDb = generalDataBase;
        _dealPageSett = Preferences.Get(PreferencesKeys.DealPageEnable, true);
        _dealViewSett = Preferences.Get(PreferencesKeys.DealArticleEnable, true);
        _dealReminderSett = Preferences.Get(PreferencesKeys.DealReminderEnabled, true);
       
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
        DisplayUpdateToadt();
    }

    private static void DisplayUpdateToadt()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
                           await (Toast.Make("Settings changes saved")).Show());
    }

    public async Task InitialiseAsync()
    {
        Outlets = new(await _generalDb.GetSources());
        _isInitialised = true;
    }

}
