using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Storage;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 打开文件对话框（MAUI 实现，直调跨平台原生文件选取器）。
    /// </summary>
    public partial class MauiOpenFileDialog : MauiDialogBase<IReadOnlyList<string>>
    {
        private readonly OpenFileDialogSettings _settings;

        public MauiOpenFileDialog(OpenFileDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _settings = settings;
        }

        protected override void ShowHandler()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    RaiseShown();

                    if (_settings.MultiSelect)
                    {
                        var results = await FilePicker.Default.PickMultipleAsync();
                        if (results != null && results.Any())
                        {
                            var paths = results.Select(r => r.FullPath).ToList();
                            TrySubmit(paths);
                            return;
                        }
                    }
                    else
                    {
                        var result = await FilePicker.Default.PickAsync();
                        if (result != null)
                        {
                            TrySubmit(new[] { result.FullPath });
                            return;
                        }
                    }

                    // 用户取消
                    Cancel();
                }
                catch
                {
                    Cancel();
                }
            });
        }

        protected override void DismissHandler()
        {
            // 原生对话框无需宿主退场动画
        }
    }
}
