using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class UserGiveawayWinsPage : ContentPage
{
    private GiveawayWinsViewModel _vm;

    public UserGiveawayWinsPage(GiveawayWinsViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _vm.GetWins().GetAwaiter();
    }
}