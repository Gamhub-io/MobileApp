﻿
namespace GamHubApp.Services.UI;

public static class BadgeCounterService
{
    private static int _count;
    public static int Count
    {
        get => _count;
        private set
        {
            _count = value;
            CountChanged?.Invoke(null, _count);
        }
    }

    public static void SetCount(int count) => Count = count;

    public static event EventHandler<int> CountChanged;
}

//// Code inspired by https://github.com/Abhayprince/TabbarBadgeShellMAUI
