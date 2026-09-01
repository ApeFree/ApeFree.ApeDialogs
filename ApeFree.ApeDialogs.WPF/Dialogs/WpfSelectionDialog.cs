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
    /// 单选对话框（WPF 实现）。
    /// </summary>
    /// <typeparam name="T">选项数据类型</typeparam>
    public class WpfSelectionDialog<T> : WpfDialogBase<T>
    {
        private readonly SelectionDialogSettings<T> _settings;
        private readonly ApeListView _listView;
        private readonly List<SelectionItem> _items;

        public WpfSelectionDialog(SelectionDialogSettings<T> settings, IEnumerable<T> collection, T defaultSelectedItem, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 480;
            Host.Height = 420;

            _items = collection.Select(item => new SelectionItem(item, settings.ItemDisplayConverter(item))).ToList();

            _listView = new ApeListView
            {
                SelectionMode = SelectionMode.Single,
                ItemTemplate = CreateItemTemplate(),
                ItemsSource = _items,
            };

            // default selected item
            var defaultIndex = _items.FindIndex(x => Equals(x.Item, defaultSelectedItem));
            if (defaultIndex >= 0)
            {
                _listView.SelectedIndex = defaultIndex;
                _listView.ScrollIntoView(_listView.SelectedItem);
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

        private T? GetSelectedItem()
        {
            return _listView.SelectedItem is SelectionItem selection
                ? (T)selection.Item!
                : default;
        }

        private void OnListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GetSelectedItem() is not null || _listView.SelectedItem is not null)
            {
                TrySubmit(GetSelectedItem()!);
            }
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(GetSelectedItem()!);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(GetSelectedItem()!);
    }
}
