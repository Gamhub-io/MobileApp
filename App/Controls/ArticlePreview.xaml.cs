using GamHubApp.Models;

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
    }
}