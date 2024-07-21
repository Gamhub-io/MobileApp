using GamHub.Models;
using GamHub.ViewModels;
using GamHub.ViewModels.PopUps;
using CommunityToolkit.Maui.Views;

namespace GamHub.Views
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