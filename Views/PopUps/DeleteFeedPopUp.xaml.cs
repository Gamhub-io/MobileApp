using CommunityToolkit.Maui.Views;
using GamHubApp.Models;
using GamHubApp.ViewModels;
using GamHubApp.ViewModels.PopUps;

namespace GamHubApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteFeedPopUp : Popup
    {
        public DeleteFeedPopUp(Feed feed, FeedsViewModel vm)
        {
            InitializeComponent();
            BindingContext = new DeleteFeedPopUpViewModel(this, feed, vm);
        }
    }
}