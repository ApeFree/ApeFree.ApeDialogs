using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs.Tests
{
    /// <summary>
    /// 测试用进度 Dialog（兼作实现方参考实现）。
    /// </summary>
    public class TestProgressDialog : BaseDialog<object, object, object, bool>, IProgressDialog
    {
        public List<double> ProgressHistory { get; } = new List<double>();
        public string? LastMessage { get; private set; }

        public TestProgressDialog(ProgressDialogSettings settings) : base(settings)
        {
            AddDefaultOptions();
        }

        public bool IsIndeterminate { get; private set; }

        public override string Title { get; set; } = string.Empty;
        public override string Content { get; set; } = string.Empty;

        public override object AddOption(DialogOption dialogOption) => new object();

        public override void ClearOptions()
        {
        }

        protected override void ShowHandler()
        {
            RaiseShown();
        }

        protected override void DismissHandler()
        {
        }

        public void UpdateProgress(double progress, string? message = null)
        {
            ProgressHistory.Add(progress);
            if (message != null)
            {
                LastMessage = message;
            }
        }

        public void SetIndeterminate(bool value) => IsIndeterminate = value;
    }
}
