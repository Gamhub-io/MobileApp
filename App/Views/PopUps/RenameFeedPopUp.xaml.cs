using GamHubApp.Models;
using GamHubApp.ViewModels;
using GamHubApp.ViewModels.PopUps;
using CommunityToolkit.Maui.Views;

namespace GamHubApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RenameFeedPopUp : Popup
    {
		public RenameFeedPopUp (Feed feed, FeedsViewModel vm)
		{
			InitializeComponent ();
			BindingContext = new RenameFeedPopUpViewModel (this,feed, vm);
        }
    }
}