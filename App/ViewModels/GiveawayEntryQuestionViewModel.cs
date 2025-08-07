
namespace GamHubApp.ViewModels;

public class GiveawayEntryQuestionViewModel : BaseViewModel
{
    private int _gemAmount;
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
}
