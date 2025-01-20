using GamHubApp.Models;
using GamHubApp.Views;

namespace GamHubApp.Controls;

public partial class ArticlePreview : ContentView
{
    public static readonly BindableProperty ArticleProperty = BindableProperty.Create(propertyName: nameof(Article), 
                                                                                       returnType: typeof(Article), 
                                                                                       declaringType: typeof(ArticlePreview), 
                                                                                       defaultBindingMode: BindingMode.OneWay); 
    public Article Article 
    { 
        get => (Article)GetValue(ArticleProperty); 
        set => SetValue(ArticleProperty, value); 
    }


    public ArticlePreview()
    {
        InitializeComponent();
        BindingContext = this;
    }

    private void Article_Tapped(object sender, EventArgs e)
    {
        if (Article == null) return;

        var articlePage = new ArticlePage(Article);


        _ = (App.Current as App).Windows[0].Page.Navigation.PushAsync(articlePage);
    }
}