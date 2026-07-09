using Android.Text;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Microsoft.Maui.Handlers;
using System.Text.RegularExpressions;

namespace GamHubApp.Platforms.Android.Renderers;

public class HtmlLabelHandler : ViewHandler<HtmlLabel, TextView>
{
    public static PropertyMapper<HtmlLabel, HtmlLabelHandler> Mapper = new(ViewHandler.ViewMapper)
    {
        [nameof(HtmlLabel.HtmlText)] = MapHtmlText
    };

    public HtmlLabelHandler() : base(Mapper) { }

    protected override TextView CreatePlatformView() => new TextView(Context);

    private static void MapHtmlText(HtmlLabelHandler handler, HtmlLabel view)
    {
        if (handler.PlatformView != null && !string.IsNullOrEmpty(view.SanitizedHtmlText))
        {
            // Pass a custom ImageGetter to the FromHtml method
            var imageGetter = new UrlImageGetter(handler.PlatformView);

#pragma warning disable CA1422 // Validate platform compatibility
            ISpanned formattedText = Html.FromHtml(
                view.SanitizedHtmlText,
                //FromHtmlFlags.ModeLegacy,
                imageGetter,
                null
            );
#pragma warning restore CA1422 // Validate platform compatibility

            handler.PlatformView.TextFormatted = formattedText;
            view.InvalidateMeasure();
        }
    }
}
public class HtmlLabel : View
{
    public static readonly BindableProperty HtmlTextProperty =
        BindableProperty.Create(nameof(HtmlText), 
            typeof(string), 
            typeof(HtmlLabel), 
            default(string),
            propertyChanged: OnHtmlTextChanged);

    public string HtmlText
    {
        get => (string)GetValue(HtmlTextProperty);
        set => SetValue(HtmlTextProperty, value);
    }
    private static void OnHtmlTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is HtmlLabel label && newValue is string rawHtml)
        {
            // Strip out <style> blocks and their contents completely
            string cleanHtml = Regex.Replace(rawHtml, @"<style[^>]*>[\s\S]*?</style>", "", RegexOptions.IgnoreCase);

            // Strip out <script> blocks just in case they exist too
            cleanHtml = Regex.Replace(cleanHtml, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

            // trim white spaces
            cleanHtml = Regex.Replace(cleanHtml, @"([\s\r\n\t]|<br\s*/?>|&nbsp;)+$", "", RegexOptions.IgnoreCase);

            // Re-assign the sanitized string without re-triggering the property changed loop
            label.SanitizedHtmlText = cleanHtml;
        }
    }

    // Internal property your handler will now read from instead of HtmlText
    public string SanitizedHtmlText { get; private set; }
}
public class UrlImageGetter : Java.Lang.Object, Html.IImageGetter
{
    private readonly TextView _textView;

    public UrlImageGetter(TextView textView)
    {
        _textView = textView;
    }

    public Drawable GetDrawable(string source)
    {
        var levelListDrawable = new LevelListDrawable();

        levelListDrawable.SetBounds(0, 0, 100, 100);

        Task.Run(async () =>
        {
            try
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(source);
                var bitmap = await BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length);

                if (bitmap != null)
                {
                    _textView.Post(() =>
                    {
                        int maxWidth = _textView.Width - _textView.PaddingLeft - _textView.PaddingRight;

                        if (maxWidth <= 0)
                        {
                            maxWidth = _textView.Context.Resources.DisplayMetrics.WidthPixels;
                        }

                        int finalWidth = bitmap.Width;
                        int finalHeight = bitmap.Height;

                        if (bitmap.Width > maxWidth && maxWidth > 0)
                        {
                            double scaleRatio = (double)maxWidth / bitmap.Width;
                            finalWidth = maxWidth;
                            finalHeight = (int)(bitmap.Height * scaleRatio);
                        }

                        var finalImage = new BitmapDrawable(_textView.Context.Resources, bitmap);

                        finalImage.SetBounds(0, 0, finalWidth, finalHeight);

                        levelListDrawable.AddLevel(1, 1, finalImage);
                        levelListDrawable.SetBounds(0, 0, finalWidth, finalHeight);
                        levelListDrawable.SetLevel(1);

                        // force rendering
                        var currentText = _textView.TextFormatted;
                        _textView.TextFormatted = null;
                        _textView.TextFormatted = currentText;

                        _textView.RequestLayout();
                    });
                }
            }
            catch
            {
                // If network fails, clear the bounds so the placeholder goes away cleanly
                _textView.Post(() => levelListDrawable.SetBounds(0, 0, 0, 0));
            }
        });

        return levelListDrawable;
    }
}

