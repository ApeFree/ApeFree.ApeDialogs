using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 多选对话框（MAUI 实现，支持全选与反选）。
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    public partial class MauiMultipleSelectionDialog<T> : MauiDialogBase<IReadOnlyList<T>>
    {
        private readonly List<T> _allItems;
        private readonly ObservableCollection<object> _selectedItems;
        private readonly CollectionView _collectionView;

        public MauiMultipleSelectionDialog(
            MultipleSelectionDialogSettings<T> settings,
            IEnumerable<T> collection,
            IEnumerable<T>? defaultSelectedItems = null,
            IMauiDialogContext? context = default,
            IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _allItems = collection?.ToList() ?? new List<T>();
            _selectedItems = new ObservableCollection<object>();

            if (defaultSelectedItems != null)
            {
                foreach (var item in defaultSelectedItems)
                {
                    if (item != null)
                    {
                        _selectedItems.Add(item);
                    }
                }
            }

            _collectionView = new CollectionView
            {
                SelectionMode = SelectionMode.Multiple,
                ItemsSource = _allItems,
                SelectedItems = _selectedItems,
                HeightRequest = 220,
            };

            _collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = GridLength.Star },
                    },
                    Padding = new Thickness(8, 6),
                };

                var checkBox = new CheckBox
                {
                    Color = MauiThemeManager.AccentColor,
                    VerticalOptions = LayoutOptions.Center,
                };

                var label = new Label
                {
                    FontSize = 14,
                    TextColor = MauiThemeManager.TextPrimary,
                    VerticalOptions = LayoutOptions.Center,
                    Margin = new Thickness(8, 0, 0, 0),
                };
                label.SetBinding(Label.TextProperty, ".");

                grid.Children.Add(checkBox);
                Grid.SetColumn(checkBox, 0);

                grid.Children.Add(label);
                Grid.SetColumn(label, 1);

                return grid;
            });

            Host.SetDialogContent(_collectionView);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(GetSelectedList())
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

            if (settings.SelectAllOption != null)
            {
                var allOption = settings.SelectAllOption with
                {
                    Action = _ => SelectAll()
                };
                AddOption(allOption);
            }

            if (settings.ReverseSelectedOption != null)
            {
                var reverseOption = settings.ReverseSelectedOption with
                {
                    Action = _ => ReverseSelection()
                };
                AddOption(reverseOption);
            }
        }

        private IReadOnlyList<T> GetSelectedList()
        {
            return _selectedItems.OfType<T>().ToList();
        }

        private void SelectAll()
        {
            _selectedItems.Clear();
            foreach (var item in _allItems)
            {
                if (item != null)
                {
                    _selectedItems.Add(item);
                }
            }
        }

        private void ReverseSelection()
        {
            var currentlySelected = new HashSet<T>(_selectedItems.OfType<T>());
            _selectedItems.Clear();
            foreach (var item in _allItems)
            {
                if (item != null && !currentlySelected.Contains(item))
                {
                    _selectedItems.Add(item);
                }
            }
        }
    }
}
