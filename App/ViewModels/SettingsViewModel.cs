namespace GamHubApp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    public Command OpenSettingsCommand
    {
        get => new Command(() => AppInfo.Current.ShowSettingsUI());
    }
    public SettingsViewModel ()
    {

    }

}
