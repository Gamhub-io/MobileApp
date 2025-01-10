

using Android.Graphics;
using Android.Text;
using Android.Text.Style;

namespace GamHubApp.Platforms.Android.Renderers
{
    internal class CustomTypefaceSpan : TypefaceSpan
    {
        private Typeface newType;

        public CustomTypefaceSpan(string family, Typeface type) : base(family)
        {
            newType = type;
        }
        public override void UpdateDrawState(TextPaint ds)
        {
            applyCustomTypeFace(ds, newType);

        }
        public override void UpdateMeasureState(TextPaint paint)
        {
            applyCustomTypeFace(paint, newType);
        }
        private static void applyCustomTypeFace(TextPaint paint, Typeface tf)
        {
            TypefaceStyle oldStyle;
            Typeface old = paint.Typeface;
            if (old == null)
            {
                oldStyle = 0;
            }
            else
            {
                oldStyle = old.Style;
            }

            TypefaceStyle fake = oldStyle & ~tf.Style;
            if ((fake & TypefaceStyle.Bold) != 0)
            {
                paint.FakeBoldText = true;
            }

            if ((fake & TypefaceStyle.Italic) != 0)
            {
                paint.TextSkewX = -0.25f;
            }

            paint.SetTypeface(tf);
        }
    }
}
