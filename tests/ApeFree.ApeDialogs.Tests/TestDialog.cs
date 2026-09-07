using System;
using System.Collections.Generic;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs.Tests
{
    /// <summary>
    /// 测试用 Dialog（兼作实现方参考实现）。
    /// </summary>
    public class TestDialog<TResult> : BaseDialog<object, object, object, TResult>
    {
        public List<DialogOption> Options { get; } = new List<DialogOption>();

        public bool ShowHandlerCalled { get; private set; }
        public bool DismissHandlerCalled { get; private set; }
        public ValidationResult? LastValidationFailure { get; private set; }

        public new DialogSettings<TResult> Settings => base.Settings;

        public TestDialog(DialogSettings<TResult> settings) : base(settings)
        {
            AddDefaultOptions();
        }

        public override string Title { get; set; } = string.Empty;
        public override string Content { get; set; } = string.Empty;

        public override object AddOption(DialogOption dialogOption)
        {
            Options.Add(dialogOption);
            return new object();
        }

        public override void ClearOptions() => Options.Clear();

        protected override void ShowHandler()
        {
            ShowHandlerCalled = true;
            RaiseShown();
        }

        protected override void DismissHandler()
        {
            DismissHandlerCalled = true;
        }

        protected override void OnValidationFailed(ValidationResult result)
        {
            LastValidationFailure = result;
        }

        public bool SimulateSubmit(TResult value) => TrySubmit(value);
    }
}
