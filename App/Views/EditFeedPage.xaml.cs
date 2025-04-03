using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class EditFeedPage : ContentPage
{
	public EditFeedPage (Feed feed, FeedsViewModel vm, GeneralDataBase generalDB)
	{
		InitializeComponent ();
		BindingContext = new EditFeedViewModel(feed, vm, generalDB);
	}
    }