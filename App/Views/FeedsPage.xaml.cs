using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Services;
using GamHubApp.Services.ChangedMessages;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class FeedsPage : ContentPage
{
    private const int rButtonYStart = -43;
    private const uint _modalHeightStart = 0;
    private readonly uint _modalWidthStart = 50;
    private readonly FeedsViewModel _vm;
    private readonly double _refreshButtonYPos;

    public bool IsFromDetail { get; set; }

    public FeedsPage(FeedsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _refreshButtonYPos = refreshButton.Y;
        refreshButton.TranslationY = rButtonYStart;
        
        RegisterMessaging();
    }

    private void RegisterMessaging()
    {
        WeakReferenceMessenger.Default.Register<UnnoticedArticlesChangedMessage>(this, (r, m) =>
        {
            if (m.Count > 0)
                ShowRefreshButton();
            else
                RemoveRefreshButton();
        });

        WeakReferenceMessenger.Default.Register<ScrollFeedPageChangedMessage>(this, (r, m) =>
        {
            ScrollFeed();
        });
    }

    private void UnRegisterMessaging()
    {
        WeakReferenceMessenger.Default.Unregister<UnnoticedArticlesChangedMessage>(this);

        WeakReferenceMessenger.Default.Unregister<ScrollFeedPageChangedMessage>(this);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _vm.Resume().GetAwaiter();
        if (_vm.DataLoaded)
            CloseDropdownMenu();

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnRegisterMessaging();
    }

    /// <summary>
    /// Function to open a the dropdown
    /// </summary>
    public void OpenDropdownMenu()
    {
        double height = 70;
        double width = 180;
        //_vm.ModalClose = false;
        // Animation
        void callbackH(double inputH) => dropdownMenu.HeightRequest = inputH;
        void callbackW(double inputW) => dropdownMenu.WidthRequest = inputW;

        uint rate = 24;
        dropdownMenu.Animate("AnimHeightDropdownMenu", callbackH, dropdownMenu.Height, height, rate, 100, Easing.SinOut);
        dropdownMenu.Animate("AnimWidthDropdownMenu", callbackW, dropdownMenu.Width, width, rate, 100, Easing.SinOut);
        dropdownMenu.Padding = 3;

        _vm.IsMenuOpen = true;
    }

    /// <summary>
    /// Function to close a modal
    /// </summary>
    public void CloseDropdownMenu()
    {
        if (dropdownMenu.Height <= _modalHeightStart && dropdownMenu.Width <= _modalWidthStart)
            return;

        // Animation
        void callbackH(double inputH) => dropdownMenu.HeightRequest = inputH;
        void callbackW(double inputW) => dropdownMenu.WidthRequest = inputW;
        uint rate = 24;

        dropdownMenu.Animate("AnimWidthDropdownMenu", callbackW, dropdownMenu.Width, _modalWidthStart, rate, 500, Easing.SinOut);
        dropdownMenu.Animate("AnimHeightDropdownMenu", callbackH, dropdownMenu.Height, _modalHeightStart, rate, 500, Easing.SinOut);

        dropdownMenu.Padding = 0;

        _vm.IsMenuOpen = false;
    }

    private void Menu_Clicked(object sender, EventArgs e)
    {
        // If dropdown is closed
        if (dropdownMenu.Padding == 0)
        {
            OpenDropdownMenu();
            return;
        }

        CloseDropdownMenu();
    }

    private void MenuItem_Tapped(object sender, EventArgs e)
    {
        // If dropdown is open
        if (dropdownMenu.Padding != 0)
        {
            CloseDropdownMenu();
        }
    }


    /// <summary>
    /// Method to display the refresh button
    /// </summary>
    public void ShowRefreshButton()
    {
        refreshButton.TranslateTo(refreshButton.X, _refreshButtonYPos, easing: Easing.BounceOut);
    }

    /// <summary>
    /// Method to remove the refresh button
    /// </summary>
    public void RemoveRefreshButton()
    {
        refreshButton.TranslateTo(refreshButton.X, rButtonYStart);
    }

    private void newsCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        // Figuring out if the scroll is on top of the screen
        _vm.OnTopScroll = e.FirstVisibleItemIndex == 0;
    }
    /// <summary>
    /// Scroll the feed
    /// </summary>
    /// <param name="position">Position you order the feed to be. default 0 (all the way up)</param>
    public void ScrollFeed(int position = 0)
    {
        newsCollectionView.ScrollTo(position);
    }
}