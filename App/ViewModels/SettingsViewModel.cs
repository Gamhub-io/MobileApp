using CommunityToolkit.Maui.Alerts;
using GamHubApp.Core;

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
            UpdateSettings(AppConstant.DealPageEnable, _dealPageSett = value);

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
            UpdateSettings(AppConstant.DealArticleEnable, _dealViewSett = value);
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
            UpdateSettings(AppConstant.DealReminderEnabled, _dealReminderSett = value);
            OnPropertyChanged(nameof(DealReminderSett));
        }
    }

    public Command OpenSettingsCommand
    {
        get => new Command(() => AppInfo.Current.ShowSettingsUI());
    }
    public SettingsViewModel ()
    {
        _dealPageSett = Preferences.Get(AppConstant.DealPageEnable, true);
        _dealViewSett = Preferences.Get(AppConstant.DealArticleEnable, true);
        _dealReminderSett = Preferences.Get(AppConstant.DealReminderEnabled, true);
    }

    /// <summary>
    /// Update boolean settings
    /// </summary>
    /// <param name="settingsKey">Key of settings</param>
    /// <param name="value">the new bool value</param>
    private void UpdateSettings(string settingsKey, bool value)
    {
        Preferences.Set(settingsKey, value);

        // Notify the user we are updating the preferences
        // - Making sure we do it on the main thread
        MainThread.BeginInvokeOnMainThread(async () =>
                   await (Toast.Make("Settings changes saved")).Show());
    }

}
