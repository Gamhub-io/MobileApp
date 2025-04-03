using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.Views;

namespace GamHubApp.ViewModels.PopUps;

public class RenameFeedPopUpViewModel : BaseViewModel
{
    private readonly GeneralDataBase _generalDB;
    private Feed _feed;

    public Feed Feed
    {
        get { return _feed; }
        set
        {
            _feed = value;
            OnPropertyChanged(nameof(Feed));
        }
    }
    private FeedsViewModel _context;

    public FeedsViewModel Context
    {
        get { return _context; }
        set
        {
            _context = value;
            OnPropertyChanged(nameof(Feed));
        }
    }
    private RenameFeedPopUp _popUp;

    public RenameFeedPopUp PopUp
    {
        get { return _popUp; }
        set
        {
            _popUp = value;
            OnPropertyChanged(nameof(PopUp));
        }
    }

    public App CurrentApp { get; }
    public Microsoft.Maui.Controls.Command Validate => new Microsoft.Maui.Controls.Command(async () =>
    {

        // Remove feed
        Context.UpdateCurrentFeed(_feed);
        await _generalDB.UpdateFeed(_feed);

        // Close the popup
        _popUp.Close();
    });

    public Microsoft.Maui.Controls.Command Cancel => new Microsoft.Maui.Controls.Command(() =>
    {
        // Close the popup
        _popUp.Close();
    });
    public RenameFeedPopUpViewModel(RenameFeedPopUp page, Feed feed, FeedsViewModel vm, GeneralDataBase generalDataBase)
    {
        _feed = feed;
        _popUp = page;
        _context = vm;
        _generalDB = generalDataBase;

        CurrentApp = App.Current as App;
    }
}
