using System;
using System.Collections.Generic;
using System.Text;

namespace GamHubApp.Services;
public class ResourceLoader
{
    public Color DarkColor { get; private set; }
    public Color LightHeaderColor { get; private set; }
    public Color VeryLightDarkColor { get; private set; }
    public Color BackgroundColor { get; private set; }
    public Color DarkSecondaryColor { get; private set; }
    public static ResourceLoader Instance { get; } = new ResourceLoader();

    public ResourceLoader()
    {
        if (Application.Current.Resources.TryGetValue("Dark", out var value))
        {
            DarkColor = (Color)value;
        }
        if (Application.Current.Resources.TryGetValue("LightHeader", out var valueLightHeader))
        {
            LightHeaderColor = (Color)valueLightHeader;
        }
        if (Application.Current.Resources.TryGetValue("VeryLightDark", out var valueVeryLightDark))
        {
            VeryLightDarkColor = (Color)valueVeryLightDark;
        }
        if (Application.Current.Resources.TryGetValue("BackgroundColor", out var valueBackgroundColor))
        {
            BackgroundColor = (Color)valueBackgroundColor;
        }
        if (Application.Current.Resources.TryGetValue("DarkSecondary", out var valueDarkSecondary))
        {
            DarkSecondaryColor = (Color)valueDarkSecondary;
        }
    }
}
