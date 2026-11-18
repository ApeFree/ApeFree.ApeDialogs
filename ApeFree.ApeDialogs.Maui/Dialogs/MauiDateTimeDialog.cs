using System;
using ApeFree.ApeDialogs.Maui.Hosting;
using ApeFree.ApeDialogs.Maui.Theme;
using ApeFree.ApeDialogs.Settings;
using ApeFree.ApeDialogs.Theme;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace ApeFree.ApeDialogs.Maui.Dialogs
{
    /// <summary>
    /// 日期与时间对话框（MAUI 实现，支持日期、时间与组合选择）。
    /// </summary>
    public partial class MauiDateTimeDialog : MauiDialogBase<DateTime>
    {
        private readonly DatePicker _datePicker;
        private readonly TimePicker _timePicker;
        private readonly DateTimePrecision _precision;

        public MauiDateTimeDialog(DateTimeDialogSettings settings, IMauiDialogContext? context = default, IDialogTheme? theme = null)
            : base(settings, context, theme ?? DefaultTheme.Instance)
        {
            _precision = settings.Precision;
            var initialDateTime = settings.DefaultDateTime != default ? settings.DefaultDateTime : DateTime.Now;

            var panel = new VerticalStackLayout { Spacing = 12 };

            _datePicker = new DatePicker
            {
                Date = initialDateTime.Date,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
            };

            _timePicker = new TimePicker
            {
                Time = initialDateTime.TimeOfDay,
                TextColor = MauiThemeManager.TextPrimary,
                BackgroundColor = MauiThemeManager.ControlBackground,
            };

            switch (settings.Precision)
            {
                case DateTimePrecision.Date:
                    panel.Children.Add(_datePicker);
                    break;

                case DateTimePrecision.Time:
                    panel.Children.Add(_timePicker);
                    break;

                case DateTimePrecision.DateTime:
                default:
                    panel.Children.Add(new Label { Text = "选择日期", FontSize = 12, TextColor = MauiThemeManager.TextSecondary });
                    panel.Children.Add(_datePicker);
                    panel.Children.Add(new Label { Text = "选择时间", FontSize = 12, TextColor = MauiThemeManager.TextSecondary });
                    panel.Children.Add(_timePicker);
                    break;
            }

            Host.SetDialogContent(panel);

            var confirmOption = settings.ConfirmOption with
            {
                Action = _ => TrySubmit(GetCalculatedDateTime())
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

            if (settings.CurrentTimeOption != null)
            {
                var nowOption = settings.CurrentTimeOption with
                {
                    Action = _ => SetNow()
                };
                AddOption(nowOption);
            }
        }

        private DateTime GetCalculatedDateTime()
        {
            var date = _datePicker.Date;
            var time = _timePicker.Time;

            return _precision switch
            {
                DateTimePrecision.Date => date,
                DateTimePrecision.Time => DateTime.Today.Add(time),
                _ => date.Add(time),
            };
        }

        private void SetNow()
        {
            var now = DateTime.Now;
            _datePicker.Date = now.Date;
            _timePicker.Time = now.TimeOfDay;
        }
    }
}
