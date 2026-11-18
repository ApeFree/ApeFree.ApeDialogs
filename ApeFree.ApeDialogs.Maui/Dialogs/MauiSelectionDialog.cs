using System;
using System.Collections.Generic;
using System.Linq;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 单选对话框（MAUI 实现）。
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    public partial class MauiSelectionDialog<T> : MauiDialogBase<T>
    {
        private T _selectedItem;

        public MauiSelectionDialog(
            SelectionDialogSettings<T> settings,
            IEnumerable<T> collection,
            T defaultSelectedItem = default!,
            IMauiDialogContext? context = default,
            IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _selectedItem = defaultSelectedItem;
            var items = collection?.ToList() ?? new List<T>();

            var collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Single,
                ItemsSource = items,
                SelectedItem = defaultSelectedItem,
                HeightRequest = 220,
            };

            collectionView.SelectionChanged += (_, e) =>
            {
                if (e.CurrentSelection.Count > 0 && e.CurrentSelection[0] is T item)
                {
                    _selectedItem = item;
                }
            };

            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 14,
                    TextColor = MauiThemeManager.TextPrimary,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Microsoft.Maui.Thickness(12, 10),
                };
                label.SetBinding(Label.TextProperty, ".");
                return label;
            });

            Host.SetDialogContent(collectionView);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(_selectedItem)
            };
            AddOption(confirmOption);

            if (settings.CancelOption != null)
            {
                var cancelOption = settings.CancelOption with
                {
                    Action = _ => Cancel()
                };
                AddOption(cancelOption);
            }
        }
    }
}
