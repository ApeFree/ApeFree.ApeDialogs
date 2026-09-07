namespace ApeFree.ApeDialogs.Settings
{
    /// <summary>
    /// 密码输入对话框配置。
    /// </summary>
    public class PasswordDialogSettings : InputDialogSettings
    {
        /// <summary>
        /// 密码遮掩字符。
        /// </summary>
        public char PasswordChar { get; set; } = '●';
    }
}
