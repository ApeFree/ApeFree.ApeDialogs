using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;

namespace ApeFree.ApeDialogs.Tests
{
    /// <summary>
    /// 测试用实时调节 Dialog（兼作实现方参考实现）。
    /// </summary>
    public class TestAdjustDialog<TResult> : BaseAdjustDialog<object, object, object, TResult>
    {
        public TestAdjustDialog(DialogSettings<TResult> settings) : base(settings)
        {
            AddDefaultOptions();
        }

        public override string Title { get; set; } = string.Empty;
        public override string Content { get; set; } = string.Empty;
        public override TResult Value => _value;

        private TResult _value = default!;

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

        /// <summary>
        /// 模拟一次调节动作：更新值并触发值变化。
        /// </summary>
        public void SimulateAdjust(TResult newValue)
        {
            _value = newValue;
            RaiseValueChanged(newValue);
        }

        public void SimulateSubmit() => Dismiss(DialogResult<TResult>.Confirmed(Value));
    }
}
