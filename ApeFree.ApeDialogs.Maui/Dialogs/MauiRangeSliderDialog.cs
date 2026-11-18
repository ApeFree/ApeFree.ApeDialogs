using System;
using ApeFree.ApeDialogs.Core;
using ApeFree.ApeDialogs.Maui.Controls;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 双滑块范围调节对话框（MAUI 实现，自绘支持）。
    /// </summary>
    public partial class MauiRangeSliderDialog : MauiAdjustDialogBase<ValueRange<decimal>>
    {
        private ValueRange<decimal> _value;
        private readonly ApeRangeSlider _rangeSlider;
        private readonly Label _valueLabel;

        public MauiRangeSliderDialog(RangeSliderDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _value = new ValueRange<decimal>(settings.DefaultStart, settings.DefaultEnd);

            var panel = new VerticalStackLayout { Spacing = 12 };

            _valueLabel = new Label
            {
                Text = $"{_value.Start:G} ~ {_value.End:G}",
                FontSize = 20,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = MauiThemeManager.AccentColor,
                IsVisible = settings.ShowValueLabel,
            };
            panel.Children.Add(_valueLabel);

            _rangeSlider = new ApeRangeSlider
            {
                Minimum = settings.Minimum,
                Maximum = settings.Maximum,
                Start = settings.DefaultStart,
                End = settings.DefaultEnd,
            };

            _rangeSlider.RangeChanged += (_, _) =>
            {
                _value = new ValueRange<decimal>(_rangeSlider.Start, _rangeSlider.End);
                _valueLabel.Text = $"{_value.Start:G} ~ {_value.End:G}";
                RaiseValueChanged(_value);
            };
            panel.Children.Add(_rangeSlider);

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

        public override ValueRange<decimal> Value => _value;
    }
}
