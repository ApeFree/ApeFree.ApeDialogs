using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 双滑块范围调节对话框（WPF 实现）。拖动过程实时触发 <see cref="IAdjustDialog{T}.ValueChanged"/> 与 Settings.OnValueChanged。
    /// </summary>
    public class WpfRangeSliderDialog : WpfAdjustDialogBase<ValueRange<decimal>>
    {
        private readonly RangeSliderDialogSettings _settings;
        private readonly ApeRangeSlider _slider;
        private readonly TextBlock _rangeLabel;

        public WpfRangeSliderDialog(RangeSliderDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _slider = new ApeRangeSlider
            {
                Minimum = settings.Minimum,
                Maximum = settings.Maximum,
                Start = settings.DefaultStart,
                End = settings.DefaultEnd,
                Height = 32,
                Margin = new Thickness(8, 16, 8, 0),
            };

            _rangeLabel = new TextBlock
            {
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = settings.ShowValueLabel ? Visibility.Visible : Visibility.Collapsed,
            };
            _rangeLabel.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillPrimary");

            UpdateLabel();

            // 拖动过程实时触发
            _slider.RangeChanged += (_, _) =>
            {
                UpdateLabel();
                RaiseValueChanged(Value);
            };

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_rangeLabel);
            panel.Children.Add(_slider);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override ValueRange<decimal> Value => new(_slider.Start, _slider.End);

        private void UpdateLabel()
        {
            var format = "0.##";
            _rangeLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0} ~ {1}",
                _slider.Start.ToString(format, CultureInfo.CurrentCulture),
                _slider.End.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit(Value);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit(Value);
    }
}
