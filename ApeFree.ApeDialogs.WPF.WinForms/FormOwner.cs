using System;
using System.Windows.Forms;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.WinForms
{
    /// <summary>
    /// WinForms 宿主窗口适配器（Form → IWin32Owner）。
    /// </summary>
    public sealed class FormOwner : IWin32Owner
    {
        private readonly Form _form;

        /// <summary>
        /// 构造 WinForms 宿主适配器。
        /// </summary>
        public FormOwner(Form form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
        }

        /// <inheritdoc/>
        public IntPtr Handle => _form.Handle;
    }
}
