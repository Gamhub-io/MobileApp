using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class GiveawayPage : ContentPage
{
    private GiveawayViewModel _vm;

	public GiveawayPage(GiveawayViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;

    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        _vm.RefreshGiveawayList().GetAwaiter();
        _vm.UpdateGems().GetAwaiter();

    }
}