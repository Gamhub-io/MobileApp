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

    private bool _dealViewSett;
    public bool DealViewSett
    {
        get => _dealViewSett;
        set
        {
            Preferences.Set(AppConstant.DealArticleEnable, _dealViewSett = value);
            OnPropertyChanged(nameof(DealViewSett));
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
    }

}
