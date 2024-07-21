using CommunityToolkit.Maui.Views;
using GamHub.Models;
using GamHub.ViewModels;
using GamHub.ViewModels.PopUps;

namespace GamHub.Views
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