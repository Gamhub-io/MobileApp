using AresNews.Controls;
using AresNews.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace AresNews.iOS.Renderers
{
    public class HtmlLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (HtmlLabel)Element;
            if (view == null) return;

            var attr = new NSAttributedStringDocumentAttributes();
            var nsError = new NSError();
            attr.DocumentType = NSDocumentType.HTML;

            Control.AttributedText = new NSAttributedString(view.Text, attr, ref nsError);

            var mutable = Control.AttributedText as NSMutableAttributedString;
            UIStringAttributes uiString = new UIStringAttributes();
            uiString.Font = UIFont.FromName("Roboto-Regular", 15f);
            uiString.ForegroundColor = UIColor.FromRGB(130, 130, 130);

            if (!string.IsNullOrEmpty(Control.Text))
            {
                if (Control.Text.Contains("<a"))
                {
                    var text1 = Control.Text.IndexOf("<a");
                    var text2 = Control.Text.IndexOf("</a>");
                    int length = text2 - text1 + 4;

                    string code = Control.Text.Substring(text1, length);
                    Control.Text = Control.Text.Replace(code, string.Empty);
                }
            }

            mutable.SetAttributes(uiString, new NSRange(0, Control.Text.Length));

        }
    }
}