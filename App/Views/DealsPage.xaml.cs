using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class DealsPage : ContentPage
{
	public DealsPage(DealsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}