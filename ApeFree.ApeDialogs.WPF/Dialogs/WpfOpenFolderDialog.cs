using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 打开文件夹对话框（WPF 实现）：基于 Windows 原生现代文件夹选取器封装，
    /// 原生支持任意驱动器盘符切换与多选。
    /// 无论在 WPF 还是 WinForms 宿主下均能完美绑定父窗口所有权。
    /// </summary>
    public class WpfOpenFolderDialog : WpfNativeFileDialogBase<OpenFolderDialogSettings, IReadOnlyList<string>>
    {
        private readonly OpenFolderDialogSettings _settings;

        public WpfOpenFolderDialog(OpenFolderDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <inheritdoc/>
        protected override void ShowNativeDialog()
        {
            IntPtr ownerHwnd = ResolveOwnerHwnd();

#if NET8_0_OR_GREATER
            var dialog = new Microsoft.Win32.OpenFolderDialog
            {
                Title = Title,
                Multiselect = _settings.MultiSelect,
            };

            if (!string.IsNullOrWhiteSpace(_settings.InitialPath) && Directory.Exists(_settings.InitialPath))
            {
                dialog.InitialDirectory = _settings.InitialPath;
            }

            bool? result = ShowCommonDialog(dialog, ownerHwnd);
            if (result == true)
            {
                TrySubmit(dialog.FolderNames);
            }
            else
            {
                Cancel();
            }
#else
            var paths = ShowNativeFolderDialog(ownerHwnd, Title, _settings.InitialPath, _settings.MultiSelect);
            if (paths is not null && paths.Count > 0)
            {
                TrySubmit(paths);
            }
            else
            {
                Cancel();
            }
#endif
        }

#if !NET8_0_OR_GREATER
        private static IReadOnlyList<string>? ShowNativeFolderDialog(IntPtr parentHwnd, string title, string? initialPath, bool multiSelect)
        {
            var dialog = (IFileOpenDialog)new FileOpenDialogRCW();
            try
            {
                uint options = FOS_PICKFOLDERS | FOS_PATHMUSTEXIST | FOS_FORCEFILESYSTEM;
                if (multiSelect)
                {
                    options |= FOS_ALLOWMULTISELECT;
                }
                dialog.SetOptions(options);

                if (!string.IsNullOrWhiteSpace(title))
                {
                    dialog.SetTitle(title);
                }

                if (!string.IsNullOrWhiteSpace(initialPath) && Directory.Exists(initialPath))
                {
                    if (SHCreateItemFromParsingName(initialPath!, IntPtr.Zero, typeof(IShellItem).GUID, out var folderItem) == 0 && folderItem != null)
                    {
                        dialog.SetFolder(folderItem);
                    }
                }

                int hr = dialog.Show(parentHwnd);
                if (hr != 0) // Canceled (e.g. 0x800704C7)
                {
                    return null;
                }

                var results = new List<string>();
                if (multiSelect)
                {
                    dialog.GetResults(out var itemArray);
                    if (itemArray != null)
                    {
                        itemArray.GetCount(out uint count);
                        for (uint i = 0; i < count; i++)
                        {
                            itemArray.GetItemAt(i, out var item);
                            if (item != null)
                            {
                                item.GetDisplayName(SIGDN_FILESYSPATH, out var path);
                                if (!string.IsNullOrWhiteSpace(path))
                                {
                                    results.Add(path);
                                }
                            }
                        }
                    }
                }
                else
                {
                    dialog.GetResult(out var item);
                    if (item != null)
                    {
                        item.GetDisplayName(SIGDN_FILESYSPATH, out var path);
                        if (!string.IsNullOrWhiteSpace(path))
                        {
                            results.Add(path);
                        }
                    }
                }

                return results;
            }
            finally
            {
                Marshal.ReleaseComObject(dialog);
            }
        }

        private const uint FOS_PICKFOLDERS = 0x00000020;
        private const uint FOS_FORCEFILESYSTEM = 0x00000040;
        private const uint FOS_ALLOWMULTISELECT = 0x00000200;
        private const uint FOS_PATHMUSTEXIST = 0x00000800;
        private const uint SIGDN_FILESYSPATH = 0x80058000;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SHCreateItemFromParsingName(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [MarshalAs(UnmanagedType.LPStruct)] Guid riid,
            out IShellItem ppv);

        [ComImport]
        [Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        private class FileOpenDialogRCW { }

        [ComImport]
        [Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IFileOpenDialog
        {
            [PreserveSig] int Show(IntPtr parent);
            void SetFileTypes(uint cFileTypes, IntPtr rgFilterSpec);
            void SetFileTypeIndex(uint iFileType);
            void GetFileTypeIndex(out uint piFileType);
            void Advise(IntPtr pfde, out uint pdwCookie);
            void Unadvise(uint dwCookie);
            void SetOptions(uint fos);
            void GetOptions(out uint pfos);
            void SetDefaultFolder(IShellItem psi);
            void SetFolder(IShellItem psi);
            void GetFolder(out IShellItem ppsi);
            void GetCurrentSelection(out IShellItem ppsi);
            void SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
            void SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
            void SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
            void SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
            void GetResult(out IShellItem ppsi);
            void AddPlace(IShellItem psi, int fdap);
            void SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
            void Close(int hr);
            void SetClientGuid(ref Guid guid);
            void ClearClientData();
            void SetFilter(IntPtr pFilter);
            void GetResults(out IShellItemArray ppenum);
            void GetSelectedItems(out IShellItemArray ppsai);
        }

        [ComImport]
        [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
            void BindToHandler(IntPtr pbc, ref Guid bhid, ref Guid riid, out IntPtr ppv);
            void GetParent(out IShellItem ppsi);
            void GetDisplayName(uint sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            void Compare(IShellItem psi, uint hint, out int piOrder);
        }

        [ComImport]
        [Guid("b63ea76d-1f85-456f-a19c-48159efa858b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemArray
        {
            void BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut);
            void GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv);
            void GetPropertyDescriptionList(ref Guid keyType, ref Guid riid, out IntPtr ppv);
            void GetAttributes(int AttribFlags, uint sfgaoMask, out uint psfgaoAttribs);
            void GetCount(out uint pdwNumItems);
            void GetItemAt(uint dwIndex, out IShellItem ppsi);
            void EnumItems(out IntPtr ppenumShellItems);
        }
#endif
    }
}
