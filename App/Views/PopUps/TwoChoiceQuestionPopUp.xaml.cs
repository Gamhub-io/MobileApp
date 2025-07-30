using CommunityToolkit.Maui.Views;

namespace GamHubApp.Views;

public partial class TwoChoiceQuestionPopUp : Popup
{
    private Action _secondaryCallback;
    private Action _primaryCallback;

    public TwoChoiceQuestionPopUp(string title,
                                  string subtitle,
                                  string primaryChoice, 
                                  string secondaryChoice, 
                                  Action primaryCallback, 
                                  Action secondaryCallback = null)
	{
		InitializeComponent();
        _secondaryCallback = secondaryCallback;
        _primaryCallback = primaryCallback;
        titlelbl.Text = title;
        subtitlelbl.Text = subtitle;
        primaryChoicelbl.Text = primaryChoice;
        secondaryChoicelbl.Text = secondaryChoice;
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