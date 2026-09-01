using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using ApeFree.ApeDialogs.WPF.Controls;
using ApeFree.ApeDialogs.WPF.Interop;

namespace ApeFree.ApeDialogs.WPF.Dialogs
{
    /// <summary>
    /// 单滑块调节对话框（WPF 实现）。拖动过程实时触发 <see cref="ApeFree.ApeDialogs.Core.IAdjustDialog{T}.ValueChanged"/> 与 Settings.OnValueChanged。
    /// </summary>
    public class WpfSliderDialog : WpfAdjustDialogBase<decimal>
    {
        private readonly SliderDialogSettings _settings;
        private readonly ApeSlider _slider;
        private readonly TextBlock _valueLabel;

        public WpfSliderDialog(SliderDialogSettings settings, IWin32Owner? owner, IDialogTheme theme)
            : base(settings, owner, theme)
        {
            _settings = settings;

            _slider = new ApeSlider
            {
                Minimum = (double)settings.Minimum,
                Maximum = (double)settings.Maximum,
                Value = (double)settings.DefaultValue,
                IsSnapToTickEnabled = settings.TickFrequency > 0,
                TickFrequency = (double)settings.TickFrequency,
                Margin = new Thickness(0, 12, 0, 0),
            };

            _valueLabel = new TextBlock
            {
                FontSize = 20,
                FontWeight = FontWeights.SemiBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Visibility = settings.ShowValueLabel ? Visibility.Visible : Visibility.Collapsed,
            };
            _valueLabel.SetResourceReference(TextBlock.ForegroundProperty, "ApeTextFillPrimary");

            UpdateLabel();

            // 拖动过程实时触发
            _slider.ValueChanged += (_, _) =>
            {
                UpdateLabel();
                RaiseValueChanged((decimal)_slider.Value);
            };

            var panel = new StackPanel();
            panel.Children.Add(ContentTextBlock);
            panel.Children.Add(_valueLabel);
            panel.Children.Add(_slider);
            Host.SetDialogContent(panel);

            AddDefaultOptions();
        }

        /// <inheritdoc/>
        public override decimal Value => (decimal)_slider.Value;

        private void UpdateLabel()
        {
            _valueLabel.Text = _slider.Value.ToString("0.##", CultureInfo.CurrentCulture);
        }

        /// <inheritdoc/>
        public override ApeButton AddOption(DialogOption dialogOption)
        {
            var button = base.AddOption(dialogOption);

            if (ReferenceEquals(dialogOption, _settings.ConfirmOption))
            {
                button.Click += (_, _) => TrySubmit((decimal)_slider.Value);
            }

            return button;
        }

        /// <inheritdoc/>
        protected override void OnConfirmRequested() => TrySubmit((decimal)_slider.Value);
    }
}
