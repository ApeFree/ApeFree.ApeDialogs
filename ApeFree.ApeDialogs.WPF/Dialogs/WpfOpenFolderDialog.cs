using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 打开文件夹对话框（WPF 实现）：自绘目录浏览器（面包屑 + 子目录列表 + 上级按钮 + 双击进入）。
    /// 已知限制：忽略 <see cref="OpenFolderDialogSettings.MultiSelect"/>（结果为当前浏览目录的单路径）。
    /// </summary>
    public class WpfOpenFolderDialog : WpfDialogBase<IReadOnlyList<string>>
    {
        private readonly OpenFolderDialogSettings _settings;
        private readonly TextBlock _pathText;
        private readonly ApeListView _listView;
        private string _currentPath = string.Empty;

        public WpfOpenFolderDialog(OpenFolderDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 520;
            Host.Height = 440;

            _pathText = new TextBlock
            {
                FontSize = 13,
                TextTrimming = TextTrimming.CharacterEllipsis,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _pathText.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillSecondary");

            var upButton = new ApeButton
            {
                Content = "Up",
                MinWidth = 0,
                Padding = new Thickness(10, 0, 10, 0),
                Margin = new Thickness(8, 0, 0, 0),
            };
            upButton.Click += (_, _) => GoUp();

            var pathRow = new Grid();
            pathRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            pathRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            Grid.SetColumn(_pathText, 0);
            Grid.SetColumn(upButton, 1);
            pathRow.Children.Add(_pathText);
            pathRow.Children.Add(upButton);

            _listView = new ApeListView
            {
                SelectionMode = SelectionMode.Single,
                Margin = new Thickness(0, 8, 0, 0),
                ItemTemplate = CreateItemTemplate(),
            };
            _listView.MouseDoubleClick += OnListDoubleClick;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            Grid.SetRow(ContentTextBlock, 0);
            Grid.SetRow(pathRow, 1);
            Grid.SetRow(_listView, 2);
            grid.Children.Add(ContentTextBlock);
            grid.Children.Add(pathRow);
            grid.Children.Add(_listView);
            Host.SetDialogContent(grid);

            var initial = string.IsNullOrWhiteSpace(_settings.InitialPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                : _settings.InitialPath!;
            LoadDirectory(initial);

            AddDefaultOptions();
        }

        private static DataTemplate CreateItemTemplate()
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding(nameof(SelectionItem.DisplayText)));
            factory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            factory.SetValue(TextBlock.TextTrimmingProperty, TextTrimming.CharacterEllipsis);
            var template = new DataTemplate(typeof(SelectionItem));
            template.VisualTree = factory;
            return template;
        }

        private void LoadDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            _currentPath = path;
            _pathText.Text = path;

            var items = new List<SelectionItem>();
            try
            {
                items = Directory.GetDirectories(path)
                    .OrderBy(DirectoryName)
                    .Select(directory => new SelectionItem(directory, "📁 " + DirectoryName(directory)))
                    .ToList();
            }
            catch (UnauthorizedAccessException)
            {
                items = new List<SelectionItem>();
            }
            catch (IOException)
            {
                items = new List<SelectionItem>();
            }

            _listView.ItemsSource = items;
        }

        private static string DirectoryName(string path)
        {
            var name = Path.GetFileName(path.TrimEnd('\\', '/'));
            return string.IsNullOrEmpty(name) ? path : name;
        }

        private void GoUp()
        {
            var parent = Directory.GetParent(_currentPath)?.FullName;
            if (parent is not null)
            {
                LoadDirectory(parent);
            }
        }

        private void OnListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem is SelectionItem selection)
            {
                var path = (string)selection.Item!;
                if (Directory.Exists(path))
                {
                    LoadDirectory(path);
                }
            }
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(new[] { _currentPath });
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(new[] { _currentPath });
    }
}
