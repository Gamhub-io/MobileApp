using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.ImageSources;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Layouts;
using CommunityToolkit.Maui.Views;

namespace GamHub.Controls
{
    /// <summary>
    /// Temporary workaround for #595 [Bug] Setting TabView.SelectedIndex does not "visually" switch tabs:
    /// https://github.com/xamarin/XamarinCommunityToolkit/issues/595 
    /// The BugFix should be merged soon:
    /// https://github.com/xamarin/XamarinCommunityToolkit/pull/738
    /// </summary>
    public class TabViewWorkaround : TabView
    {
        public static readonly BindableProperty SelectedIndexWorkaroundProperty = BindableProperty.Create(
            propertyName: nameof(SelectedIndexWorkaround),
            returnType: typeof(int),
            declaringType: typeof(TabViewWorkaround),
            defaultValue: -1,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: OnWorkaroundPropertyChanged);

        private static void OnWorkaroundPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue || (int)oldValue == currentIndex)
                return;

            ((TabViewWorkaround)bindable).UpdateIndexWorkaround((int)newValue);

            currentIndex = (int)newValue;
        }

        public int SelectedIndexWorkaround
        {
            get => (int)GetValue(SelectedIndexWorkaroundProperty);
            set => SetValue(SelectedIndexWorkaroundProperty, value);
        }

        private MethodInfo updateSelectedIndexMethod;
        private static int currentIndex = -1;

        public TabViewWorkaround() : base()
        {
            updateSelectedIndexMethod = typeof(TabView).GetMethod("UpdateSelectedIndex", BindingFlags.NonPublic | BindingFlags.Instance);
            SelectionChanged += TabView_SelectionChanged;
        }

        private void UpdateIndexWorkaround(int index)
        {
            try
            {
                updateSelectedIndexMethod.Invoke(this, new object[] { index, false });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UpdateSelectedIndex Ex: " + ex);
            }
        }

        private void TabView_SelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            //if (SelectedIndex == SelectedIndexWorkaround)
            //    return;

            SelectedIndexWorkaround = SelectedIndex;
        }

    }
}
