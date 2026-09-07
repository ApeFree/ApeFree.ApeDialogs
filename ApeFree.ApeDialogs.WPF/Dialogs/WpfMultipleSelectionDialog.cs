using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 多选对话框（WPF 实现）。
    /// </summary>
    /// <typeparam name="T">选项数据类型</typeparam>
    public class WpfMultipleSelectionDialog<T> : WpfDialogBase<IReadOnlyList<T>>
    {
        private readonly MultipleSelectionDialogSettings<T> _settings;
        private readonly ApeListView _listView;
        private readonly List<SelectionItem> _items;

        public WpfMultipleSelectionDialog(MultipleSelectionDialogSettings<T> settings, IEnumerable<T> collection, IEnumerable<T>? defaultSelectedItems, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 480;
            Host.Height = 440;

            _items = collection.Select(item => new SelectionItem(item, settings.ItemDisplayConverter(item))).ToList();

            _listView = new ApeListView
            {
                SelectionMode = SelectionMode.Multiple,
                ItemTemplate = CreateItemTemplate(),
                ItemsSource = _items,
            };

            // default selected items
            if (defaultSelectedItems is not null)
            {
                var defaults = defaultSelectedItems.ToList();
                foreach (var item in _items)
                {
                    if (defaults.Any(d => Equals(d, item.Item)))
                    {
                        _listView.SelectedItems.Add(item);
                    }
                }
            }

            _listView.MouseDoubleClick += OnListDoubleClick;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(ContentTextBlock, 0);
            Grid.SetRow(_listView, 1);
            grid.Children.Add(ContentTextBlock);
            grid.Children.Add(_listView);
            Host.SetDialogContent(grid);

            AddDefaultOptions();
        }

        private static DataTemplate CreateItemTemplate()
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new Binding(nameof(SelectionItem.DisplayText)));
            factory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);

            var template = new DataTemplate(typeof(SelectionItem));
            template.VisualTree = factory;
            return template;
        }

        private List<T> GetSelectedItems()
        {
            return _listView.SelectedItems.Cast<SelectionItem>()
                .Select(x => (T)x.Item!)
                .ToList();
        }

        private void OnListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TrySubmit(GetSelectedItems());
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(GetSelectedItems());
            }
            else if (ReferenceEquals(dialogOption, _settings.SelectAllOption))
            {
                button.Click += (_, _) => SelectAll();
            }
            else if (ReferenceEquals(dialogOption, _settings.ReverseSelectedOption))
            {
                button.Click += (_, _) => ReverseSelection();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(GetSelectedItems());

        private void SelectAll()
        {
            foreach (var item in _items)
            {
                if (!_listView.SelectedItems.Contains(item))
                {
                    _listView.SelectedItems.Add(item);
                }
            }
        }

        private void ReverseSelection()
        {
            var selected = _listView.SelectedItems.Cast<SelectionItem>().ToList();
            foreach (var item in selected)
            {
                _listView.SelectedItems.Remove(item);
            }

            foreach (var item in _items)
            {
                if (!selected.Contains(item))
                {
                    _listView.SelectedItems.Add(item);
                }
            }
        }
    }
}
