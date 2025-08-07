using GamHubApp.ViewModels;


namespace GamHubApp.Views;

public partial class GemTopUpPage : ContentPage
{
    private GemTopUpViewModel _vm;

    public GemTopUpPage(GemTopUpViewModel viewModel)
	{
		BindingContext = _vm = viewModel;
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Task.Run(() =>
        {
           _= _vm.LoadOptionsAsync();

        });
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        _vm.PlanSelected = false;
    }
}