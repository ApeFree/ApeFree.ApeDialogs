using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 保存文件对话框（WPF Fluent 实现）：自绘文件浏览器（路径导航 + 上级按钮 + 文件列表 + 文件名输入 + 保存）。
    /// </summary>
    public class WpfSaveFileDialog : WpfDialogBase<string>
    {
        private readonly SaveFileDialogSettings _settings;
        private readonly TextBlock _pathText;
        private readonly ApeTextBox _fileNameBox;
        private readonly ApeListView _listView;
        private string _currentPath = string.Empty;

        public WpfSaveFileDialog(SaveFileDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            Host.Width = 560;
            Host.Height = 480;

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
                Margin = new Thickness(0, 8, 0, 8),
                ItemTemplate = CreateItemTemplate(),
            };
            _listView.SelectionChanged += OnListSelectionChanged;
            _listView.MouseDoubleClick += OnListDoubleClick;

            _fileNameBox = new ApeTextBox
            {
                Text = settings.DefaultFileName ?? string.Empty,
                PlaceholderText = "File name",
                Height = 32,
                Margin = new Thickness(0, 0, 0, 4),
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            Grid.SetRow(ContentTextBlock, 0);
            Grid.SetRow(pathRow, 1);
            Grid.SetRow(_listView, 2);
            Grid.SetRow(_fileNameBox, 3);

            grid.Children.Add(ContentTextBlock);
            grid.Children.Add(pathRow);
            grid.Children.Add(_listView);
            grid.Children.Add(_fileNameBox);
            Host.SetDialogContent(grid);

            var initial = string.IsNullOrEmpty(_settings.InitialPath)
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
                var dirs = Directory.GetDirectories(path)
                    .OrderBy(DirectoryName)
                    .Select(d => new SelectionItem(d, "📁 " + DirectoryName(d)));
                items.AddRange(dirs);

                var files = GetMatchingFiles(path, _settings.SearchPattern)
                    .OrderBy(FileName)
                    .Select(f => new SelectionItem(f, "📄 " + FileName(f)));
                items.AddRange(files);
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }

            _listView.ItemsSource = items;
        }

        private static IEnumerable<string> GetMatchingFiles(string directoryPath, string? searchPattern)
        {
            if (string.IsNullOrWhiteSpace(searchPattern) || searchPattern.Trim() == "*.*")
            {
                searchPattern = "*";
            }

            var patterns = searchPattern
                .Split(new[] { ';', '|', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            if (patterns.Count == 0)
            {
                patterns.Add("*");
            }

            var matchedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pattern in patterns)
            {
                var pat = pattern;
                var openParen = pat.IndexOf('(');
                var closeParen = pat.IndexOf(')');
                if (openParen >= 0 && closeParen > openParen)
                {
                    pat = pat.Substring(openParen + 1, closeParen - openParen - 1).Trim();
                }

                try
                {
                    foreach (var file in Directory.GetFiles(directoryPath, pat))
                    {
                        matchedFiles.Add(file);
                    }
                }
                catch
                {
                }
            }

            return matchedFiles;
        }

        private static string DirectoryName(string path)
        {
            var name = Path.GetFileName(path.TrimEnd('\\', '/'));
            return string.IsNullOrEmpty(name) ? path : name;
        }

        private static string FileName(string path) => Path.GetFileName(path);

        private void GoUp()
        {
            var parent = Directory.GetParent(_currentPath)?.FullName;
            if (parent is not null)
            {
                LoadDirectory(parent);
            }
        }

        private void OnListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listView.SelectedItem is SelectionItem selection && selection.Item is string path && File.Exists(path))
            {
                _fileNameBox.Text = Path.GetFileName(path);
            }
        }

        private void OnListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_listView.SelectedItem is SelectionItem selection && selection.Item is string path)
            {
                if (Directory.Exists(path))
                {
                    LoadDirectory(path);
                }
                else if (File.Exists(path))
                {
                    _fileNameBox.Text = Path.GetFileName(path);
                    SubmitSave();
                }
            }
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => SubmitSave();
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => SubmitSave();

        private void SubmitSave()
        {
            var fileName = _fileNameBox.Text?.Trim();
            if (string.IsNullOrEmpty(fileName))
            {
                Host.PlayShakeAnimation();
                return;
            }

            var fullPath = Path.IsPathRooted(fileName) ? fileName : Path.Combine(_currentPath, fileName);
            if (!string.IsNullOrEmpty(fullPath))
            {
                TrySubmit(fullPath);
            }
        }
    }
}
