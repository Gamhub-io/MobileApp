using CommunityToolkit.Maui.Views;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class GiveawayEntryQuestionPopUp : Popup
{
    private Action _secondaryCallback;
    private Action _primaryCallback;

    public GiveawayEntryQuestionPopUp(int gemAmount,
                             Action primaryCallback, 
                             Action secondaryCallback = null)
	{
		InitializeComponent();
        _secondaryCallback = secondaryCallback;
        _primaryCallback = primaryCallback;
        BindingContext = _vm = new GiveawayEntryQuestionViewModel();
        _vm.GemAmount = gemAmount;

    }
    private int _gemAmount;
    private GiveawayEntryQuestionViewModel _vm;

    public int GemAmount
    {
        get
        {
            return _gemAmount;
        }
        set
        {
            _gemAmount = value;
            OnPropertyChanged(nameof(GemAmount));
        }
    }


    private void Primary_Clicked (object sender, EventArgs e)
    {
		this.CloseAsync().GetAwaiter();
        _primaryCallback();
    }
    private void Secondary_Clicked (object sender, EventArgs e)
    {
		this.CloseAsync().GetAwaiter();

        if (_secondaryCallback is not null)
            _secondaryCallback();
    }
}