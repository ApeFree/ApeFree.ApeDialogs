using System;
using System.IO;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 保存文件对话框（WPF 实现）：基于 Windows 原生 Common Item Dialog 封装，
    /// 原生支持任意驱动器盘符切换、覆盖提示与外壳集成。
    /// 无论在 WPF 还是 WinForms 宿主下均能完美绑定父窗口所有权。
    /// </summary>
    public class WpfSaveFileDialog : WpfNativeFileDialogBase<SaveFileDialogSettings, string>
    {
        private readonly SaveFileDialogSettings _settings;

        public WpfSaveFileDialog(SaveFileDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <inheritdoc/>
        protected override void ShowNativeDialog()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = Title,
                OverwritePrompt = _settings.OverwritePrompt,
            };

            if (!string.IsNullOrWhiteSpace(_settings.DefaultFileName))
            {
                dialog.FileName = _settings.DefaultFileName;
            }

            if (!string.IsNullOrWhiteSpace(_settings.InitialPath) && Directory.Exists(_settings.InitialPath))
            {
                dialog.InitialDirectory = _settings.InitialPath;
            }

            if (!string.IsNullOrWhiteSpace(_settings.SearchPattern) && _settings.SearchPattern != "*")
            {
                dialog.Filter = $"Supported files ({_settings.SearchPattern})|{_settings.SearchPattern}|All files (*.*)|*.*";
            }

            IntPtr ownerHwnd = ResolveOwnerHwnd();
            bool? result = ShowCommonDialog(dialog, ownerHwnd);

            if (result == true)
            {
                TrySubmit(dialog.FileName);
            }
            else
            {
                Cancel();
            }
        }
    }
}
