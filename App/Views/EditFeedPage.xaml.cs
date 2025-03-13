using GamHubApp.Models;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EditFeedPage : ContentPage
{
	public EditFeedPage (Feed feed, FeedsViewModel vm)
	{
		InitializeComponent ();
		BindingContext = new EditFeedViewModel(feed, vm);
	}
    }