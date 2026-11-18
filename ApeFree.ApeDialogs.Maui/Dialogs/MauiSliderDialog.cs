using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 单滑块调节对话框（MAUI 实现，支持实时回调）。
    /// </summary>
    public partial class MauiSliderDialog : MauiAdjustDialogBase<decimal>
    {
        private decimal _value;
        private readonly Slider _slider;
        private readonly Label _valueLabel;

        public MauiSliderDialog(SliderDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _value = settings.DefaultValue;

            var panel = new VerticalStackLayout { Spacing = 12 };

            _valueLabel = new Label
            {
                Text = _value.ToString("G"),
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = MauiThemeManager.AccentColor,
                IsVisible = settings.ShowValueLabel,
            };
            panel.Children.Add(_valueLabel);

            _slider = new Slider
            {
                Minimum = (double)settings.Minimum,
                Maximum = (double)settings.Maximum,
                Value = (double)settings.DefaultValue,
                MinimumTrackColor = MauiThemeManager.AccentColor,
                ThumbColor = MauiThemeManager.AccentColor,
            };

            _slider.ValueChanged += (_, e) =>
            {
                var step = (double)settings.TickFrequency;
                var raw = e.NewValue;
                if (step > 0)
                {
                    raw = Math.Round(raw / step) * step;
                }
                _value = (decimal)raw;
                _valueLabel.Text = _value.ToString("G");
                RaiseValueChanged(_value);
            };
            panel.Children.Add(_slider);

            Host.SetDialogContent(panel);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(_value)
            };
            AddOption(confirmOption);

            if (settings.CancelOption != null)
            {
                var cancelOption = settings.CancelOption with
                {
                    Action = _ => Cancel()
                };
                AddOption(cancelOption);
            }
        }

        public override decimal Value => _value;
    }
}
