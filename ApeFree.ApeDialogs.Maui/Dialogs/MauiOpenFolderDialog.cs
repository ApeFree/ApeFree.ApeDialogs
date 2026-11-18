using System;
using System.Collections.Generic;
using System.IO;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 打开文件夹对话框（MAUI 跨平台实现）。
    /// </summary>
    public partial class MauiOpenFolderDialog : MauiDialogBase<IReadOnlyList<string>>
    {
        private readonly Entry _pathEntry;
        private readonly Label _errorLabel;

        public MauiOpenFolderDialog(OpenFolderDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            var panel = new VerticalStackLayout { Spacing = 8 };

            var initial = !string.IsNullOrWhiteSpace(settings.InitialPath)
                ? settings.InitialPath
                : FileSystem.AppDataDirectory;

            _pathEntry = new Entry
            {
                Text = initial,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
                Placeholder = "请输入或确认文件夹路径",
            };
            panel.Children.Add(_pathEntry);

            var quickButtons = new HorizontalStackLayout { Spacing = 8 };

            var appDataBtn = new Button
            {
                Text = "应用目录",
                FontSize = 11,
                HeightRequest = 32,
                Padding = new Microsoft.Maui.Thickness(8, 0),
                BackgroundColor = MauiThemeManager.ControlBackground,
                TextColor = MauiThemeManager.TextSecondary,
                BorderColor = MauiThemeManager.StrokeDefault,
                BorderWidth = 1,
            };
            appDataBtn.Clicked += (_, _) => _pathEntry.Text = FileSystem.AppDataDirectory;
            quickButtons.Children.Add(appDataBtn);

            var cacheBtn = new Button
            {
                Text = "缓存目录",
                FontSize = 11,
                HeightRequest = 32,
                Padding = new Microsoft.Maui.Thickness(8, 0),
                BackgroundColor = MauiThemeManager.ControlBackground,
                TextColor = MauiThemeManager.TextSecondary,
                BorderColor = MauiThemeManager.StrokeDefault,
                BorderWidth = 1,
            };
            cacheBtn.Clicked += (_, _) => _pathEntry.Text = FileSystem.CacheDirectory;
            quickButtons.Children.Add(cacheBtn);

            panel.Children.Add(quickButtons);

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
            var path = _pathEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                _errorLabel.Text = "路径不能为空";
                _errorLabel.IsVisible = true;
                _ = Host.PlayShakeAnimationAsync();
                return;
            }

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                TrySubmit(new[] { path });
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
