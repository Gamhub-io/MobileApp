using CommunityToolkit.Maui.Views;

namespace GamHubApp.Views;

public partial class RewardPopUp : Popup
{
	public RewardPopUp(string gemAmount)
	{
		InitializeComponent();
		BindingContext = this;

        GemAmount = gemAmount;
    }

    public string GemAmount { get; private set; }

    private void Close_Clicked (object sender, EventArgs e)
    {
		this.CloseAsync().GetAwaiter();
    }
}