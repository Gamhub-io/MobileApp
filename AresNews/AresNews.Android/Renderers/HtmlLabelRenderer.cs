using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AresNews.Controls;
using AresNews.Droid.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.FastRenderers;

[assembly: ExportRenderer(typeof(HtmlLabel), typeof(HtmlLabelRenderer))]
namespace AresNews.Droid.Renderers
{
    
    public class HtmlLabelRenderer : Xamarin.Forms.Platform.Android.FastRenderers.LabelRenderer
    {
        public HtmlLabelRenderer(Context ctx) : base(ctx)
        {
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var view = (HtmlLabel)Element;
            if (view == null) return;
            if (!string.IsNullOrEmpty(Control.Text))
            {
                if (Control.Text.Contains("<a"))
                {
                    var a = Control.Text.IndexOf("<a");
                    var b = Control.Text.IndexOf("</a>");
                    var d = Control.Text.Length;
                    var c = Control.Text.Length - Control.Text.IndexOf("</a>");
                    int length = b - a + 4;

                    string code = Control.Text.Substring(a, length);
                    Control.SetText(Html.FromHtml(view.Text.ToString().Replace(code, string.Empty),FromHtmlOptions.ModeLegacy), TextView.BufferType.Spannable);
                }
                else
                    Control.SetText(Html.FromHtml(view.Text.ToString(), FromHtmlOptions.ModeLegacy), TextView.BufferType.Spannable);
            }
        }
    }
}