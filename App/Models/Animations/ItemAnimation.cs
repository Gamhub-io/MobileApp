using CommunityToolkit.Maui.Animations;
using CommunityToolkit.Maui.Extensions;
using GamHubApp.Services;

namespace GamHubApp.Models.Animations;

public class ItemAnimation : BaseAnimation
{
    private const int Rate = 100;
    private ResourceLoader _resourceLoader;
    public ItemAnimation() : this(ResourceLoader.Instance)
    {
    }
    public ItemAnimation (ResourceLoader resourceLoader)
    {
        _resourceLoader = resourceLoader;
    }


    public async override Task Animate(VisualElement view, CancellationToken token = default)
    {
        const int pause = 200;

        var colors = new[]
        {
            _resourceLoader.BackgroundColor,
            _resourceLoader.LightDarkColor,
            _resourceLoader.DarkColor,
        };

        while (!token.IsCancellationRequested)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                int prev = i - 1;
                if (prev < 0)
                    prev = 0;
                var from = colors[prev];
                var to = colors[i];

                TaskCompletionSource<bool> tcs = new();

                await ColorTransition(view, _resourceLoader.BackgroundColor, to, Rate, Easing.CubicInOut, token);
                await Task.Delay(pause, token);
            }
        }
        //while (!token.IsCancellationRequested)
        //{
        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.BackgroundColor, Rate);

        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkColor, Rate);

        //    if (_resourceLoader.DarkSecondaryColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkSecondaryColor, Rate);
        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.BackgroundColor, Rate);
        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkColor, Rate);

        //    if (_resourceLoader.DarkSecondaryColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkSecondaryColor, Rate);
        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.BackgroundColor, Rate);
        //    if (_resourceLoader.DarkColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkColor, Rate);

        //    if (_resourceLoader.DarkSecondaryColor != null)
        //        await view.BackgroundColorTo(_resourceLoader.DarkSecondaryColor, Rate);
        //}

    }
    private Task ColorTransition(
    VisualElement view,
    Color from,
    Color to,
    uint duration,
    Microsoft.Maui.Easing easing,
    CancellationToken token)
    {
        TaskCompletionSource<bool> tcs = new();

        view.Animate(
            name: "ColorLerp",
            callback: t =>
            {
                var eased = easing.Ease(t);

                view.BackgroundColor = Color.FromRgba(
                    from.Red + (to.Red - from.Red) * eased,
                    from.Green + (to.Green - from.Green) * eased,
                    from.Blue + (to.Blue - from.Blue) * eased,
                    from.Alpha + (to.Alpha - from.Alpha) * eased
                );
            },
            start: 0,
            end: 0.3,
            length: duration,
            easing: easing,
            finished: (v, c) => tcs.TrySetResult(true)
        );

        token.Register(() => tcs.TrySetCanceled());

        return tcs.Task;
    }
}
