using System;
using System.IO;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 保存文件对话框（MAUI 跨平台实现）。
    /// </summary>
    public partial class MauiSaveFileDialog : MauiDialogBase<string>
    {
        private readonly SaveFileDialogSettings _settings;
        private readonly Entry _fileNameEntry;
        private readonly Label _errorLabel;
        private readonly string _targetDirectory;

        public MauiSaveFileDialog(SaveFileDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _settings = settings;
            _targetDirectory = !string.IsNullOrWhiteSpace(settings.InitialPath)
                ? settings.InitialPath
                : FileSystem.AppDataDirectory;

            var panel = new VerticalStackLayout { Spacing = 8 };

            var dirLabel = new Label
            {
                Text = $"保存位置: {_targetDirectory}",
                FontSize = 12,
                TextColor = MauiThemeManager.TextSecondary,
                LineBreakMode = LineBreakMode.TailTruncation,
            };
            panel.Children.Add(dirLabel);

            _fileNameEntry = new Entry
            {
                Text = settings.DefaultFileName ?? string.Empty,
                Placeholder = "请输入文件名",
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
            };
            _fileNameEntry.Completed += (_, _) => Commit();
            panel.Children.Add(_fileNameEntry);

            _errorLabel = new Label
            {
                TextColor = MauiThemeManager.NegativeColor,
                FontSize = 12,
                IsVisible = false,
            };
            panel.Children.Add(_errorLabel);

            Host.SetDialogContent(panel);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => Commit()
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

        private void Commit()
        {
            var fileName = _fileNameEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                _errorLabel.Text = "文件名不能为空";
                _errorLabel.IsVisible = true;
                _ = Host.PlayShakeAnimationAsync();
                return;
            }

            try
            {
                var fullPath = Path.Combine(_targetDirectory, fileName);
                TrySubmit(fullPath);
            }
            catch (Exception ex)
            {
                _errorLabel.Text = ex.Message;
                _errorLabel.IsVisible = true;
                _ = Host.PlayShakeAnimationAsync();
            }
        }
    }
}
