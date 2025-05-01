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
            Preferences.Set(AppConstant.DealPageEnable, _dealPageSett = value);
            OnPropertyChanged(nameof(DealPageSett));
        }
    }
    public Command OpenSettingsCommand
    {
        get => new Command(() => AppInfo.Current.ShowSettingsUI());
    }
    public SettingsViewModel ()
    {
        _dealPageSett = Preferences.Get(AppConstant.DealPageEnable, true);
    }

}
