using GamHubApp.Models;
using GamHubApp.ViewModels;
using GamHubApp.ViewModels.PopUps;
using CommunityToolkit.Maui.Views;
using GamHubApp.Services;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class RenameFeedPopUp : Popup
{
	public RenameFeedPopUp (Feed feed, FeedsViewModel vm, GeneralDataBase generalDataBase)
	{
		InitializeComponent ();
		BindingContext = new RenameFeedPopUpViewModel (this,feed, vm, generalDataBase);
    }
}